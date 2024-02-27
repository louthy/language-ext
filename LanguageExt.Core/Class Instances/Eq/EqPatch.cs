using System.Diagnostics.Contracts;
using LanguageExt.Traits;

namespace LanguageExt.ClassInstances;

/// <summary>
/// Equality instance for `Patch`
/// </summary>
public struct EqPatch<EqA, A> : Eq<Patch<EqA, A>> where EqA : Eq<A>
{
    [Pure]
    public static bool Equals(Patch<EqA, A> x, Patch<EqA, A> y) => 
        x == y;

    [Pure]
    public static int GetHashCode(Patch<EqA, A> x) =>
        HashablePatch<EqA, A>.GetHashCode(x);
}
