using EcoSim.Objects;
using EcoSim.Planet;

namespace EcoSim.Planets.Definitions
{
    public class DistrictType
    {
        public readonly Jobtype Job; // The job it provides.
        public readonly string Name; // 
        public readonly Labeled<int> Price;
        public readonly NaturalResource? Resource;

        public DistrictType(Jobtype job, string name, Labeled<int> price, NaturalResource? resource)
        {
            Price = price;
            Job = job;
            Name = name;
            Price = price;
            Resource = resource;
        }
    }
}