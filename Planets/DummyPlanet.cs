using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EcoSim.Items;
using EcoSim.Objects;

namespace EcoSim.Planet
{
    // Doesn't do anything, just a dummy planet that implements IMarket
    // And can be initilized with a name and some prices.
    // Meant for Ship Testing.
    internal class DummyPlanet: IMarket
    {
        public string Name {get; private set;} = "";
        public Point Position {get; set;}
        public List<Labeled<int>> PriceMap {get; private set;}= new();

        public DummyPlanet(string name, Point position)
        {
            Name = name;
            Position = position;
        }

        public void AddPrice(string resource, int price)
        {
            PriceMap.Add(new Labeled<int>(resource, price));
        }

        public override string ToString()
        {
            StringBuilder oString = new StringBuilder();
            Draw(oString);
            DrawMarket(oString);
            return oString.ToString();
        }

        internal void Draw(StringBuilder oString)
        {
            oString.AppendLine($"Planet: {Name} at {Position}");
        }

        internal void DrawMarket(StringBuilder oString)
        {
            oString.AppendLine("Prices:");
            foreach (var price in PriceMap)
            {
                oString.AppendLine($"{price.Key}: {price.Value} credits");
            }
        }
    }
}
