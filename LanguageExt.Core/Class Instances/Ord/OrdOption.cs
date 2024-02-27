using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using LanguageExt.Traits;

namespace LanguageExt.ClassInstances;

public struct OrdOption<OrdA, A> : Ord<Option<A>>
    where OrdA : Ord<A>
{
    [Pure]
    public static int Compare(Option<A> x, Option<A> y) =>
        x.CompareTo<OrdA>(y);

    [Pure]
    public static bool Equals(Option<A> x, Option<A> y) =>
        x.Equals<OrdA>(y);

    [Pure]
    public static int GetHashCode(Option<A> x) =>
        x.GetHashCode();
}
