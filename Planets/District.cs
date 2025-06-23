using EcoSim.Objects;

namespace EcoSim.Planet
{
    public class District
    {
        public string Name { get; private set; }
        public NaturalResource? Resource { get; private set; }
        private int _size;
        private LabeledValue<int> _price;

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
        public LabeledValue<int> Price
        {
            get => _price;
            set
            {
                if(value.Value < 0)
                    throw new ArgumentOutOfRangeException(nameof(Size), "Price cannot be negative.");
                _price = value;
            }
        }
        public Job Job { get; set; } // The job that this provides.
        public District(string name, Job job, LabeledValue<int> price, NaturalResource? resource = null)
        {
            Size = 0;
            Name = name;
            Job = job;
            Price = price;
            Resource = resource;
        }
    }
}