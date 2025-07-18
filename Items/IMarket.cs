﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EcoSim.Objects;

// Expose the name, and resources+Prices of a market
namespace EcoSim.Items
{
    internal interface IMarket
    {
        string Name {get; }
        List<Labeled<int>> PriceMap {get;}
    }
}
