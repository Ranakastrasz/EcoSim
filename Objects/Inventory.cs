using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoSim.Objects
{
    // Might be better as an Interface
    // but, a container which lets you add or remove items, or adjust values though Add or Remove.
    public class Inventory
    {
        public Dictionary<string,float> Items {get; private set;} = new();
        // Probably need a capacity limiter too. I.e. fuel tank full.
        // Needs a Canadd, tryAdd.

        public void Add(Labeled<float> item)
        {
            // No safety check here, assume negatives are permitted for now.
            string key = item.Key;
            ;
            if (!Items.TryGetValue(key, out float existingItem))
            {
                Items.Add(key,item);
            }
            else
            {
                // Update the existing item's quantity
                var updatedItem = existingItem + item;
                Items[key] = updatedItem;
            }
        }


        public void Remove(string item)
        {
            // Just flat out delete here.
            Items.Remove(item);
        }

        public bool Contains(string item)
        {
            bool exists = Items.TryGetValue(item,out var existingItem);
            return exists;
        }
        public bool Contains(Labeled<float> item)
        { 
            return (Contains(item.Key));
        }

        public bool CanSpend(Labeled<float> cost) // Needs a list version too.
        { 
            // Asserting
            if (cost.Value < 0) throw new ArgumentOutOfRangeException(nameof(cost),cost,"Costs must not be negative");
            if (cost.Value == 0) return true;
            if (Items.TryGetValue(cost.Key,out var existingItem))
            { 
                if (existingItem < cost) return true;
                return false;
            }
            return true;
        }
        public bool TrySpend (Labeled<float> cost)
        {
            if (CanSpend(cost))
            { 
                Items.TryGetValue(cost.Key,out var existingItem);
                existingItem = existingItem - cost;
                return true;
            }
            return false;
        }

        public Labeled<float> this[string key] // Dunno. might not want this. Works though.
        {
            get => new Labeled<float>(key, Items[key]);
            set => Items[key] = value;
        }

    }
}
