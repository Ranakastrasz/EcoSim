using System;
using System.Numerics; // Required for INumber<T> (C# 11+)

namespace EcoSim.Objects
{
    /// <summary>
    /// Kinda a wrapper for a KeyValue Pair.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public readonly struct Labeled<T> where T : INumber<T>
    {
        private readonly KeyValuePair<String,T> _keyValuePair;
        public string Key { get => _keyValuePair.Key;}
        public string Label => Key;
        public T Value { get => _keyValuePair.Value; }

        public Labeled(string key, T value)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Key cannot be null or whitespace.", nameof(key));
            _keyValuePair = new KeyValuePair<String,T>(key, value);
        }

        public Labeled<T> WithValue(T newValue) => new(Key, newValue);
        public Labeled<T> Abs() => new(Key, T.Abs(Value));
        public bool IsZero => Value == T.Zero;

        public static implicit operator T(Labeled<T> labeled) => labeled.Value;
        public static implicit operator KeyValuePair<string, T>(Labeled<T> labeled) => new(labeled.Key, labeled.Value);
        public static implicit operator Labeled<T>(KeyValuePair<string, T> kvp) => new(kvp.Key, kvp.Value);

        public static Labeled<T> operator +(Labeled<T> a, T b) => new(a.Key, a.Value + b);
        public static Labeled<T> operator -(Labeled<T> a, T b) => new(a.Key, a.Value - b);
        public static Labeled<T> operator *(Labeled<T> a, T b) => new(a.Key, a.Value * b);
        public static Labeled<T> operator /(Labeled<T> a, T b) => new(a.Key, a.Value / b);

        public static Labeled<T> operator -(Labeled<T> a) => new(a.Key, -a.Value);

        public override string ToString() => $"{Key}: {Value}";
    }   
}