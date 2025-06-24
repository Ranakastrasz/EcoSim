
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoSim.Extensions
{
    public static class DictionaryExtensions
    {
        public static TValue ForceGet<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, Func<TValue> creator)
        {
            if(!dict.TryGetValue(key, out var value))
            {
                value = creator();
                dict[key] = value;
            }
            return value;
        }
    }
}
