using EcoSim.Interfaces;
using EcoSim.Objects;
using EcoSim.Planets.Definitions;
using System.Drawing;

namespace EcoSim.Planets.Stacks
{
    public class DistrictStack: AbstractStack<DistrictType>
    {
        public DistrictType District { get => BaseType; protected set => BaseType = value; }

        public DistrictStack(DistrictType district, int count = 0) : base(district)
        {
            BaseType = district;
            Count = count;
        }
    }
}