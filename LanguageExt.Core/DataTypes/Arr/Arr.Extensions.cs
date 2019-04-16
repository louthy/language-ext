using System.Diagnostics.Contracts;
using System.Linq;
using LanguageExt;
using static LanguageExt.Prelude;

public static class ArrExtensions
{
    /// <summary>
    /// Monadic join
    /// </summary>
    [Pure]
    public static A[] Flatten<A>(this A[][] ma) =>
        ma.Bind(identity).ToArray();

    /// <summary>
    /// Monadic join
    /// </summary>
    [Pure]
    public static Arr<A> Flatten<A>(this Arr<Arr<A>> ma) =>
        ma.Bind(identity);
}
