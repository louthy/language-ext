using LanguageExt.Traits;
using System.Diagnostics.Contracts;
using static LanguageExt.Prelude;

namespace LanguageExt.ClassInstances;

/// <summary>
/// Equality and ordering
/// </summary>
public struct OrdSeq<OrdA, A> : Ord<Seq<A>>
    where OrdA : Ord<A>
{
    /// <summary>
    /// Equality test
    /// </summary>
    /// <param name="x">The left hand side of the equality operation</param>
    /// <param name="y">The right hand side of the equality operation</param>
    /// <returns>True if x and y are equal</returns>
    [Pure]
    public static bool Equals(Seq<A> x, Seq<A> y) =>
        EqSeq<OrdA, A>.Equals(x, y);

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
    public static int Compare(Seq<A> x, Seq<A> y)
    {
        using var enumx = x.GetEnumerator();
        using var enumy = y.GetEnumerator();

        while(true)
        {
            var r1 = enumx.MoveNext();
            var r2 = enumy.MoveNext();
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
    public static int GetHashCode(Seq<A> x) =>
        hash(x);
}

/// <summary>
/// Equality and ordering
/// </summary>
public struct OrdSeq<A> : Ord<Seq<A>>
{
    /// <summary>
    /// Equality test
    /// </summary>
    /// <param name="x">The left hand side of the equality operation</param>
    /// <param name="y">The right hand side of the equality operation</param>
    /// <returns>True if x and y are equal</returns>
    [Pure]
    public static bool Equals(Seq<A> x, Seq<A> y) =>
        OrdSeq<OrdDefault<A>, A>.Equals(x, y);

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
    public static int Compare(Seq<A> x, Seq<A> y) =>
        OrdSeq<OrdDefault<A>, A>.Compare(x, y);

    /// <summary>
    /// Get the hash-code of the provided value
    /// </summary>
    /// <returns>Hash code of x</returns>
    [Pure]
    public static int GetHashCode(Seq<A> x) =>
        OrdSeq<OrdDefault<A>, A>.GetHashCode(x);
}
