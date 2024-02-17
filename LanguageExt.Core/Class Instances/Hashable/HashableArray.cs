using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using static LanguageExt.Prelude;
using static LanguageExt.Trait;

namespace LanguageExt.ClassInstances;

/// <summary>
/// Array hash
/// </summary>
public struct HashableArray<HashA, A> : Hashable<A[]> where HashA : Hashable<A>
{
    /// <summary>
    /// Get hash code of the value
    /// </summary>
    /// <param name="x">Value to get the hash code of</param>
    /// <returns>The hash code of x</returns>
    [Pure]
    public static int GetHashCode(A[] x) =>
        hash<HashA, A>(x);
}

/// <summary>
/// Array hash
/// </summary>
public struct HashableArray<A> : Hashable<A[]>
{
    /// <summary>
    /// Get hash code of the value
    /// </summary>
    /// <param name="x">Value to get the hash code of</param>
    /// <returns>The hash code of x</returns>
    [Pure]
    public static int GetHashCode(A[] x) =>
        HashableArray<HashableDefault<A>, A>.GetHashCode(x);
}
