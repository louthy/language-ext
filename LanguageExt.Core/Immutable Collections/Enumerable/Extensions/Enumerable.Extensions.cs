#pragma warning disable CS0693 // Type parameter has the same name as the type parameter from outer type

using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Text;

namespace LanguageExt;

public static partial class EnumerableExtensions
{
    /// <param name="ma">Enumerable to inject values into</param>
    /// <typeparam name="A">Bound type</typeparam>
    extension<A>(IEnumerable<A> ma)
    {
        /// <summary>
        /// Force evaluation of the enumerable
        /// </summary>
        public Unit Consume() =>
            Iterable.consume(ma);
    }

    /// <summary>
    /// Concatenate all strings into one
    /// </summary>
    [Pure]
    public static string Concat(this IEnumerable<string> xs)
    {
        var sb = new StringBuilder();
        foreach (var x in xs)
        {
            sb.Append(x);
        }

        return sb.ToString();
    }
}
