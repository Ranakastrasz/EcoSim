
using EcoSim.Interfaces;
using EcoSim.IO;
using EcoSim.Objects;
using EcoSim.Planet;
using EcoSim.Ships;
using EcoSim.Simulations;
using System.Drawing;
using System.IO;
using System.Linq.Expressions;
using System.Numerics;
using System.Text;

namespace EcoSim
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to EcoSim!");
            Console.WriteLine("Type 'exit' to quit the simulation.");
            Console.WriteLine("Type 'ship' to simulate a ship, or 'planet' to simulate a planet.");




            string? input = Console.ReadLine()?.ToLower();
            if(input == "exit")
                return;
            if(input == "planet")
            {
                SinglePlanetSim planetSim = new(new());
                planetSim.PlanetSim();
            }
            else if(input == "ship")
            {
                ShipSim();
            }
            else
            {
                Console.WriteLine("Invalid input. Please type 'ship' or 'planet' or 'exit'.");
            }
        }

        static internal void ShipSim()
        { 
            Ship ship = new("SS Enterprise", new(0,0), 10, 1000);

            Dictionary<string, DummyPlanet> planets = new()
            {
                { "earth", new DummyPlanet("Earth", new Point(0, 0)) },
                { "mars", new DummyPlanet("Mars", new Point(4, 0)) },
                { "venus", new DummyPlanet("Venus", new Point(2, 3)) }
            };
            // Crappy manual prefabs. But, resource types are mutable, so eh.
            planets["earth"].AddPrice("Food", 10);
            planets["earth"].AddPrice("Fuel", 5);
            planets["earth"].AddPrice("Minerals", 15);

            planets["mars"].AddPrice("Food", 12);
            planets["mars"].AddPrice("Fuel", 6);
            planets["mars"].AddPrice("Minerals", 18);
            
            planets["venus"].AddPrice("Food", 8);
            planets["venus"].AddPrice("Fuel", 4);
            planets["venus"].AddPrice("Minerals", 10);
            
            while (true) // Main Sim Loop.
            {
                StringBuilder oString = new();
                // First, draw current state.
                ship.Draw(oString);
                ship.DrawCargo(oString);

                DummyPlanet? currentPlanet = planets.Values.FirstOrDefault(p => p.Position == ship.Position);
                // Currently, you are always at a planet, at least theoretically. But ship knows its location, not it's planet.

                foreach (DummyPlanet? planet in planets.Values)
                {
                    planet.Draw(oString);
                }

                oString.Append("---------------------------------------------------");
                // Next, wait for user input.
                oString.Append("Enter command (jump, market, exit):");
                // Need to tell users extra arguments. Jump {Planet Name}, Market opens a menu, kinda.
                while(true)
                {
                    // Wait for user input to continue
                    Console.WriteLine(oString);
                    ConsoleCommand command = ConsoleCommand.GetInput();

                    if(command.Name == "exit")
                        break;
                    // End sim.
                    if(command.Name == "jump")
                    {
                        string destinationString = command.Args[0] ?? "";
                        

                        // Right now, destination is a planet, and we get the position from that.
                        DummyPlanet? destination = planets.GetValueOrDefault(destinationString);
                        if (destination == null)
                        {
                            Console.WriteLine("Invalid destination.");
                            continue;
                        }
                        try
                        {
                            ship.TryJump(destination.Position);
                            Console.WriteLine($"Jump to {destination.Name} Successful.");
                            break;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message); // good enough for now.
                        }
                    }
                    else if (command.Name == "market")
                    {
                        // Enter market. First. draw stuff.
                    
                        if(currentPlanet != null)
                        {
                            oString = new();
                            ship.Draw(oString);
                            ship.DrawCargo(oString);
                            currentPlanet.Draw(oString);
                            currentPlanet.DrawMarket(oString);
                        }
                        else
                        {// Really, should be a catch, where I call "GetMarket(Position)
                            Console.WriteLine($"No Planet at {ship.Position} ");
                            continue;
                        }
                        while (true)
                        {
                            oString.Append("Enter command (buy, sell, back):");
                            Console.WriteLine(oString);

                            command = ConsoleCommand.GetInput();
                            if (command.Name == "back")
                                break;
                    
                            string itemType = command.Args[0] ?? "";
                            if (!ConsoleCommand.TryGetIntArg(command.Args,1,out int itemQuantity) || itemType == "")
                            {
                                Console.WriteLine($"");
                                continue;
                            }
                            if (itemQuantity <= 0)
                            { 
                                Console.WriteLine($"Invalid Quantity {itemQuantity}");
                                continue;
                            }
                            Labeled<int> cargoStack = new(itemType, itemQuantity);
                    
                            Labeled<int>? marketEntry = currentPlanet.PriceMap.Find(p => p.Key == itemType);
                            if (marketEntry == null)
                            {
                                Console.WriteLine($"Item type {itemType} does not exist in the Market");
                                continue;
                            }
                            // Everything should be valid, item and quanity wise.

                            if (command.Name == "buy")
                            {
                                ship.TryBuyCargo(cargoStack,marketEntry.Value);
                                break;
                            }
                            else if (command.Name == "sell")
                            { 
                                ship.TrySellCargo(cargoStack,marketEntry.Value);
                                break;
                            }
                            else
                            {
                                Console.WriteLine("Invalid Command");
                            }
                        }
                    }
                }
                Console.WriteLine("---------------------------------------------------");
                Console.WriteLine("Press Enter to Continue");
                Console.ReadLine();
            }   
        }
    }
}