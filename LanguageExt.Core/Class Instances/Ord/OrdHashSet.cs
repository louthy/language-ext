using LanguageExt.Traits;
using System.Diagnostics.Contracts;
using static LanguageExt.Prelude;

namespace LanguageExt.ClassInstances;

/// <summary>
/// Equality and ordering
/// </summary>
public struct OrdHashSet<OrdA, A> : Ord<HashSet<A>>
    where OrdA : Ord<A>
{
    /// <summary>
    /// Equality test
    /// </summary>
    /// <param name="x">The left hand side of the equality operation</param>
    /// <param name="y">The right hand side of the equality operation</param>
    /// <returns>True if x and y are equal</returns>
    [Pure]
    public static bool Equals(HashSet<A> x, HashSet<A> y) =>
        EqHashSet<OrdA, A>.Equals(x, y);

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
    public static int Compare(HashSet<A> x, HashSet<A> y)
    {
        if (x.Count > y.Count) return 1;
        if (x.Count < y.Count) return -1;
        var       sa    = toSet(x);
        var       sb    = toSet(y);
        using var iterA = sa.GetEnumerator();
        using var iterB = sb.GetEnumerator();
        while (iterA.MoveNext() && iterB.MoveNext())
        {
            var cmp = OrdA.Compare(iterA.Current, iterB.Current);
            if (cmp != 0) return cmp;
        }

        return 0;
    }

    /// <summary>
    /// Get the hash-code of the provided value
    /// </summary>
    /// <returns>Hash code of x</returns>
    [Pure]
    public static int GetHashCode(HashSet<A> x) =>
        x.GetHashCode();
}

/// <summary>
/// Equality and ordering
/// </summary>
public struct OrdHashSet<A> : Ord<HashSet<A>>
{
    /// <summary>
    /// Equality test
    /// </summary>
    /// <param name="x">The left hand side of the equality operation</param>
    /// <param name="y">The right hand side of the equality operation</param>
    /// <returns>True if x and y are equal</returns>
    [Pure]
    public static bool Equals(HashSet<A> x, HashSet<A> y) =>
        OrdHashSet<OrdDefault<A>, A>.Equals(x, y);

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
    public static int Compare(HashSet<A> x, HashSet<A> y) =>
        OrdHashSet<OrdDefault<A>, A>.Compare(x, y);

    /// <summary>
    /// Get the hash-code of the provided value
    /// </summary>
    /// <returns>Hash code of x</returns>
    [Pure]
    public static int GetHashCode(HashSet<A> x) =>
        OrdHashSet<OrdDefault<A>, A>.GetHashCode(x);
}
