using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances;

/// <summary>
/// Lst hash
/// </summary>
public struct HashableLst<HashA, A> : Hashable<Lst<A>> where HashA : Hashable<A>
{
    /// <summary>
    /// Get hash code of the value
    /// </summary>
    /// <param name="x">Value to get the hash code of</param>
    /// <returns>The hash code of x</returns>
    [Pure]
    public static int GetHashCode(Lst<A> x) =>
        hash<HashA, A>(x);
}

/// <summary>
/// Lst hash
/// </summary>
public struct HashableLst<A> : Hashable<Lst<A>> 
{
    /// <summary>
    /// Get hash code of the value
    /// </summary>
    /// <param name="x">Value to get the hash code of</param>
    /// <returns>The hash code of x</returns>
    [Pure]
    public static int GetHashCode(Lst<A> x) =>
        HashableLst<HashableDefault<A>, A>.GetHashCode(x);
}
