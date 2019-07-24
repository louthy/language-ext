using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageExt.Parsec
{
    public static class StringAndCollectionExtensions
    {
        public static PString ToPString(this string value) =>
            PString.Zero.SetValue(value);

        public static PString<T> ToPString<T>(this IEnumerable<T> value) =>
            PString<T>.Zero.SetValue(value.ToArray());
    }
}
