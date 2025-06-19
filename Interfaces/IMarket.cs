using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EcoSim.Objects;

// Expose the name, and resources+Prices of a market
namespace EcoSim.Interfaces
{
    internal interface IMarket
    {
        string Name {get; }
        List<LabeledValue<int>> PriceMap {get;}
    }
}
