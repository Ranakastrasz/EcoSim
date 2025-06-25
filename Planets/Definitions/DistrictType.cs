using EcoSim.Objects;
using EcoSim.Interfaces.Definitions;

namespace EcoSim.Planets.Definitions
{
    public class DistrictType: IDefinitionType
    {
        public readonly string Name;
        public readonly string ID;
        public readonly string Description = "";
        string IDefinitionType.Name => Name;
        string IDefinitionType.ID => ID;
        string IDefinitionType.Description => Description;

        public readonly JobType Job;  // The job it provides.
        public readonly int JobCount; // How many jobs it provides of that type
                                      // Technically, multiple jobs should be providable.
                                      // A list of KeyValue pairs, job, job count, may be needed later.
        public readonly Labeled<int> Price; // The cost to build
        public readonly ResourceNodeType? Resource; // The resource deposit it is built on. Optional.

        public DistrictType(string name, JobType job, int jobCount, Labeled<int> price, ResourceNodeType? resource = null)
        {
            Name = name;
            ID = name.ToLower();
            Job = job;
            JobCount = jobCount;
            Price = price;
            Price = price;
            Resource = resource;
        }
    }
}