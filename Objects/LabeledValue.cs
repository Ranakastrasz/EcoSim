using System;
using System.Numerics; // Required for INumber<T> (C# 11+)

namespace EcoSim.Objects
{
    // LabeledValue<T> where T is a numeric type (int, float, double, etc.)
    // Requires C# 11 for INumber<T> generic math.
    public readonly struct LabeledValue<T> where T : INumber<T>, IComparable<T>, IEquatable<T>
    {
        public string Label { get; }
        public T Value { get; } // Negatives permitted, i.e., costs or debts.

        public LabeledValue(string label, T quantity)
        {
            if (string.IsNullOrWhiteSpace(label))
                throw new ArgumentException("Label cannot be null or whitespace.", nameof(label));
            Label = label;
            Value = quantity;
        }

        public LabeledValue<T> Abs() => new LabeledValue<T>(Label, T.Abs(Value));

        public LabeledValue<T> WithQuantity(T newQuantity)
        {
            return new LabeledValue<T>(Label, newQuantity);
        }

        public bool IsEmpty => Value == T.Zero;
        public bool IsPositive => Value > T.Zero;
        public bool IsNegative => Value < T.Zero;

        // --- Conversion Operators ---

        // Implicit conversion from LabeledValue<T> to T (extracts the raw quantity)
        public static implicit operator T(LabeledValue<T> labeledValue) => labeledValue.Value;

        // Explicit conversion from T to LabeledValue<T> (requires an explicit cast because Label is lost)
        // This is tricky as the Label is lost. Consider if this makes sense for your domain.
        // If used, you'd likely need a "default" label. Omitted for now due to potential ambiguity.
        // public static explicit operator LabeledValue<T>(T quantity) => new LabeledValue<T>("Default", quantity);


        // --- Unary Operators ---

        public static LabeledValue<T> operator -(LabeledValue<T> a)
        {
            return new LabeledValue<T>(a.Label, -a.Value);
        }

        // --- Binary Operators (LabeledValue<T> vs T) ---

        // Add LabeledValue<T> and T (returns LabeledValue<T>)
        public static LabeledValue<T> operator +(LabeledValue<T> a, T quantity)
        {
            return new LabeledValue<T>(a.Label, a.Value + quantity);
        }
        public static LabeledValue<T> operator +(T quantity, LabeledValue<T> a)
        {
            return new LabeledValue<T>(a.Label, quantity + a.Value);
        }

        // Subtract LabeledValue<T> and T (returns LabeledValue<T>)
        public static LabeledValue<T> operator -(LabeledValue<T> a, T quantity)
        {
            return new LabeledValue<T>(a.Label, a.Value - quantity);
        }
        public static LabeledValue<T> operator -(T quantity, LabeledValue<T> a)
        {
            return new LabeledValue<T>(a.Label, quantity - a.Value);
        }

        // Multiply LabeledValue<T> and T (returns LabeledValue<T>)
        public static LabeledValue<T> operator *(LabeledValue<T> a, T multiplier)
        {
            return new LabeledValue<T>(a.Label, a.Value * multiplier);
        }
        public static LabeledValue<T> operator *(T multiplier, LabeledValue<T> a)
        {
            return new LabeledValue<T>(a.Label, multiplier * a.Value);
        }

        // Divide LabeledValue<T> and T (returns LabeledValue<T>)
        public static LabeledValue<T> operator /(LabeledValue<T> a, T divisor)
        {
            if (divisor == T.Zero)
                throw new DivideByZeroException();
            return new LabeledValue<T>(a.Label, a.Value / divisor);
        }

        // --- Binary Operators (LabeledValue<T> vs LabeledValue<T>) ---

        // Add two LabeledValue<T> (requires matching labels, returns LabeledValue<T>)
        public static LabeledValue<T> operator +(LabeledValue<T> a, LabeledValue<T> b)
        {
            if (a.Label != b.Label)
                throw new InvalidOperationException($"Cannot add labeled values with different labels: '{a.Label}' and '{b.Label}'.");
            return a + b.Value; // Uses the LabeledValue<T> + T operator
        }

        // Subtract two LabeledValue<T> (requires matching labels, returns LabeledValue<T>)
        public static LabeledValue<T> operator -(LabeledValue<T> a, LabeledValue<T> b)
        {
            if (a.Label != b.Label)
                throw new InvalidOperationException($"Cannot subtract labeled values with different labels: '{a.Label}' and '{b.Label}'.");
            return new LabeledValue<T>(a.Label, a.Value - b.Value);
        }

        // --- Equality Comparison ---

        // Equality checks for both label and quantity
        public static bool operator ==(LabeledValue<T> a, LabeledValue<T> b)
        {
            return a.Label == b.Label && a.Value.Equals(b.Value);
        }
        public static bool operator !=(LabeledValue<T> a, LabeledValue<T> b)
        {
            return !(a == b);
        }

        // --- Explicit Quantity Comparison (Removed implicit > < >= <= for LabeledValue<T>) ---
        // To compare quantities, explicitly cast to T: (int)itemA > (int)itemB

        // --- Object Overrides ---

        public override string ToString()
        {
            return $"{Label}: {Value}";
        }

        public override bool Equals(object? obj)
        {
            if (obj is LabeledValue<T> other)
            {
                return Label == other.Label && Value.Equals(other.Value);
            }
            return false;
        }

        public override int GetHashCode() => HashCode.Combine(Label, Value);

        public LabeledValue<T> WithValue(T value)
        {
            return new LabeledValue<T>(Label, value);
        }

    }
}