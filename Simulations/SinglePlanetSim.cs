using AssertUtils;
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
using System.Security.AccessControl;
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
            var mineralWorker = new JobType("minerals"  ,"Miner",
                new []{new Labeled<float>("Minerals"    , 4) },
                new []{new Labeled<float>("Energy"      , 1) });

            var energyWorker = new JobType("energy" ,"Technician",
                new []{new Labeled<float>("Energy"  , 4)},
                new []{new Labeled<float>("Food"    , 1)});

            var foodWorker = new JobType("food"     ,"Farmer",
                new []{new Labeled<float>("Food"    , 4) },
                new []{new Labeled<float>("Energy"  , 1) });

            Labeled<int> districtCost = new("Minerals",50);



            List<DistrictType> districts = new();
            districts.Add(new DistrictType("minerals","Mining District", mineralWorker ,2, districtCost));
            districts.Add(new DistrictType("energy"  ,"Energy District", energyWorker  ,2, districtCost));
            districts.Add(new DistrictType("food"    ,"Food District"  , foodWorker    ,2, districtCost));

            planet.TryAddDistrictTypes(districts);
            foreach(DistrictType districtType in districts)
            {
                planet.AddDistrict(districtType, 1, out int districtsAdded);
            }

            List<Labeled<float>> stockpiles = new();
            stockpiles.Add(new("Minerals", 50));
            stockpiles.Add(new("Energy", 50));
            stockpiles.Add(new("Food", 50));

            planet.RegisterStockpiles(stockpiles);

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
        private void GetInput(out State newState)
        {
            try
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
                    CommandJob(command, out newState);
                    return;
                }
                else if(command.IsCommand("District"))
                {
                    CommandDistrict(command, out newState);
                    return;
                }
                else
                {
                    Console.WriteLine($"Command {command.Name} not recognized");
                    newState = State.Input;
                    return;
                }
            }
            catch(IndexOutOfRangeException e)
            {
                Console.WriteLine("Invalid command arguments. Please try again.");
                newState = State.Input;
                return;
            }
            catch(ArgumentOutOfRangeException e)
            {
                Console.WriteLine(e.Message);
                newState = State.Input;
                return;
            }
            catch(Exception e)
            {
                Console.WriteLine($"Error processing command: {e.Message}");
                newState = State.Input;
                return;
            }
            { 
               
            }
#pragma warning disable CS0162 // Unreachable code detected
            newState = State.Error; // Technically, should never happen
#pragma warning restore CS0162 // Unreachable code detected
        }

        private void CommandJob(ConsoleCommand command, out State newState)
        {
            // Invalid key breaks the whole thing.
            command.GetArg(0, out var keyArg);
            AssertUtil.ContainsKey(Planet.Jobs, keyArg);
            JobType jobType = Planet.Jobs[keyArg].BaseType;

            // What to do with it. 
            command.GetArg(1, out var actionArg);
            string QuantityArg = "";
            try
            {
                command.GetArg(2, out QuantityArg);
            }
            catch
            {
            }

            if(!float.TryParse(QuantityArg, out float value))
                value = float.PositiveInfinity;

            try
            {
                if(actionArg.Equals("add"))
                {
                    Planet.TryFillJobs(jobType, (int)value, out int workersAdded);
                    Console.WriteLine($"{Planet.Jobs[keyArg].Name} Added {workersAdded} jobs");
                }
                else if(actionArg.Equals("remove"))
                {
                    Planet.TryEmptyJobs(jobType, (int)value, out int workersAdded);
                    Console.WriteLine($"{Planet.Jobs[keyArg].Name} Removed {workersAdded} jobs");
                }
                else
                {
                    throw new ArgumentOutOfRangeException(nameof(command), $"Invalid Job Arguments[2]: {actionArg}");
                }
            }
            catch(Exception e)
            {
                if(e.Message != "Insufficient Job Slots") // Should be defined in a JobManager or JobSector class instead. But works for now.
                {
                    throw new Exception(e.Message, e);
                }
                else
                {
                    Console.WriteLine(e.Message); // Not ideal. But eh.
                }
            }
            newState = State.Draw;
        }
        private void CommandDistrict(ConsoleCommand command, out State newState)
        {
            // Invalid key breaks the whole thing.
            command.GetArg(0, out var keyArg);
            AssertUtil.ContainsKey(Planet.Districts, keyArg);

            DistrictType districtType = Planet.Districts[keyArg].BaseType;
            // What to do with it. 
            command.GetArg(1, out var actionArg);
            string QuantityArg = "";
            try
            {
                command.GetArg(2, out QuantityArg);
            }
            catch
            {
            }

            if(!float.TryParse(QuantityArg, out float value))
                value = float.PositiveInfinity;

            try
            {
                if(actionArg.Equals("add"))
                {
                    Planet.TryBuyDistrict(districtType, (int)value, out int districtsAdded);
                    //Planet.AddDistrict(districtType, (int)value, out int districtsAdded);
                    Console.WriteLine($"{Planet.Jobs[keyArg].Name} Added {districtsAdded} districts");
                }
                else if(actionArg.Equals("remove"))
                {
                    Planet.RemoveDistrict(districtType, (int)value, out int districtsRemoved);
                    Console.WriteLine($"{Planet.Jobs[keyArg].Name} Removed {districtsRemoved} districts");
                }
                else
                {
                    throw new ArgumentOutOfRangeException(nameof(command), $"Invalid District Arguments[2]: {actionArg}");
                }
            }
            catch(Exception e)
            {
                if (e.GetType() == typeof(ArgumentOutOfRangeException))
                {
                    Console.WriteLine(e.Message);
                }
                else
                {
                    Console.WriteLine(e.Message); // Not ideal. But eh.
                }
            }
            newState = State.Draw;
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
            Console.WriteLine($"Districts  : {string.Join(", ", Planet.Districts.Select(kv => $"{kv.Key}: {kv.Value.Count}"))}");
            Console.WriteLine($"Commands: Continue, Exit");
            Console.WriteLine($"Job <JobType> <add|remove> <1>");

        }
    }
}
