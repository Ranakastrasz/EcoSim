using EcoSim.IO;
using EcoSim.Objects;
using EcoSim.Planet;
using EcoSim.Planets.Definitions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace EcoSim.Simulations
{
    internal class SinglePlanetSim
    {
        public SimplePlanet Planet {get; private set; }

        public SinglePlanetSim(SimplePlanet planet)
        {

            Planet = planet;
            //planet.AddPopulation(10); // Initial population
            // Food, Energy, Minerals are the only raw resources.
            // Later, strategic resources may be added
            // Produced resources like Metal, Alloy, Consumer Goods, Tools, but we need upkeep and factory jobs for those.

            //NaturalResource mineralNode = new ("Mineral Deposit",12);
            //NaturalResource energyNode  = new ("Coal Deposit",6);
            //NaturalResource foodNode    = new ("Fertile Land",8);

            //planet.AddNaturalResource(mineralNode);
            //planet.AddNaturalResource(energyNode);
            //planet.AddNaturalResource(foodNode);
            var mineralWorker = new JobType("Miner",
                new []{new Labeled<float>("Minerals", 4) },
                new []{new Labeled<float>("Energy"  , 1) });

            var energyWorker = new JobType("Technician",
                new []{new Labeled<float>("Energy", 4)},
                new []{new Labeled<float>("Food"  , 1)});

            var foodWorker = new JobType("Farmer",
                new []{new Labeled<float>("Food", 4) },
                new []{new Labeled<float>("Energy"  , 1) });

            Labeled<int> districtCost = new("Minerals",50);



            List<DistrictType> districts = new();
            districts.Add(new DistrictType("Mining District", mineralWorker ,2, districtCost));
            districts.Add(new DistrictType("Energy District", energyWorker  ,2, districtCost));
            districts.Add(new DistrictType("Food District"  , foodWorker    ,2, districtCost));

            planet.TryAddDistrictTypes(districts);
            foreach(DistrictType districtType in districts)
            {
                planet.AddDistrict(districtType, 1);
            }

            List<Labeled<float>> stockpiles = new();
            stockpiles.Add(new("Minerals", 50));
            stockpiles.Add(new("Energy", 50));
            stockpiles.Add(new("Food", 50));

            planet.AddStockpiles(stockpiles);

        }
        enum State
        {
            Error,
            Draw,
            Input,
            Update,
            Exit
        }

        State state;
        internal void PlanetSim()
        {
            state = State.Draw;
            while (true)
            {
                try
                {
                    switch(state)
                    {

                        case State.Draw:
                        {
                            Draw();
                            state = State.Input;
                            break;
                        }
                        case State.Input:
                        {
                            // Draw input request.

                            GetInput(out State newState);
                            state = newState;
                            break;
                        }
                        case State.Update:
                        {
                            Update();
                            state = State.Draw;
                            break;
                        }
                        case State.Exit:
                        {
                            return;
                        }
                        case State.Error:
                        {
                            throw new Exception("Invalid Planet State");
                            
                        }
                        default:
                        {
                            return;
                        }
                    }
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.ToString());
                    state = State.Input;
                }
            }   
        }
        internal void Update()
        {

            Planet.Update();

            Console.WriteLine("--------------------------------------------------");
         }

        internal void Draw()
        {
            // Features currently disabled.

            //Console.WriteLine($"Population: {Planet.Population}");
            //Console.WriteLine($"Unemployed Population: {Planet.UnemployedPopulation}");
            //Console.WriteLine($"Resource Deposits: {string.Join(", ", Planet.NaturalResources.Select(kv => $"{kv.Key}: {kv.Value.AvailableDeposits}/{kv.Value.TotalDeposits}"))}");

            // This one should be Stockpile.Draw or something
            Console.WriteLine($"Resource Stockpiles: {string.Join(", ", Planet.Stockpiles.Items.Select(kv => $"{kv.Key}: {kv.Value}"))}");
            // Same with the rest, really. 
            Console.WriteLine($"Job Sectors: {string.Join(", ", Planet.Jobs.Select(kv => $"{kv.Key}: {kv.Value.Workers}/{kv.Value.Jobs}"))}");
            Console.WriteLine($"Districts  : {string.Join(", ", Planet.Districts.Select(kv => $"{kv.Key}: {kv.Value.Size}"))}");
            Console.WriteLine($"Commands: Continue, Exit");
            Console.WriteLine($"Job <JobType> <add|remove> <1>");

        }
        private void GetInput(out State newState)
        {
            ConsoleCommand command = ConsoleCommand.GetInput();
            if(command.IsCommand("Continue"))
            {
                newState = State.Update;
                return;
            }
            else if(command.IsCommand("Exit"))
            {
                newState = State.Exit;
                return;
            }
            else if(command.IsCommand("Job")) // Job Miner add 3, Or, Job.Miner.Add(3), I suppose.
            {
                // Invalid key breaks the whole thing.
                command.GetArg(0, out var keyArg);
                if(!Planet.Jobs.ContainsKey(keyArg))
                    throw new KeyNotFoundException();

                // What to do with it. 
                command.GetArg(1, out var actionArg);
                string valueArg = "";
                try
                {
                    command.GetArg(2, out valueArg);
                }
                catch
                {
                }

                if(!float.TryParse(valueArg, out float value))
                    value = float.PositiveInfinity;


                if(actionArg.Equals("add"))
                {
                }
                else if(actionArg.Equals("remove"))
                {
                    value = -value;
                }
                else
                {
                    throw new ArgumentNullException(actionArg);
                }

                try
                {
                    Planet.TryAssignJobs(keyArg, (int)value, out int workersAdded);
                    Console.WriteLine($"{Planet.Jobs[keyArg].Name} Added {workersAdded} jobs");
                }
                catch(Exception e)
                {
                    if(e.Message != "Insufficient Job Slots") // Should be defined in the JobManager or JobSector class instead. But works for now.
                    {
                        throw new Exception(e.Message, e);
                    }
                    else
                    {
                        Console.WriteLine(e.Message); // Not ideal. But eh.
                    }
                }
                newState = State.Draw;
                return;
            }
            else if(command.IsCommand("District"))
            {
                Console.WriteLine("Not yet implemented");
                // Try District command.
                newState = State.Input;
                return;
            }
            else
            {
                Console.WriteLine($"Command {command.Name} not recognized");
                newState = State.Input;
                return;
            }
#pragma warning disable CS0162 // Unreachable code detected
            newState = State.Error; // Technically, should never happen
#pragma warning restore CS0162 // Unreachable code detected
        }
    }
}
