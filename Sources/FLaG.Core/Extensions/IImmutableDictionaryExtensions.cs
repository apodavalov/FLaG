using System.Collections.Immutable;

namespace FLaG.Core.Extensions
{
    public static class IImmutableDictionaryExtensions
    {
        public static TValue ValueOrThrow<TKey, TValue>(
            this IImmutableDictionary<TKey, TValue> obj,
            TKey key
        )
        {
            if (obj.TryGetValue(key, out TValue? value))
            {
                return value;
            }

            throw new InvalidOperationException(
                "The expected key doesn't exist in the dictionary."
            );
        }
    }
}
