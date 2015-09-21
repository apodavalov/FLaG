using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace FLaGLib.Extensions
{
    public static class IDictionaryExtensions
    {
        public static IReadOnlyDictionary<K,V> AsReadOnly<K,V>(this IDictionary<K, V> obj)
        {
            return new ReadOnlyDictionary<K, V>(obj);
        }

        public static SortedDictionary<K, V> ToSortedDictionary<K, V>(this IDictionary<K, V> obj)
        {
            return new SortedDictionary<K, V>(obj);
        }

        public static Dictionary<K,V> ToDictionary<K,V>(this IDictionary<K,V> obj)
        {   
            return new Dictionary<K, V>(obj);
        }

        public static TValue ValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> obj, TKey key, TValue defaultValue)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            return obj.ContainsKey(key) ? obj[key] : defaultValue;
        }

        public static ILookup<TKey, TValue> ToLookup<TKey, TValue, T>(this IDictionary<TKey, T> obj) where T : IEnumerable<TValue> 
        {
            return obj.SelectMany(p => p.Value.Select(value => new KeyValuePair<TKey,TValue>(p.Key, value))).ToLookup(pair => pair.Key, pair => pair.Value);
        }
    }
}
