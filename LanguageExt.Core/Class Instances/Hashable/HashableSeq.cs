using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances;

/// <summary>
/// Seq hashing
/// </summary>
public struct HashableSeq<HashA, A> : Hashable<Seq<A>>
    where HashA : Hashable<A>
{
    /// <summary>
    /// Get hash code of the value
    /// </summary>
    /// <param name="x">Value to get the hash code of</param>
    /// <returns>The hash code of x</returns>
    [Pure]
    public static int GetHashCode(Seq<A> x) =>
        hash<HashA, A>(x);
}

/// <summary>
/// Seq hashing
/// </summary>
public struct HashableSeq<A> : Hashable<Seq<A>>
{
    /// <summary>
    /// Get hash code of the value
    /// </summary>
    /// <param name="x">Value to get the hash code of</param>
    /// <returns>The hash code of x</returns>
    [Pure]
    public static int GetHashCode(Seq<A> x) =>
        HashableSeq<HashableDefault<A>, A>.GetHashCode(x);
}
