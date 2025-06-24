using EcoSim.Objects;
using EcoSim.Planets.Definitions;
using System.Drawing;

namespace EcoSim.Planets.Stacks
{
    public class DistrictStack
    {
        public DistrictType DistrictType { get; private set; }

        public string Name => DistrictType.Name;
        public JobType Job => DistrictType.Job;
        public int JobCount => DistrictType.JobCount;
        public Labeled<int> Price => DistrictType.Price;
        public ResourceNodeType? Resource => DistrictType.Resource;

        private int _size;

        public int Size
        {
            get => _size;
            set
                {
                if(value < 0)
                    throw new ArgumentOutOfRangeException(nameof(Size), "Size cannot be negative.");
                _size = value;
            }
        }
        public DistrictStack(DistrictType type, int size = 0)
        {
            DistrictType = type;
            Size = size;
        }
    }
}