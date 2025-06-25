using EcoSim.Interfaces;
using EcoSim.Interfaces.Definitions;

namespace EcoSim.Planets.Definitions
{
    internal class ItemType: IDefinitionType, IStackable
    {
        public string Name { get; }

        public string ID { get; }

        public string Description => "";

        public int StackSize { get; } = int.MaxValue;
        public float StackSizeF { get; } = float.PositiveInfinity;


        public ItemType(string name, string id = "", int stackSize = int.MaxValue)
        { 
            Name = name;
            ID = id == null ? name.ToLower() : id;
            StackSize = stackSize;
            StackSizeF = stackSize;
        }

    }
}
