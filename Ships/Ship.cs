﻿using EcoSim.Objects;
using EcoSim.Ships.Interfaces;
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
    public class OutOfFuel : Exception{ }
    public class InvalidJump : Exception{ }
    public class InsufficientCredits : Exception{ }
    public class InsufficientCargo : Exception{ }
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

        public void TryBuyCargo(Labeled<int> cargo, int pricePerUnit)
        {
            int totalCost = cargo.Value * pricePerUnit;
            if (credits >= totalCost)
            {
                credits -= totalCost;
                AddCargo(cargo);
            }
            else
            {
                throw new InsufficientCredits();
            }
        }
        public void TrySellCargo(Labeled<int> cargo, int pricePerUnit)
        {
            if (CargoHold.ContainsKey(cargo.Key) && CargoHold[cargo.Key] >= cargo.Value)
            {
                CargoHold[cargo.Key] -= cargo.Value;
                if (CargoHold[cargo.Key] == 0)
                {
                    CargoHold.Remove(cargo.Key);
                }
                int totalRevenue = cargo.Value * pricePerUnit;
                credits += totalRevenue;
            }
            else
            {
                throw new InsufficientCargo();
            }
        }

        public void AddCargo(Labeled<int> cargo)
        {
            if (CargoHold.ContainsKey(cargo.Key))
            {
                CargoHold[cargo.Key] += cargo.Value;
            }
            else
            {
                CargoHold.Add(cargo.Key, cargo.Value);
            }
        }
        public void RemoveCargo(Labeled<int> cargo)
        {
            if (CargoHold.ContainsKey(cargo.Key))
            {
                CargoHold[cargo.Key] -= cargo.Value;
                if (CargoHold[cargo.Key] <= 0)
                {
                    CargoHold.Remove(cargo.Key);
                    return;
                }
            }
            throw new InsufficientCargo(); // Either no cargo of this type, or not enough. Either way, same issue, same answer.
        }
        public void TrySpend(int cost)
        {
            if(credits >= cost)
            {
                credits -= cost;
            }
            else
            { 
                throw new InsufficientCredits();
            }
        }

        public void TryJump(Point destination)
        { 
            if (destination == Position)
                throw new InvalidJump();
            Vector2 jumpVector = new Vector2(destination.X - Position.X, destination.Y - Position.Y);
            int fuelCost = (int)Math.Ceiling(jumpVector.Length()); // Fuel cost is the Euclidean distance rounded up
            if (Fuel >= fuelCost)
            {
                Position = destination;
                Fuel -= fuelCost;
                return;
            }
            else
            {
                throw new OutOfFuel();
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

        public void Draw(StringBuilder oString)
        {
            oString.AppendLine($"Ship Name: {Name}");
            oString.AppendLine($"Position: {Position.X}:{Position.Y}");
            oString.AppendLine($"Fuel: {Fuel}");
            oString.AppendLine($"Credits: {credits}");
        }

        public void DrawCargo(StringBuilder oString)
        {
            oString.AppendLine("Cargo Hold:");
            foreach (var cargo in CargoHold)
            {
                oString.AppendLine($"  {cargo.Key}: {cargo.Value}");
            }
        }
    }
}
