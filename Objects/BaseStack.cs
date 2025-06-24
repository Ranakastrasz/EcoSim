using EcoSim.Interfaces;
using EcoSim.Interfaces.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoSim.Objects
{
    public abstract class BaseStack<TType>: IStack<TType> where TType : IDefinitionType
    {
        // Protected set allows derived classes to set it, but not outside consumers
        public TType BaseType { get; protected set; }

        private int _count;
        public int Count
        {
            get => _count;
            set
            {
                if(value < 0)
                    throw new ArgumentOutOfRangeException(nameof(BaseStack<TType>), "Stack Count cannot be negative.");
                _count = value;
            }
        }

        // Common property implementation
        public string Name => BaseType.Name;
        public string ID => BaseType.ID;

        // Constructor for the base class
        protected BaseStack(TType baseType, int initialSize = 0)
        {
            BaseType = baseType;
            Count = initialSize;
        }

        public virtual void Add(int amount)
        {
            if(amount < 0)
                throw new ArgumentOutOfRangeException(nameof(amount), "Amount to add cannot be negative.");
            Count += amount;
        }

        // Another common method
        public virtual void Remove(int amount)
        {
            if(amount < 0)
                throw new ArgumentOutOfRangeException(nameof(amount), "Amount to remove cannot be negative.");
            if(Count < amount)
            {
                throw new InvalidOperationException($"Cannot remove {amount} from {Name} stack. Only {Count} available.");
            }
            Count -= amount;
        }

        // Add any other shared methods or properties here
    }
}
