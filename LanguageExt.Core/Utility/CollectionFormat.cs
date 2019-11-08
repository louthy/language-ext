using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace LanguageExt
{
    public static class CollectionFormat
    {
        public static string ToShortString<A>(IEnumerable<A> ma, string separator = ", ")
        {
            var items = ma.Take(50).ToList();

            if(items.Count < 50)
            {
                return $"{String.Join(separator, items)}";
            }
            else
            {
                items.RemoveAt(49);
                return $"{String.Join(separator, items)}...";
            }
        }
    }
}
