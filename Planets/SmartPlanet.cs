using EcoSim.Objects;


namespace EcoSim.Planet
{
    public class SmartPlanet // Needs to impliment iMarket.

    { 
        //public int Population = 0; // Kinda a resource too, but interacts with jobs very differently.
        //public int UnemployedPopulation => Population - JobSectors.Values.Sum(sector => sector.Workers);
        public Dictionary<string, NaturalResource> NaturalResources = [];
        public Dictionary<string, District> Districts               = [];
        public Dictionary<string, LabeledValue<int>> Stockpiles     = [];
        public Dictionary<string, JobSector> JobSectors             = [];
        public int size = 0;

        public SmartPlanet()
        {
        }
        //public void AddPopulation(int amount)
        //{
        //    Population += amount;
        //}
        public void AddStockpiles(List<LabeledValue<int>> ItemList)
        { 
            foreach (LabeledValue<int> item in ItemList)
            { 
                AddToStockpile(item);
            }
        }

        public void AddToStockpile(LabeledValue<int> item)
        {
            // If the item already exists, update its quantity
            if (Stockpiles.ContainsKey(item.Label))
            {
                Stockpiles[item.Label] = Stockpiles[item.Label] + item.Value;
            }
            else
            {
                Stockpiles.Add(item.Label, item);
            }
        }

        public void AddDistricts(List<District> districts)
        { 
            foreach (District district in districts)
            {
                // probably want error checking here.
                AddToDistrict(district);
            }
        }

        public void AddToDistrict(District district, int value = 0)
        { 
            // If the item already exists, update its quantity

            if (Districts.ContainsKey(district.Name))
            {
                Districts[district.Name].Size += value;
            }
            else
            {
                Districts.Add(district.Name, district);
            }
            AddJobs(district.Job, value); //
            AssignJobs(district.Job.Name,value); // Temporary.
        }

        public void Update()
        {
            // So, the jobSectors should be the only part we have to manage on update.
            // Tell it to update, and this should cause resource changes to occur.

            // Population eats food. Ideally this would be like, Population.update or something. Hardcode for now
            //if (Stockpiles.ContainsKey("Food"))
            //{
            //    int foodNeeded = Population;
            //    if (Stockpiles["Food"].Value < foodNeeded)
            //    {
            //        // Not enough food, Technically, the population should start to die off or something.
            //        Stockpiles["Food"] = Stockpiles["Food"].WithValue(0); // All food consumed
            //    }
            //    else
            //    {
            //        // Enough food, consume it
            //        Stockpiles["Food"] -= foodNeeded;
            //    }
            //}

            // Then, do all the jobs that exist, if plausible.
            foreach(JobSector sector in JobSectors.Values)
            {
                int jobsToWork = sector.Workers;
                // If there are no workers, skip this sector
                if(jobsToWork > 0)
                {
                    // If the job requires resources, check if we have enough
                    if(sector.Job.Upkeep != null)
                    {
                        var Upkeep = sector.Job.Upkeep.Value; // Get the upkeep delta

                        // Check if we have enough resources to pay the upkeep
                        int stockpileQuantity = Stockpiles.ContainsKey(Upkeep.Label) ? Stockpiles[Upkeep.Label].Value : 0;

                        // See how many jobs we have resources for
                        jobsToWork = Math.Min(sector.Workers, stockpileQuantity / sector.Job.Upkeep.Value);
                        

                        // Deduct upkeep from stockpile
                        Stockpiles[Upkeep.Label] = Stockpiles[Upkeep.Label] + Upkeep * jobsToWork;
                    }

                    if(!Stockpiles.TryGetValue(sector.Job.Yield.Label, out LabeledValue<int> yield))
                    {
                        Stockpiles.Add(sector.Job.Yield.Label, yield.WithQuantity(0));
                    }
                    Stockpiles[sector.Job.Yield.Label] = Stockpiles[sector.Job.Yield.Label] + sector.Job.Yield * jobsToWork;
                }
            }
        }

        internal void AddNaturalResource(NaturalResource naturalResource)
        {
            NaturalResources.TryGetValue(naturalResource.Name, out var resource);
            if (resource == null)
            {
                NaturalResources.Add(naturalResource.Name,naturalResource);
                resource = NaturalResources[naturalResource.Name];
            }
            resource.TotalDeposits += naturalResource.TotalDeposits;
            resource.AvailableDeposits += naturalResource.AvailableDeposits;
        }

        internal void AddJobs(Job job, int quantuty)
        {
            JobSectors.TryGetValue(job.Name, out var sector);
            if (sector == null)
            { 
                //JobSectors.Add(job.Name,job);
            }

            JobSectors.Add(job.Name, new JobSector(job)
            { JobSlots = quantuty });
        }
        public void AssignJobs(string Name, int quantity)
        {
            // Probably want a reverse function to unassign jobs too. or let this do negative quantities.
            //if(UnemployedPopulation < quantity)
            //{
            //    return; // Not enough population to assign jobs
            //}
            // Assign workers to the job sector
            if (JobSectors.ContainsKey(Name))
            {
                var sector = JobSectors[Name];
                sector.Workers += quantity;
                if (sector.Workers > sector.JobSlots)
                {
                    // If there are more workers than slots, cap it
                    sector.Workers = sector.JobSlots;
                }
            }
        }
    }
}