using System.Diagnostics.Contracts;
using LanguageExt.TypeClasses;

namespace LanguageExt.ClassInstances;

public struct EqCompositions<A> : Eq<Compositions<A>>
{
    [Pure]
    public static bool Equals(Compositions<A> x, Compositions<A> y) =>
        x == y;

    [Pure]
    public static int GetHashCode(Compositions<A> x) =>
        HashableCompositions<A>.GetHashCode(x);
}
