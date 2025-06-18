
using EcoSim.Objects;
using System.Numerics;
using EcoSim.Planet;
using EcoSim.Ships;
using EcoSim.Interfaces;
using System.Drawing;

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

        static internal void ShipSim()
        { 
            Ship ship = new("SS Enterprise", new(0,0), 10, 1000);

            Dictionary<string, DummyPlanet> planets = new()
            {
                { "earth", new DummyPlanet("Earth", new Point(0, 0)) },
                { "mars", new DummyPlanet("Mars", new Point(4, 0)) },
                { "venus", new DummyPlanet("Venus", new Point(2, 3)) }
            };

            planets["earth"].AddPrice("Food", 10);
            planets["earth"].AddPrice("Fuel", 5);
            planets["earth"].AddPrice("Minerals", 15);

            planets["mars"].AddPrice("Food", 12);
            planets["mars"].AddPrice("Fuel", 6);
            planets["mars"].AddPrice("Minerals", 18);
            
            planets["venus"].AddPrice("Food", 8);
            planets["venus"].AddPrice("Fuel", 4);
            planets["venus"].AddPrice("Minerals", 10);

            while (true)
            {
                String oString = ship.ToString() + "\n";
                DummyPlanet? currentPlanet = planets.Values.FirstOrDefault(p => p.Position == ship.Position);

                foreach (DummyPlanet? planet in planets.Values)
                {
                    oString += planet.ToString() + "\n";
                }

                Console.WriteLine(oString);
                Console.WriteLine("Enter command (refuel, jump, market, exit):");
                // Wait for user input to continue
                string iString = Console.ReadLine()?.ToLower() ?? "";

                if(iString == "exit")
                    break;
                if(iString == "jump")
                {
                    Console.WriteLine("Enter Destination:");
                    iString = Console.ReadLine()?.ToLower() ?? "";
                    DummyPlanet? destination = planets.GetValueOrDefault(iString);
                    if (destination == null)
                    {
                        Console.WriteLine("Invalid destination.");
                        continue;
                    }
                    if (ship.TryJump(destination.Position)) // Assuming 10 is the fuel cost for a jump
                    {
                        Console.WriteLine($"Jumped to {destination.Name} at position {destination.Position}.");
                    }
                    else
                    {
                        Console.WriteLine("Not enough fuel to jump.");
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
                Console.WriteLine($"Resource Stockpiles: {string.Join(", ", earth.Stockpiles.Select(kv => $"{kv.Key}: {kv.Value.Quantity}"))}");
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