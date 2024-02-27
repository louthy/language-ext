using LanguageExt.Traits;
using System.Diagnostics.Contracts;
using static LanguageExt.Prelude;

namespace LanguageExt.ClassInstances;

/// <summary>
/// Equality and ordering
/// </summary>
public struct OrdArray<OrdA, A> : Ord<A[]>
    where OrdA : Ord<A>
{
    /// <summary>
    /// Equality test
    /// </summary>
    /// <param name="x">The left hand side of the equality operation</param>
    /// <param name="y">The right hand side of the equality operation</param>
    /// <returns>True if x and y are equal</returns>
    [Pure]
    public static bool Equals(A[] x, A[] y) =>
        EqArray<OrdA, A>.Equals(x, y);

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
    public static int Compare(A[] mx, A[] my)
    {
        if (ReferenceEquals(mx, my)) return 0;
        if (ReferenceEquals(mx, null)) return -1;
        if (ReferenceEquals(my, null)) return 1;

        var cmp = mx.Length.CompareTo(my.Length);
        if (cmp == 0)
        {
            for(var i = 0; i < mx.Length; i++)
            {
                cmp = OrdA.Compare(mx[i], my[i]);
                if (cmp != 0) return cmp;
            }
            return 0;
        }
        else
        {
            return cmp;
        }
    }

    /// <summary>
    /// Get the hash-code of the provided value
    /// </summary>
    /// <returns>Hash code of x</returns>
    [Pure]
    public static int GetHashCode(A[] x) =>
        hash(x);
}

/// <summary>
/// Equality and ordering
/// </summary>
public struct OrdArray<A> : Ord<A[]>
{
    /// <summary>
    /// Equality test
    /// </summary>
    /// <param name="x">The left hand side of the equality operation</param>
    /// <param name="y">The right hand side of the equality operation</param>
    /// <returns>True if x and y are equal</returns>
    [Pure]
    public static bool Equals(A[] x, A[] y) =>
        EqArray<A>.Equals(x, y);

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
    public static int Compare(A[] mx, A[] my) =>
        OrdArray<OrdDefault<A>, A>.Compare(mx, my);

    /// <summary>
    /// Get the hash-code of the provided value
    /// </summary>
    /// <returns>Hash code of x</returns>
    [Pure]
    public static int GetHashCode(A[] x) =>
        OrdArray<OrdDefault<A>, A>.GetHashCode(x);
}
