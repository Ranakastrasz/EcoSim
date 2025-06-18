using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EcoSim.Interfaces;

namespace EcoSim.Planet
{
    // Doesn't do anything, just a dummy planet that implements IMarket
    // And can be initilized with a name and some prices.
    internal class DummyPlanet: IMarket
    {
        public string Name {get; private set;} = "";
        public Point Position {get; set;}
        public List<Tuple<string, int>> PriceMap {get; private set;}= new();

        public DummyPlanet(string name, Point position)
        {
            Name = name;
            Position = position;
        }

        public void AddPrice(string resource, int price)
        {
            PriceMap.Add(new Tuple<string, int>(resource, price));
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Planet: {Name} at {Position}");
            sb.AppendLine("Prices:");
            foreach (var price in PriceMap)
            {
                sb.AppendLine($"{price.Item1}: {price.Item2} credits");
            }
            return sb.ToString();
        }
    }
}
