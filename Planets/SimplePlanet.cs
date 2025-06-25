using EcoSim.Extensions;
using EcoSim.Items;
using EcoSim.Objects;
using EcoSim.Planets.Definitions;
using EcoSim.Planets.Stacks;


namespace EcoSim.Planet
{
    public class SimplePlanet
    { 
        public Dictionary<string, DistrictStack> Districts = []; // Districts produce jobs.
        public Dictionary<string, JobStack> Jobs           = []; // Jobs, on Update, produce/consume resources.
        public Inventory Stockpiles = new ();
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
        public void TryAddDistrictTypes(List<DistrictType> districts)
        { 
            foreach (DistrictType district in districts)
            {
                TryAddDistrictType(district);
            }
        }

        public bool TryAddDistrictType(DistrictType type)
        {
            if(!Districts.ContainsKey(type.ID))
            {
                Districts[type.ID] = new DistrictStack(type);
                return true;
            }
            return false;
        }

        public void AddDistrict(DistrictType districtType, int value)
        {
            // If the item already exists, update its quantity
            DistrictStack district = Districts.ForceGet(districtType.ID, () => new DistrictStack(districtType));

            district.Size += value;
            AddJobs(district.Job, value); // Should be a sync jobs or something. Update jobs from districts.
        }

        public void Update()
        {
            // So, the jobSectors should be the only part we have to manage on update.
            // Tell it to update, and this should cause resource changes to occur.


            // Then, do all the jobs that exist, if plausible.
            foreach(JobStack sector in Jobs.Values)
            {
                sector.Update(Stockpiles);
            }
        }
        private void AddJobs(JobType job, int quantity)
        {

            if(!Jobs.TryGetValue(job.ID, out var sector))
            {
                sector = new JobStack(job);
                Jobs.Add(job.ID, sector);
            }
            Jobs[job.ID].Add(quantity);
        }
        public void TryAssignJobs(string name, int quantity, out int workersAdded)
        {

            // Probably want a reverse function to unassign jobs too. or let this do negative quantities.
            // Assign workers to the job sector
            name = name.ToLower();
            var sector = Jobs[name]; // Includes builtin assert.

            sector.AddWorkers(quantity,out workersAdded);
        }
    }
}