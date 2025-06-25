using EcoSim.Interfaces.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoSim.Interfaces
{

    internal interface IStackCollection<TStack, TDefinition>: IEnumerable<TStack>
        where TStack : IStack<TDefinition>
        where TDefinition : IDefinitionType
    {
        // Properties
        int Count { get; } // How many unique stack types are in the collection

        // Core Dictionary-like operations
        TStack? GetById(string id); // Retrieves a stack by its definition's ID
        //TStack? GetByName(string name); // Retrieves a stack by its definition's Name

        bool Contains(string id); // Checks if a stack with a given ID exists

        // Operations for managing stack quantities
        // Might add overloads, for passing in stacks or Types directly. Dunno yet.
        void AddQuantity(string id, int quantity); // Adds quantity to an existing stack
        void RemoveQuantity(string id, int quantity); // Removes quantity from an existing stack (handles negative check)
        void SetQuantity(string id, int quantity); // Sets the exact quantity for an existing stack

        // Iteration is inherited from IEnumerable<TStack>
    }
}
