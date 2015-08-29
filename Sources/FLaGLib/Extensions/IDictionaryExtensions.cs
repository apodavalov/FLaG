using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FLaGLib.Extensions
{
    public static class IDictionaryExtensions
    {
        public static TValue ValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> obj, TKey key, TValue defaultValue)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            return obj.ContainsKey(key) ? obj[key] : defaultValue;
        }

        public static ILookup<TKey, TValue> ToLookup<TKey, TValue, T>(this IDictionary<TKey, T> obj) where T : IEnumerable<TValue> 
        {
            return obj.SelectMany(p => p.Value.Select(value => new KeyValuePair<TKey,TValue>(p.Key, value))).ToLookup(pair => pair.Key, pair => pair.Value);
        }
    }
}
