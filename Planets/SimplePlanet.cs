using AssertUtils;
using EcoSim.Extensions;
using EcoSim.Items;
using EcoSim.Objects;
using EcoSim.Planets.Definitions;
using EcoSim.Planets.Stacks;
using System.Xml.Linq;

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
        public void RegisterStockpiles(List<Labeled<float>> ItemList)
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

        public void AddDistrict(DistrictType districtType, int quantity, out int districtsAdded)
        {
            AssertUtil.Positive(quantity);
            DistrictStack district = Districts.ForceGet(districtType.ID, () => new DistrictStack(districtType));

            district.Add(quantity);
            AddJobs(district.BaseType.Job, quantity*district.BaseType.JobCount, out int jobsAdded);
            districtsAdded = quantity;
            // Should be a sync jobs or something. Update jobs from districts.
            // Especially for Remove, since if it drops workers below the count.
            // Better a function recalculate and safely remove workers.
        }
        public void RemoveDistrict(DistrictType districtType, int quantity, out int districtsRemoved)
        {
            AssertUtil.Positive(quantity);
            DistrictStack district = Districts.ForceGet(districtType.ID, () => new DistrictStack(districtType));

            district.Remove(quantity, out districtsRemoved);
            RemoveJobs(district.BaseType.Job, quantity * district.BaseType.JobCount, out int jobsRemoved);
        }
        internal void TryBuyDistrict(DistrictType districtType, int quantity, out int districtsAdded)
        {
            AssertUtil.Positive(quantity);
            DistrictStack district = Districts.ForceGet(districtType.ID, () => new DistrictStack(districtType));

//            Stockpiles.CanSpend(district.BaseType.Price, out bool spent);
  //          int toBuy = Math.Min(quantity, district.BaseType.Price

            district.Add(quantity);
            AddJobs(district.BaseType.Job, quantity * district.BaseType.JobCount, out int jobsAdded);
            districtsAdded = quantity;
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
        private void AddJobs(JobType job, int quantity, out int JobsAdded)
        {
            if(!Jobs.TryGetValue(job.ID, out var jobs))
            {
                jobs = new JobStack(job);
                Jobs.Add(job.ID, jobs);
            }
            Jobs[job.ID].Add(quantity);
            JobsAdded = quantity;
        }
        private void RemoveJobs(JobType job, int quantity, out int JobsRemoved)
        {
            JobsRemoved = 0;
            if (Jobs.TryGetValue(job.ID, out var jobs))
            {
                jobs.Remove(quantity, out int removed);
                //if (jobs.Count == 0) // Breaks other things, so don't do this yet.
                //{
                //    Jobs.Remove(job.ID);
                //}
                JobsRemoved = removed;
            }
        }
        internal void TryFillJobs(JobType job, int quantity, out int workersAdded)
        {
            AssertUtil.ContainsKey(Jobs, job.ID);
            AssertUtil.Positive(quantity);
            var sector = Jobs[job.ID];
            sector.AddWorkers(quantity, out workersAdded);
        }
        internal void TryEmptyJobs(JobType job, int quantity, out int workersRemoved)
        {
            AssertUtil.ContainsKey(Jobs, job.ID);
            AssertUtil.Positive(quantity);
            var sector = Jobs[job.ID];
            sector.RemoveWorkers(quantity, out workersRemoved);
        }

    }
}