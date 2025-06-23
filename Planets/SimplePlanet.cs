using EcoSim.Objects;
using EcoSim.Planets.Definitions;


namespace EcoSim.Planet
{
    public class SimplePlanet
    { 
        public Dictionary<string, DistrictStack> Districts               = []; // Districts produce jobs.
        public Inventory Stockpiles = new ();                             // Essentially a dictionary of Labeled<Float>
        public Dictionary<string, JobStack> JobSectors             = []; // Jobs, on Update, produce/consume resources.
        public int size = 0;

        public SimplePlanet()
        {
        }
        public void AddStockpiles(List<Labeled<float>> ItemList)
        { 
            foreach (Labeled<float> item in ItemList)
            { 
                AddToStockpile(item);
            }
        }

        public void AddToStockpile(Labeled<float> item)
        {
            Stockpiles.Add(item);
        }

        //Both probably need negative handling too, unless I have remove district instead.
        public void AddDistricts(List<DistrictStack> districts)
        { 
            foreach (DistrictStack district in districts)
            {
                AddDistrict(district);
            }
        }

        public void AddDistrict(DistrictStack district, int value = 0)
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
            AddJobs(district.Job, value);
        }

        public void Update()
        {
            // So, the jobSectors should be the only part we have to manage on update.
            // Tell it to update, and this should cause resource changes to occur.


            // Then, do all the jobs that exist, if plausible.
            foreach(JobStack sector in JobSectors.Values)
            {
                sector.Update(Stockpiles);
            }
        }
        private void AddJobs(Jobtype job, int quantity)
        {

            if(!JobSectors.TryGetValue(job.id, out var sector))
            {
                sector = new JobStack(job);
                JobSectors.Add(job.id, sector);
            }
            JobSectors[job.id].AddJobs(quantity,out int jobsAdded);
        }
        public void TryAssignJobs(string name, int quantity, out int workersAdded)
        {

            // Probably want a reverse function to unassign jobs too. or let this do negative quantities.
            // Assign workers to the job sector
            name = name.ToLower();
            var sector = JobSectors[name]; // Includes builtin assert.

            sector.AddWorkers(quantity,out workersAdded);
        }
    }
}