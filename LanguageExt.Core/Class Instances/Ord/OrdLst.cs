using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace LanguageExt.ClassInstances;

/// <summary>
/// Equality and ordering
/// </summary>
public struct OrdLst<OrdA, A> : Ord<Lst<A>>
    where OrdA : Ord<A>
{
    /// <summary>
    /// Equality test
    /// </summary>
    /// <param name="x">The left hand side of the equality operation</param>
    /// <param name="y">The right hand side of the equality operation</param>
    /// <returns>True if x and y are equal</returns>
    [Pure]
    public static bool Equals(Lst<A> x, Lst<A> y) =>
        EqLst<OrdA, A>.Equals(x, y);

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
    public static int Compare(Lst<A> mx, Lst<A> my)
    {
        var cmp = mx.Count.CompareTo(my.Count);
        if (cmp == 0)
        {
            using var xiter = mx.GetEnumerator();
            using var yiter = my.GetEnumerator();
            while (xiter.MoveNext() && yiter.MoveNext())
            {
                cmp = OrdA.Compare(xiter.Current, yiter.Current);
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
    public static int GetHashCode(Lst<A> x) =>
        x.GetHashCode();
}

/// <summary>
/// Equality and ordering
/// </summary>
public struct OrdLst<A> : Ord<Lst<A>>
{
    /// <summary>
    /// Equality test
    /// </summary>
    /// <param name="x">The left hand side of the equality operation</param>
    /// <param name="y">The right hand side of the equality operation</param>
    /// <returns>True if x and y are equal</returns>
    [Pure]
    public static bool Equals(Lst<A> x, Lst<A> y) =>
        OrdLst<OrdDefault<A>, A>.Equals(x, y);

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
    public static int Compare(Lst<A> x, Lst<A> y) =>
        OrdLst<OrdDefault<A>, A>.Compare(x, y);

    /// <summary>
    /// Get the hash-code of the provided value
    /// </summary>
    /// <returns>Hash code of x</returns>
    [Pure]
    public static int GetHashCode(Lst<A> x) =>
        OrdLst<OrdDefault<A>, A>.GetHashCode(x);
}
