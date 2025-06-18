using EcoSim.Objects;

namespace EcoSim.Planet
{
    public struct Job
    {
        public string Name { get; private set; }
        public LabeledValue<int> Yield { get; private set; } // Will be a dictionary, and include the upkeep as well later.
        public LabeledValue<int>? Upkeep { get; private set; }
        public Job(string name, LabeledValue<int> yield, LabeledValue<int>? upkeep = null)
        {
            Name = name;
            Yield = yield;
            Upkeep = upkeep;
        }
        // Kinda want a method to try and run the job, but this is a struct.
        // Its the JobSector that will manage the workers and resources.
        // Once created, this isn't supposed to be changed.
    }
}