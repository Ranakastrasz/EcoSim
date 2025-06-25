using AssertUtils;
using EcoSim.Interfaces;
using EcoSim.Interfaces.Definitions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoSim.Objects
{
    public abstract class AbstractStack<TType>: IStack<TType>
        where TType : IDefinitionType, IStackable
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
                    throw new ArgumentOutOfRangeException(nameof(AbstractStack<TType>), "Stack Count cannot be negative.");
                _count = value;
            }
        }

        // Common property implementation
        public string Name => BaseType.Name;
        public string ID => BaseType.ID;

        // Constructor for the base class
        protected AbstractStack(TType baseType, int initialSize = 0)
        {
            BaseType = baseType;
            Count = initialSize;
        }

        /// <summary>
        /// Add exactly this much. Return success.
        /// </summary>
        /// <param name="amount"></param>
        public bool Add(int amount)
        {
            AssertUtil.Positive(amount);
            Count += amount;
            return true; // Currently, no stack size checks.
        }
        public virtual void ForceAdd(int amount, out int added)
        {
            AssertUtil.NotPositive(amount);
            amount = Math.Min(amount, BaseType.StackSize-Count);
            Count += amount;
            added = amount;
            AssertUtil.Less(Count,BaseType.StackSize);
        }

        // Another common method
        public virtual void Remove(int amount, out int removed)
        {
            if(amount < 0)
                throw new ArgumentOutOfRangeException(nameof(amount), "Amount to remove cannot be negative.");
            if(Count < amount)
            {
                throw new InvalidOperationException($"Cannot remove {amount} from {Name} stack. Only {Count} available.");
            }
            Count -= amount;
            removed = amount;
        }

        // Add any other shared methods or properties here
    }
}
