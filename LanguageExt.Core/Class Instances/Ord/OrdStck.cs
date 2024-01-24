using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace LanguageExt.ClassInstances;

/// <summary>
/// Equality and ordering
/// </summary>
public struct OrdStck<OrdA, A> : Ord<Stck<A>>
    where OrdA : Ord<A>
{
    /// <summary>
    /// Equality test
    /// </summary>
    /// <param name="x">The left hand side of the equality operation</param>
    /// <param name="y">The right hand side of the equality operation</param>
    /// <returns>True if x and y are equal</returns>
    [Pure]
    public static bool Equals(Stck<A> x, Stck<A> y) =>
        EqStck<OrdA, A>.Equals(x, y);

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
    public static int Compare(Stck<A> x, Stck<A> y)
    {
        var cmp = x.Count.CompareTo(y.Count);
        if (cmp == 0)
        {
            using var enumx = x.GetEnumerator();
            using var enumy = y.GetEnumerator();
            var count = x.Count;

            for (int i = 0; i < count; i++)
            {
                enumx.MoveNext();
                enumy.MoveNext();
                cmp = OrdA.Compare(enumx.Current, enumy.Current);
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
    public static int GetHashCode(Stck<A> x) =>
        x.GetHashCode();
}

/// <summary>
/// Equality and ordering
/// </summary>
public struct OrdStck<A> : Ord<Stck<A>>
{
    /// <summary>
    /// Equality test
    /// </summary>
    /// <param name="x">The left hand side of the equality operation</param>
    /// <param name="y">The right hand side of the equality operation</param>
    /// <returns>True if x and y are equal</returns>
    [Pure]
    public static bool Equals(Stck<A> x, Stck<A> y) =>
        OrdStck<OrdDefault<A>, A>.Equals(x, y);

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
    public static int Compare(Stck<A> x, Stck<A> y) =>
        OrdStck<OrdDefault<A>, A>.Compare(x, y);

    /// <summary>
    /// Get the hash-code of the provided value
    /// </summary>
    /// <returns>Hash code of x</returns>
    [Pure]
    public static int GetHashCode(Stck<A> x) =>
        OrdStck<OrdDefault<A>, A>.GetHashCode(x);
}
