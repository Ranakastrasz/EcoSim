using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Not sure what this will exactly do yet. Probably how ships can interact with each other uses this tho.
namespace EcoSim.Ships.Interfaces
{
    internal interface IShip
    {
        string Name {get; }
        Point Position {get; }
    }
}
