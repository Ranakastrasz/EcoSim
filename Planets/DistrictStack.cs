using EcoSim.Objects;
using EcoSim.Planets.Definitions;

namespace EcoSim.Planet
{
    public class DistrictStack
    {
        public string Name { get; private set; }
        public NaturalResource? Resource { get; private set; }
        private int _size;
        private Labeled<int> _price;

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
        public Labeled<int> Price
        {
            get => _price;
            set
            {
                if(value.Value < 0)
                    throw new ArgumentOutOfRangeException(nameof(Size), "Price cannot be negative.");
                _price = value;
            }
        }
        public Jobtype Job { get; set; } // The job that this provides.
        public DistrictStack(string name, Jobtype job, Labeled<int> price, NaturalResource? resource = null)
        {
            Size = 0;
            Name = name;
            Job = job;
            Price = price;
            Resource = resource;
        }
    }
}