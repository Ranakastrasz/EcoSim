
using EcoSim.Interfaces;
using EcoSim.Objects;
using EcoSim.Planet;
using EcoSim.Ships;
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
                PlanetSim();    
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

        record ConsoleCommand(string Name, string[] Args);

        static ConsoleCommand GetConsoleCommand()
        {
            string input = Console.ReadLine()?.ToLower() ?? "";
            string[] parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            return parts.Length == 0
                ? new ConsoleCommand("", Array.Empty<string>())
                : new ConsoleCommand(parts[0], parts.Skip(1).ToArray());
        }
        static bool TryGetIntArg(string[] args, int index, out int result)
        {
            result = 0;
            return args.Length > index && int.TryParse(args[index], out result);
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
                    ConsoleCommand command = GetConsoleCommand();

                    if(command.Name == "exit")
                        break;
                    // End sim.
                    if(command.Name == "jump")
                    {
                        string destinationString = command.Args[0] ?? "";

                        
                            Console.WriteLine(oString);

                            command = GetConsoleCommand();
                        
                            if (destinationString == "back")
                                break;

                            destinationString = destinationString.ToLower();
                            break;
                        }
                        if (destinationString == "back") // A planet named back would break this.
                            continue;

                        // Would want to do coordinates here too. But for now, don't bother. Just direct planet only.
                        // Probably check for "{int},{int}" or something.

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

                            command = GetConsoleCommand();
                            if (command.Name == "back")
                                break;
                    
                            string itemType = command.Args[0] ?? "";
                            if (!TryGetIntArg(command.Args,1,out int itemQuantity) || itemType == "")
                            {
                                Console.WriteLine($"");
                                continue;
                            }
                            if (itemQuantity <= 0)
                            { 
                                Console.WriteLine($"Invalid Quantity {itemQuantity}");
                                continue;
                            }
                            LabeledValue<int> cargoStack = new(itemType, itemQuantity);
                    
                            LabeledValue<int>? marketEntry = currentPlanet.PriceMap.Find(p => p.Label == itemType);
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

        static internal void PlanetSim()
        { 

            SmartPlanet earth = new SmartPlanet();
            earth.AddPopulation(10); // Initial population
            // Food, Energy, Minerals are the only raw resources.
            // Later, strategic resources may be added
            // Produced resources like Metal, Alloy, Consumer Goods, Tools, but we need upkeep and factory jobs for those.
            

            earth.AddNaturalResource(new NaturalResource("Ore Deposits",12, new Job("Surface Mining", new LabeledValue<int>("Minerals", 1))));
            earth.AddNaturalResource(new NaturalResource("Wild Edibles and Foods",8, new Job("Hunter Gathering", new LabeledValue<int>("Food", 2))));
            
            // Natural energy is kinda connected to people innately. Job cap is infinite.
            // This is really more a workshop or something, I dunno. Energy is abstract.
            // Probably will want to not have this provide a job normally. Or as a job like "Unemployed" 
            earth.AddJobs(new Job("Manual Labour", new LabeledValue<int>("Energy", 1)), 100);

            // Coal power is a basic power source, but requires upkeep of minerals.
            earth.AddJobs(new Job("Coal Power", new LabeledValue<int>("Energy", 8), new LabeledValue<int>("Minerals",-3)), 2);


            Console.WriteLine($"Planet created with {earth.NaturalResources.Count} resources.");

            //earth.AddJobs(new Job("Energy Production", new Resource("Energy", 1)),4);
            earth.AssignJobs("Manual Labour", 1);
            earth.AssignJobs("Surface Mining", 2);
            earth.AssignJobs("Hunter Gathering", 5);
            earth.AssignJobs("Coal Power", 1);


            while (true)
            {
                earth.Update();
                Console.WriteLine($"Population: {earth.Population}");
                Console.WriteLine($"Unemployed Population: {earth.UnemployedPopulation}");
                Console.WriteLine($"Resource Deposits: {string.Join(", ", earth.NaturalResources.Select(kv => $"{kv.Key}: {kv.Value.AvailableDeposits}/{kv.Value.TotalDeposits}"))}");
                Console.WriteLine($"Resource Stockpiles: {string.Join(", ", earth.Stockpiles.Select(kv => $"{kv.Key}: {kv.Value.Value}"))}");
                Console.WriteLine($"Job Sectors: {string.Join(", ", earth.JobSectors.Select(kv => $"{kv.Key}: {kv.Value.Workers}/{kv.Value.JobSlots}"))}");
                Console.WriteLine($"Districts: {string.Join(", ", earth.Districts.Select(kv => $"{kv.Key}: {kv.Value.TotalSize}"))}");
                Console.WriteLine("--------------------------------------------------");
                // Wait for user input to continue
                if(Console.ReadLine() == "exit")
                    break;
            }   
        }
    }
}