using LanguageExt.Traits;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using static LanguageExt.Prelude;

namespace LanguageExt.ClassInstances;

/// <summary>
/// Equality and ordering
/// </summary>
public struct OrdIterable<OrdA, A> : Ord<Iterable<A>>
    where OrdA : Ord<A>
{
    /// <summary>
    /// Equality test
    /// </summary>
    /// <param name="x">The left hand side of the equality operation</param>
    /// <param name="y">The right hand side of the equality operation</param>
    /// <returns>True if x and y are equal</returns>
    [Pure]
    public static bool Equals(Iterable<A> x, Iterable<A> y) =>
        EqIterable<OrdA, A>.Equals(x, y);

    /// <summary>
    /// Compare two values
    /// </summary>
    /// <param name="x">Left hand side of the compare operation</param>
    /// <param name="y">Right hand side of the compare operation</param>
    /// <returns>
    /// if x greater than y : 1
    /// if x less than y    : -1
    /// if x equals y       : 0
    /// </returns>
    [Pure]
    public static int Compare(Iterable<A> x, Iterable<A> y)
    {
        if (ReferenceEquals(x, y)) return 0;
        if (ReferenceEquals(x, null)) return -1;
        if (ReferenceEquals(y, null)) return 1;

        using var enumx = x.GetEnumerator();
        using var enumy = y.GetEnumerator();

        while (true)
        {
            bool r1 = enumx.MoveNext();
            bool r2 = enumy.MoveNext();
            if (!r1 && !r2) return 0;
            if (!r1) return -1;
            if (!r2) return 1;

            var cmp = OrdA.Compare(enumx.Current, enumy.Current);
            if (cmp != 0) return cmp;
        }
    }

    /// <summary>
    /// Get the hash-code of the provided value
    /// </summary>
    /// <returns>Hash code of x</returns>
    [Pure]
    public static int GetHashCode(Iterable<A> x) =>
        hash(x);
}

/// <summary>
/// Equality and ordering
/// </summary>
public struct OrdIterable<A> : Ord<Iterable<A>>
{
    /// <summary>
    /// Equality test
    /// </summary>
    /// <param name="x">The left hand side of the equality operation</param>
    /// <param name="y">The right hand side of the equality operation</param>
    /// <returns>True if x and y are equal</returns>
    [Pure]
    public static bool Equals(Iterable<A> x, Iterable<A> y) =>
        OrdIterable<OrdDefault<A>, A>.Equals(x, y);

    /// <summary>
    /// Compare two values
    /// </summary>
    /// <param name="x">Left hand side of the compare operation</param>
    /// <param name="y">Right hand side of the compare operation</param>
    /// <returns>
    /// if x greater than y : 1
    /// if x less than y    : -1
    /// if x equals y       : 0
    /// </returns>
    [Pure]
    public static int Compare(Iterable<A> x, Iterable<A> y) =>
        OrdIterable<OrdDefault<A>, A>.Compare(x, y);

    /// <summary>
    /// Get the hash-code of the provided value
    /// </summary>
    /// <returns>Hash code of x</returns>
    [Pure]
    public static int GetHashCode(Iterable<A> x) =>
        OrdIterable<OrdDefault<A>, A>.GetHashCode(x);
}
