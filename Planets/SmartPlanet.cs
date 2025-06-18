using EcoSim.Objects;


namespace EcoSim.Planet
{
    public class SmartPlanet // Needs to impliment iMarket.

    { 
        public int Population = 0; // Kinda a resource too, but interacts with jobs very differently.
        public int UnemployedPopulation => Population - JobSectors.Values.Sum(sector => sector.Workers);
        public Dictionary<string, NaturalResource> NaturalResources = [];
        public Dictionary<string, District> Districts               = [];
        public Dictionary<string, LabeledValue<int>> Stockpiles              = [];
        public Dictionary<string, JobSector> JobSectors             = [];

        public SmartPlanet()
        {
        }
        public void AddPopulation(int amount)
        {
            Population += amount;
        }
        public void AddStockpile(List<LabeledValue<int>> ItemList)
        { 
            foreach (LabeledValue<int> item in ItemList)
            { 
                AddItem(item);
                //Stockpiles[item.Label] = item; // This will overwrite existing items, which is fine for now.
            }
            //Stockpiles.Add("Food", new LabeledValue<int>("Food", 50));
            //Stockpiles.Add("Energy", new LabeledValue<int>("Energy", 20));
            //Stockpiles.Add("Minerals", new LabeledValue<int>("Minerals", 30));    
        }

        public void AddItem(LabeledValue<int> item)
        {
            // If the item already exists, update its quantity
            if (Stockpiles.ContainsKey(item.Label))
            {
                Stockpiles[item.Label] = Stockpiles[item.Label] + item.Quantity;
            }
            else
            {
                Stockpiles.Add(item.Label, item);
            }
        }
        public void Update()
        {
            // So, the jobSectors should be the only part we have to manage on update.
            // Tell it to update, and this should cause resource changes to occur.

            // Population eats food. Ideally this would be like, Population.update or something. Hardcode for now
            if (Stockpiles.ContainsKey("Food"))
            {
                int foodNeeded = Population;
                if (Stockpiles["Food"].Quantity < foodNeeded)
                {
                    // Not enough food, Technically, the population should start to die off or something.
                    Stockpiles["Food"] = Stockpiles["Food"].WithValue(0); // All food consumed
                }
                else
                {
                    // Enough food, consume it
                    Stockpiles["Food"] -= foodNeeded;
                }
            }

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
                        int stockpileQuantity = Stockpiles.ContainsKey(Upkeep.Label) ? Stockpiles[Upkeep.Label].Quantity : 0;

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
            if (!NaturalResources.TryGetValue(naturalResource.Job.Name, out NaturalResource? value))
            {
                NaturalResources.Add(naturalResource.Job.Name, naturalResource);
                JobSectors.Add(naturalResource.Job.Name, new JobSector(naturalResource.Job)
                { JobSlots = naturalResource.AvailableDeposits });
            }
            else
            {
                value.TotalDeposits += naturalResource.TotalDeposits;
                value.AvailableDeposits += naturalResource.AvailableDeposits;
            }
        }

        internal void AddJobs(Job job, int quantuty)
        {
            // Not sure if this is a function i really want to have in the Planet class, but it makes sense for now.
            JobSectors.Add(job.Name, new JobSector(job)
            { JobSlots = quantuty });
        }
        public void AssignJobs(string Name, int quantity)
        {
            // Probably want a reverse function to unassign jobs too. or let this do negative quantities.
            if(UnemployedPopulation < quantity)
            {
                return; // Not enough population to assign jobs
            }
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