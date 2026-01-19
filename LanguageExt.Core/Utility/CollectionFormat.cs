using System;
using System.Linq;
using System.Collections.Generic;

namespace LanguageExt
{
    public static class CollectionFormat
    {
        /// <summary>
        /// Application wide setting for the maximum number of items 
        /// shown in a call to the `ToString` method of any LanguageExt 
        /// collection type.
        /// </summary>
        public static int MaxShortItems = 50;

        /// <summary>
        /// Turn an enumerable into a string in the format `a, b, c, ...`
        /// </summary>
        public static string ToShortString<A>(IEnumerable<A> ma, string separator = ", ")
        {
            var items = ma.Take(MaxShortItems).ToList();

            return items.Count < MaxShortItems
                ? $"{string.Join(separator, items)}"
                : $"{string.Join(separator, items)} ...";
        }

        /// <summary>
        /// Turn an enumerable into a string in the format `a, b, c, ...`
        /// </summary>
        public static string ToShortString<A>(IEnumerable<A> ma, long count, string separator = ", ") =>
            count <= MaxShortItems
                ? $"{string.Join(separator, ma)}"
                : $"{string.Join(separator, ma.Take(MaxShortItems))} ... {count - MaxShortItems} more";

        /// <summary>
        /// Turn an enumerable into a string in the format `[a, b, c, ...]`
        /// </summary>
        public static string ToShortArrayString<A>(IEnumerable<A> ma, string separator = ", ") =>
            $"[{ToShortString(ma, separator)}]";

        /// <summary>
        /// Turn an enumerable into a string in the format `[a, b, c, ...]`
        /// </summary>
        public static string ToShortArrayString<A>(IEnumerable<A> ma, long count, string separator = ", ") =>
            $"[{ToShortString(ma, count, separator)}]";

        /// <summary>
        /// Turn an enumerable into a string in the format `a, b, c, ...`
        /// </summary>
        public static string ToFullString<A>(IEnumerable<A> ma, string separator = ", ") =>
            $"{string.Join(separator, ma)}";

        /// <summary>
        /// Turn an enumerable into a string in the format `[a, b, c, ...]`
        /// </summary>
        public static string ToFullArrayString<A>(IEnumerable<A> ma, string separator = ", ") =>
            $"[{ToFullString(ma, separator)}]";
    }
}
