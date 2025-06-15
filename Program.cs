
namespace EcoSim
{
    public class Resource
    {
        public string Name { get; set; }
        public int Quantity { get; set; }
        public Resource(string name, int quantity)
        {
            Name = name;
            Quantity = quantity;
        }
    }
    public struct Job
    {
        public string Name { get; set; }
        public Resource Yield { get; set; }
        public Resource? Upkeep { get; set; }
        public Job(string name, Resource yield, Resource? upkeep = null)
        {
            Name = name;
            Yield = yield;
            Upkeep = upkeep;
        }
        // Kinda want a method to try and run the job, but this is a struct.
        // Its the JobSector that will manage the workers and resources.
    }

    public class NaturalResource
    {
        // Raw resource deposits. Provides a basic natural job, or can be exploited by infrastructure.
        public int Name { get; set; }
        public int TotalDeposits { get; set; } // How many deposits exist
        public int AvailableDeposits { get; set; } // How many deposits are currently available for jobs

        public Job Job { get; set; } // The job that this provides.
        public NaturalResource(string Name, int deposits, Job job)
        {
            TotalDeposits = deposits;
            AvailableDeposits = deposits;
            Job = job;
        }
        // At most, maybe it talks to the District
    }
    public class JobSector
    {
        public Job Job { get; set; }
        public int JobSlots { get; set; }
        public int Workers { get; set; } // How many workers are currently assigned to this job

        public JobSector(Job job)
        {
            Job = job;
            JobSlots = 0;
            Workers = 0;
        }
        // Update, Tells the workers to do the job, and somehow sends the resource changes to the somewhere else.
    }

    public class District
    {
        public int TotalSize { get; set; } // How many deposits are being exploited

        public Job Job { get; set; } // The job that this provides.
        public District(Job job)
        {
            TotalSize = 0;
            Job = job;
        }
        // Infrastructure. Built on top of the natural resources, and provides a better job than the normal one.
        // Also removes the natural resource's job from the sector.
    }


    public class Planet
    { 
        public int Population = 0; // Kinda a resource too, but interacts with jobs very differently.
        public int UnemployedPopulation => Population - JobSectors.Values.Sum(sector => sector.Workers);
        public Dictionary<string, NaturalResource> NaturalResources = [];
        public Dictionary<string, District> Districts               = [];
        public Dictionary<string, Resource> Stockpiles              = [];
        public Dictionary<string, JobSector> JobSectors             = [];

        public Planet()
        {
        }
        public void AddPopulation(int amount)
        {
            Population += amount;
        }

        public void Update()
        {
            // So, the jobSectors should be the only part we have to manage on update.
            // Tell it to update, and this should cause resource changes to occur.

            // Population eats food
            if (Stockpiles.ContainsKey("Food"))
            {
                int foodNeeded = Population;
                if (Stockpiles["Food"].Quantity < foodNeeded)
                {
                    // Not enough food, Technically, the population should start to die off or something.
                    Stockpiles["Food"].Quantity = 0; // All food consumed
                }
                else
                {
                    // Enough food, consume it
                    Stockpiles["Food"].Quantity -= foodNeeded;
                }
            }

            foreach(JobSector sector in JobSectors.Values)
            {
                if (sector.Workers > 0)
                {
                    // Simulate job processing
                    if (sector.Job.Upkeep != null)
                    {
                        // Check if we have enough resources to pay for upkeep
                        if (!Stockpiles.ContainsKey(sector.Job.Upkeep.Name) || Stockpiles[sector.Job.Upkeep.Name].Quantity < sector.Job.Upkeep.Quantity * sector.Workers)
                        {
                            // Not enough resources to pay for upkeep, skip this sector
                            continue;
                        }
                        // Deduct upkeep from stockpiles
                        Stockpiles[sector.Job.Upkeep.Name].Quantity -= sector.Job.Upkeep.Quantity * sector.Workers;
                    }
                    // Produce yield
                    if (!Stockpiles.ContainsKey(sector.Job.Yield.Name))
                    {
                        Stockpiles.Add(sector.Job.Yield.Name, new Resource(sector.Job.Yield.Name, 0));
                    }
                    Stockpiles[sector.Job.Yield.Name].Quantity += sector.Job.Yield.Quantity * sector.Workers;
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
    class Program
    {
        static void Main(string[] args)
        {

            Planet earth = new Planet();
            earth.AddPopulation(10); // Initial population
            // Food, Energy, Minerals are the only raw resources.
            // Later, strategic resources may be added
            // Produced resources like Metal, Alloy, Consumer Goods, Tools, but we need upkeep and factory jobs for those.
            

            earth.AddNaturalResource(new NaturalResource("Ore Deposits",12, new Job("Surface Mining", new Resource("Minerals", 1))));
            earth.AddNaturalResource(new NaturalResource("Wild Edibles and Foods",8, new Job("Hunter Gathering", new Resource("Food", 2))));
            
            // Natural energy is kinda connected to people innately. Job cap is infinite.
            // This is really more a workshop or something, I dunno. Energy is abstract.
            // Probably will want to not have this provide a job normally. Or as a job like "Unemployed" 
            earth.AddJobs(new Job("Manual Labour", new Resource("Energy", 1)), 100);


            Console.WriteLine($"Planet created with {earth.NaturalResources.Count} resources.");

            //earth.AddJobs(new Job("Energy Production", new Resource("Energy", 1)),4);
            earth.AssignJobs("Manual Labour", 1);
            earth.AssignJobs("Surface Mining", 2);
            earth.AssignJobs("Hunter Gathering", 5);


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