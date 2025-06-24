using AssertUtils;
using EcoSim.Interfaces.Definitions;
using EcoSim.Objects;

namespace EcoSim.Planets.Definitions
{
    public class JobType: IDefinitionType
    {
        // Probably should impliment ItemDelta, instead of handling it here. When ItemDelta exists anyway.
        public string Name { get; }
        public string ID { get; }
        string IDefinitionType.Name => Name;
        string IDefinitionType.ID => ID;
        public IReadOnlyList<Labeled<float>> Inputs { get; } // Required resources to spend to run this job.
        public IReadOnlyList<Labeled<float>> Outputs { get; } // Resources produced when the job is run, assuming Input is met.
        

        public JobType(string name, IReadOnlyList<Labeled<float>> outputs, IReadOnlyList<Labeled<float>>? inputs = null)
        {
            Name = name;
            ID = name.ToLower();
            Outputs = outputs.ToList().AsReadOnly(); // Safe, nonmutable copy.
            Inputs = inputs?.ToList().AsReadOnly() ?? new List<Labeled<float>>().AsReadOnly();

            foreach(var output in Outputs)
                AssertUtil.AssertArgumentNotNegative(output.Value);

            foreach(var input in Inputs)
                AssertUtil.AssertArgumentNotNegative(input.Value);
            // Could, I suppose, allow an array which is then sorted, and 0 values are simply ignored,
            // So I can have both inputs and outputs in one batch
            // But this is cleaner.
        }

        public bool CanRun(Inventory inventory)
        {
            foreach (var input in Inputs)
            {
                if (inventory.CanSpend(input))
                    return false;
            }
            return true;
        }

        public void Run(Inventory inventory) // May need a times to run, int or float, or "Run as many up to" or so on. 
        {
            // Will be in inventory when multispend/check exists. Pass in ItemDelta thing.
            if (!CanRun(inventory)) throw new InvalidOperationException("Not enough resources"); 

            foreach (var input in Inputs)
                inventory.Add(-input);

            foreach (var output in Outputs)
                inventory.Add(output);
        }
    }
}