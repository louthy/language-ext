using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace LanguageExt
{
    public static class CollectionFormat
    {
        public static string ToShortString<A>(IEnumerable<A> ma, int length = -1, string separator = ", ")
        {
            var items = ma.Take(50).ToList();

            return items.Count < length || length == -1
                ? $"{String.Join(separator, items)}"
                : $"{String.Join(separator, items)}...";
        }
    }
}
