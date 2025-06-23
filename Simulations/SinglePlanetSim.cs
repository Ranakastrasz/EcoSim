using EcoSim.Objects;
using EcoSim.Planet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoSim.Simulations
{
    internal class SinglePlanetSim
    {
        public SmartPlanet Planet {get; private set; }

        public SinglePlanetSim(SmartPlanet planet)
        {

            Planet = planet;
            //planet.AddPopulation(10); // Initial population
            // Food, Energy, Minerals are the only raw resources.
            // Later, strategic resources may be added
            // Produced resources like Metal, Alloy, Consumer Goods, Tools, but we need upkeep and factory jobs for those.
            
            NaturalResource mineralNode = new ("Mineral Deposit",12);
            NaturalResource energyNode  = new ("Coal Deposit",6);
            NaturalResource foodNode    = new ("Fertile Land",8);
            
            planet.AddNaturalResource(mineralNode);
            planet.AddNaturalResource(energyNode);
            planet.AddNaturalResource(foodNode);

            Job mineralWorker = new Job("Miner"     ,new("Minerals" ,4),new("Energy",1));
            Job energyWorker  = new Job("Technician",new("Energy"   ,4),new("Food"  ,1));
            Job foodWorker    = new Job("Farmer"    ,new("Food"     ,4),new("Energy",1));

            LabeledValue<int> districtCost = new("Minerals",50);

            List<District> districts = new();
            districts.Add(new District("Mining District",mineralWorker  ,districtCost,mineralNode));
            districts.Add(new District("Energy District",energyWorker   ,districtCost,energyNode));
            districts.Add(new District("Food District"  ,foodWorker     ,districtCost,foodNode));


            planet.AddDistricts(districts);
            planet.AddToDistrict(planet.Districts["Mining District"],1); // not the best way, but eh.
            planet.AddToDistrict(planet.Districts["Energy District"],1);
            planet.AddToDistrict(planet.Districts["Food District"],1);



        }
        internal void PlanetSim()
        { 


            while (true)
            {
                Planet.Update();
                //Console.WriteLine($"Population: {Planet.Population}");
                //Console.WriteLine($"Unemployed Population: {Planet.UnemployedPopulation}");
                Console.WriteLine($"Resource Deposits: {string.Join(", ", Planet.NaturalResources.Select(kv => $"{kv.Key}: {kv.Value.AvailableDeposits}/{kv.Value.TotalDeposits}"))}");
                Console.WriteLine($"Resource Stockpiles: {string.Join(", ", Planet.Stockpiles.Select(kv => $"{kv.Key}: {kv.Value.Value}"))}");
                Console.WriteLine($"Job Sectors: {string.Join(", ", Planet.JobSectors.Select(kv => $"{kv.Key}: {kv.Value.Workers}/{kv.Value.JobSlots}"))}");
                Console.WriteLine($"Districts: {string.Join(", ", Planet.Districts.Select(kv => $"{kv.Key}: {kv.Value.Size}"))}");
                Console.WriteLine("--------------------------------------------------");
                // Wait for user input to continue
                if(Console.ReadLine() == "exit")
                    break;
            }   
        }
    }
}
