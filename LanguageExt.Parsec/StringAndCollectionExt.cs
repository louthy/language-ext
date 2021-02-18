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

        public static PString<T> ToPString<T>(this IEnumerable<T> value, Func<T, Pos> tokenPos) =>
            PString<T>.Zero(tokenPos).SetValue(value.ToArray());

        public static PString<T> ToPString<T>(this Seq<T> value, Func<T, Pos> tokenPos) =>
            PString<T>.Zero(tokenPos).SetValue(value.ToArray());
    }
}
