namespace Serpent.InterfaceProxy.Extensions
{
    using System.Collections.Generic;
    using System.Linq;

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

        public static Dictionary<TItem, TItem> ZipToDictionary<TItem>(this IEnumerable<TItem> items, IEnumerable<TItem> moreItems)
        {
            var returnItems = items.Zip(moreItems, (a, b) => new KeyValuePair<TItem, TItem>(a, b));
            return returnItems.ToDictionary(p => p.Key, p => p.Value);
        }
    }
}