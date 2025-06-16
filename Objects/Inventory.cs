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
        public Dictionary<string, LabeledValue<int>> Items {get; private set;} = new();


        public void Add(LabeledValue<int> item)
        {
            // No safety check here, assume negatives are permitted for now.
            string label = item.Label;
            var existingItem = Items[label];
            if (existingItem.IsEmpty)
            {
                Items.Add(label,item);
            }
            else
            {
                // Update the existing item's quantity
                var updatedItem = existingItem + item;
                Items[label] = updatedItem;
            }
        }

        public void Remove(string item)
        {
            // Just flat out delete here.
            Items.Remove(item);
        }

        public bool Contains(string item)
        {
            var existingItem = Items.FirstOrDefault(i => i.Label == item);
            if (existingItem.IsEmpty)
            {
                throw new ArgumentException($"Item '{item}' not found in inventory.");
            }
        }
        public bool TrySpend (LabeledValue<int> cost)
        {
            // Negative checks are permitted here.
            // I think the cost should be positive, and subtracted. And the cost is greater than the available quantity, return false.
            if (Contains(cost.Label) && )
            { 
                
            }
            
            return false

        }
    }
}
