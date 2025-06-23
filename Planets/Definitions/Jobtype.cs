using AssertUtils;
using EcoSim.Objects;

namespace EcoSim.Planets.Definitions
{
    public class Jobtype
    {
        // Probably should impliment ItemDelta, instead of handling it here. When ItemDelta exists anyway.
        public string Name { get; }
        public string id { get; }
        public IReadOnlyList<Labeled<float>> Outputs { get; } // Resources produced when the job is run, assuming Input is met.
        public IReadOnlyList<Labeled<float>> Inputs { get; } // Required resources to spend to run this job.
        public Jobtype(string name, IReadOnlyList<Labeled<float>> outputs, IReadOnlyList<Labeled<float>>? inputs = null)
        {
            Name = name;
            id = name.ToLower();
            Outputs = outputs.ToList().AsReadOnly(); // Safe, nonmutable copy.
            Inputs = inputs?.ToList().AsReadOnly() ?? new List<Labeled<float>>().AsReadOnly();

            foreach(var output in Outputs)
                AssertUtil.AssertArgumentNotNegative(output.Value);

            foreach(var input in Inputs)
                AssertUtil.AssertArgumentNotNegative(input.Value);
            // Could, I suppose, allow an array which is then sorted, and 0 values are simply ignored. but eh.
        }
        //public Job(string name, Labeled<float> output, IReadOnlyList<Labeled<float>>? inputs = null)
        //    : this(name, new[]{output}, inputs){ }
        //public Job(string name, IReadOnlyList<Labeled<float>> outputs, Labeled<float> input)
        //    : this(name, outputs, new[]{input }){ }

        public bool CanRun(Inventory inventory)
        {
            foreach (var input in Inputs)
            {
                if (inventory.CanSpend(input))
                    return false;
            }
            return true;
        }

        public void Run(Inventory inventory)
        {
            // Will be in inventory when multispend/check exists. Pass in ItemDelta thing.
            if (!CanRun(inventory)) throw new InvalidOperationException("Not enough resources"); 

            foreach (var input in Inputs)
                inventory.Add(-input);

            foreach (var output in Outputs)
                inventory.Add(output);
        }
    }
    //public static class JobFactory
    //{
    //    public static Job Create(string name, Labeled<float> output, Labeled<float>? input = null) =>
    //        new Job(name, new[] { output }, input != null ? new[] { input } : null);
    //
    //    public static Job Create(string name, IEnumerable<Labeled<float>> outputs, Labeled<float>? input = null) =>
    //        new Job(name, outputs.ToList(), input != null ? new[] { input } : null);
    //
    //    public static Job Create(string name, Labeled<float> output, IEnumerable<Labeled<float>>? inputs) =>
    //        new Job(name, new[] { output }, inputs?.ToList());
    //
    //    public static Job Create(string name, IEnumerable<Labeled<float>> outputs, IEnumerable<Labeled<float>>? inputs) =>
    //        new Job(name, outputs.ToList(), inputs?.ToList());
    //}
}