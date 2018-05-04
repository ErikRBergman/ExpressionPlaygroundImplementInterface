namespace Serpent.InterfaceProxy.Extensions
{
    using System.Collections.Generic;
    using System.Linq;

    public static class EnumerableExtensions
    {
        public static IEnumerable<T> Append<T>(this IEnumerable<T> items, T itemToAppend)
        {
            return items.Union(itemToAppend.ToEnumerable());
        }

        public static IEnumerable<T> Prepend<T>(this IEnumerable<T> items, T itemToAppend)
        {
            return itemToAppend.ToEnumerable().Union(items);
        }

        public static IEnumerable<T> ToEnumerable<T>(this T item)
        {
            yield return item;
        }
    }
}