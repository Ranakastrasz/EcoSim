using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoSim.Interfaces
{
    public interface IStackable
    {
        int StackSize { get => int.MaxValue; } // Stack size per slot, essentially.
        float StackSizeF { get => float.PositiveInfinity; }
    }
}
