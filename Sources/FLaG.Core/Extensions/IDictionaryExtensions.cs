namespace FLaG.Core.Extensions
{
    public static class IDictionaryExtensions
    {
        public static ILookup<TKey, TValue> ToLookup<TKey, TValue, T>(this IDictionary<TKey, T> obj)
            where T : IEnumerable<TValue>
        {
            return obj.SelectMany(p =>
                    p.Value.Select(value => new KeyValuePair<TKey, TValue>(p.Key, value))
                )
                .ToLookup(pair => pair.Key, pair => pair.Value);
        }

        public static TValue GetOrAdd<TKey, TValue>(
            this IDictionary<TKey, TValue> obj,
            TKey key,
            Func<TValue> valueFactory
        )
        {
            if (obj.TryGetValue(key, out TValue? storedValue))
            {
                return storedValue;
            }

            TValue value = valueFactory();
            obj.Add(key, value);
            return value;
        }
    }
}
