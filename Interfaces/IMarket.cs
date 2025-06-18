using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Expose the name, and resources+Prices of a market
namespace EcoSim.Interfaces
{
    internal interface IMarket
    {
        string Name {get; }
        List<Tuple<string, int>> PriceMap {get;}
    }
}
