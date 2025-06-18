using EcoSim.Interfaces;
using EcoSim.Objects;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Dynamic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace EcoSim.Ships
{
    internal class Ship: IShip
    {
        public string Name {get; private set;} = "";
        public Point Position {get; private set;}

        int Fuel {get; set;} = 0;
        int credits {get; set;} = 0;

        Dictionary<string, int> CargoHold {get; set;} = new();

        public Ship(string name, Point position, int fuel, int credits)
        {
            Name = name;
            Position = position;
            Fuel = fuel;
            this.credits = credits;
        }

        public void AddCargo(LabeledValue<int> cargo)
        {
            if (CargoHold.ContainsKey(cargo.Label))
            {
                CargoHold[cargo.Label] += cargo.Quantity;
            }
            else
            {
                CargoHold.Add(cargo.Label, cargo.Quantity);
            }
        }
        public void RemoveCargo(LabeledValue<int> cargo)
        {
            if (CargoHold.ContainsKey(cargo.Label))
            {
                CargoHold[cargo.Label] -= cargo.Quantity;
                if (CargoHold[cargo.Label] <= 0)
                {
                    CargoHold.Remove(cargo.Label);
                }
            }
            else
            {
                throw new Exception($"Cargo {cargo.Label} not found in hold.");
            }
        }
        public void Refuel(int amount)
        {
            Fuel += amount;
        }
        public bool TryJump(Point destination)
        { 
            Vector2 jumpVector = new Vector2(destination.X - Position.X, destination.Y - Position.Y);
            int fuelCost = jumpVector.Length() > 0 ? (int)Math.Ceiling(jumpVector.Length()) : 1; // Ensure at least 1 fuel is used for a jump
            if (Fuel >= fuelCost)
            {
                Position = destination;
                Fuel -= fuelCost;
                return true;
            }
            else
            {
                return false;
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Ship Name: {Name}");
            sb.AppendLine($"Fuel: {Fuel}");
            sb.AppendLine($"Credits: {credits}");
            sb.AppendLine("Cargo Hold:");
            foreach (var cargo in CargoHold)
            {
                sb.AppendLine($"  {cargo.Key}: {cargo.Value}");
            }
            return sb.ToString();
        }

        internal bool TryJump(Point position, int v)
        {
            throw new NotImplementedException();
        }
    }
}
