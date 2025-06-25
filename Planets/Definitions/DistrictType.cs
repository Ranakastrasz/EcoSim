using EcoSim.Objects;
using EcoSim.Interfaces.Definitions;
using EcoSim.Interfaces;
using EcoSim.Utils;
using AssertUtils;

namespace EcoSim.Planets.Definitions
{
    public class DistrictType: IDefinitionType, IStackable
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

        public DistrictType(string id, string name, JobType job, int jobCount, Labeled<int> price, ResourceNodeType? resource = null)
        {
            Name = name;
            ID = id.ToLower().RemoveWhitespace();
            AssertUtil.Equal(ID, id); // Ensure ID is lowercase and whitespace removed.

            Job = job;
            JobCount = jobCount;
            Price = price;
            Resource = resource;
        }
    }
}