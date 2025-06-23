using AssertUtils;
using EcoSim.Objects;
using EcoSim.Planets.Definitions;
using System.Drawing;
using System.Xml;

namespace EcoSim.Planet
{
    public class JobStack
    {
        private int _workers; // How many workers are currently assigned to this job
        private int _jobs;
        public Jobtype Job { get; private set; }
        public int Jobs
        {
            get => _jobs;
            set
            {
                AssertUtil.AssertArgumentNotNegative(value);
                _jobs = value;
                if(_workers > _jobs)
                    _workers = _jobs;
            }
        }
        public int Workers
        {
            get => _workers;
            set
            {
                AssertUtil.AssertArgumentNotNegative(value);
                AssertUtil.AssertArgumentNotGreater(value,_jobs);
                _workers = value;
            }
        }

        public JobStack(Jobtype job)
        {
            Job = job;
            Jobs = 0;
            Workers = 0;
        }
        
        // Update, Tells the workers to do the job, and sends the resource changes to the passed in inventory
        internal void Update(Inventory inventory)
        { 
            int jobsToWork = Workers;
            // If there are no workers, skip this sector
            if(jobsToWork == 0)
                return;
            // If the job requires resources, check if we have enough
            if(Job.Inputs.Count > 0)
            {
                // Put a loop here. Later.
                foreach(var input in Job.Inputs)
                {

                    // Check if we have enough resources to pay the upkeep
                    float stockpileQuantity = inventory[input.Key].Value;
                    // See how many jobs we have resources for
                    jobsToWork = (int)Math.Min(jobsToWork, stockpileQuantity / input.Value);


                }
                if(jobsToWork != 0)
                {
                    foreach(var input in Job.Inputs)
                    {
                        if(jobsToWork > 0)
                        {
                            inventory.TrySpend(input * jobsToWork); // Technically, doesn't need to try at this point.
                                                                     // But, Until I can pass an entire List of LabeledValues in, as a delta...
                        }
                    }
                }
            }
            if(jobsToWork == 0)
                return;
            foreach(var output in Job.Outputs)
            {
                var ToAdd = output * jobsToWork;
                inventory.Add(ToAdd);
            }
        }

        public void AddWorkers(int workers, out int workersAdded)
        {
            int newWorkers = Workers + workers;
            newWorkers = int.Clamp(newWorkers, 0, Jobs);

            workersAdded = newWorkers - Workers;
            
            Workers = newWorkers;
        }
        public void RemoveWorkers(int workers, out int workersRemoved)
        {
            AddWorkers(-workers, out int workersAdded);
            workersRemoved = -workersAdded;
        }
        
        public void AddJobs(int jobs, out int jobsAdded)
        {
            int newJobs = Jobs + jobs;
            newJobs = int.Max(newJobs, 0);

            jobsAdded = newJobs - Jobs;

            Jobs = newJobs;
        }
        public void RemoveJobs(int jobs, out int jobsRemoved)
        {
            AddJobs(-jobs, out int jobsAdded);
            jobsRemoved = -jobsAdded;
        }
        public void RemoveAllWorkers() => RemoveWorkers(Workers, out int workersRemoved);

    }
}