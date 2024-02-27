using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using LanguageExt.Traits;

namespace LanguageExt.ClassInstances;

/// <summary>
/// Hash for `Patch`
/// </summary>
public struct HashablePatch<EqA, A> : Hashable<Patch<EqA, A>> where EqA : Eq<A>
{
    /// <summary>
    /// Get hash code of the value
    /// </summary>
    /// <param name="x">Value to get the hash code of</param>
    /// <returns>The hash code of x</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetHashCode(Patch<EqA, A> x) => 
        x.GetHashCode();
}
