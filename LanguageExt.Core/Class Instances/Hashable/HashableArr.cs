using System.Diagnostics.Contracts;
using static LanguageExt.Prelude;

namespace LanguageExt.ClassInstances;

/// <summary>
/// Array hash
/// </summary>
public struct HashableArr<HashA, A> : Hashable<Arr<A>> where HashA : Hashable<A>
{
    /// <summary>
    /// Get hash code of the value
    /// </summary>
    /// <param name="x">Value to get the hash code of</param>
    /// <returns>The hash code of x</returns>
    [Pure]
    public static int GetHashCode(Arr<A> x) =>
        hash<HashA, A>(x);
}

/// <summary>
/// Array hash
/// </summary>
public struct HashableArr<A> : Hashable<Arr<A>>
{
    /// <summary>
    /// Get hash code of the value
    /// </summary>
    /// <param name="x">Value to get the hash code of</param>
    /// <returns>The hash code of x</returns>
    [Pure]
    public static int GetHashCode(Arr<A> x) =>
        HashableArr<HashableDefault<A>, A>.GetHashCode(x);
}
