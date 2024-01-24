using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances;

public struct HashableCompositions<A> : Hashable<Compositions<A>>
{
    [Pure]
    public static int GetHashCode(Compositions<A> x) =>
        x.GetHashCode();
}
