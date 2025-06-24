using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EcoSim.Interfaces.Definitions;

namespace EcoSim.Interfaces
{
    internal interface IStack<TType> where TType : IDefinitionType
    {
        TType BaseType { get; }
        int Count { get; set; }
    }
}
