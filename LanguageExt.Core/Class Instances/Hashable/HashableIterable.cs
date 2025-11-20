using static LanguageExt.Prelude;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances;

/// <summary>
/// Iterable hashing
/// </summary>
public struct HashableIterable<HashA, A> : Hashable<Iterable<A>>
    where HashA : Hashable<A>
{
    /// <summary>
    /// Get hash code of the value
    /// </summary>
    /// <param name="x">Value to get the hash code of</param>
    /// <returns>The hash code of x</returns>
    [Pure]
    public static int GetHashCode(Iterable<A> x) =>
        hash<HashA, A>(x);
}

/// <summary>
/// Iterable hashing
/// </summary>
public struct HashableIterable<A> : Hashable<Iterable<A>>
{
    /// <summary>
    /// Get hash code of the value
    /// </summary>
    /// <param name="x">Value to get the hash code of</param>
    /// <returns>The hash code of x</returns>
    [Pure]
    public static int GetHashCode(Iterable<A> x) =>
        HashableIterable<HashableDefault<A>, A>.GetHashCode(x);
}
