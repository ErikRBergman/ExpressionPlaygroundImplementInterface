namespace Serpent.InterfaceProxy.Extensions
{
    using System.Collections.Generic;

    public static class DictionaryExtensions
    {
        public static TValue GetValueOrDefault<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue)
        {
            if (dictionary.TryGetValue(key, out var value))
            {
                return value;
            }

            return defaultValue;
        }

        public static Dictionary<TKey, TValue> Addf<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            dictionary.Add(key, value);
            return dictionary;
        }
    }
}