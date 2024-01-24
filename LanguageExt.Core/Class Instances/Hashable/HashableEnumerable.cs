using static LanguageExt.Prelude;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances;

/// <summary>
/// Enumerable hashing
/// </summary>
public struct HashableEnumerable<HashA, A> : Hashable<IEnumerable<A>>
    where HashA : Hashable<A>
{
    /// <summary>
    /// Get hash code of the value
    /// </summary>
    /// <param name="x">Value to get the hash code of</param>
    /// <returns>The hash code of x</returns>
    [Pure]
    public static int GetHashCode(IEnumerable<A> x) =>
        hash<HashA, A>(x);
}

/// <summary>
/// Enumerable hashing
/// </summary>
public struct HashableEnumerable<A> : Hashable<IEnumerable<A>>
{
    /// <summary>
    /// Get hash code of the value
    /// </summary>
    /// <param name="x">Value to get the hash code of</param>
    /// <returns>The hash code of x</returns>
    [Pure]
    public static int GetHashCode(IEnumerable<A> x) =>
        HashableEnumerable<HashableDefault<A>, A>.GetHashCode(x);
}
