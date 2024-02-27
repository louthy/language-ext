using LanguageExt.Traits;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances;

/// <summary>
/// Equality test
/// </summary>
/// <param name="x">The left hand side of the equality operation</param>
/// <param name="y">The right hand side of the equality operation</param>
/// <returns>True if x and y are equal</returns>
public struct EqSeq<EqA, A> : Eq<Seq<A>>
    where EqA : Eq<A>
{
    /// <summary>
    /// Equality check
    /// </summary>
    [Pure]
    public static bool Equals(Seq<A> x, Seq<A> y)
    {
        if (x.Count != y.Count) return false;

        while (true)
        {
            var a = x.IsEmpty;
            var b = y.IsEmpty;
            if (a != b) return false;
            if (a && b) return true;

            if (!EqA.Equals(x.Head, y.Head)) return false;
            x = x.Tail;
            y = y.Tail;
        }
    }

    /// <summary>
    /// Get hash code of the value
    /// </summary>
    /// <param name="x">Value to get the hash code of</param>
    /// <returns>The hash code of x</returns>
    [Pure]
    public static int GetHashCode(Seq<A> x) =>
        HashableSeq<EqA, A>.GetHashCode(x);
}

/// <summary>
/// Equality test
/// </summary>
/// <param name="x">The left hand side of the equality operation</param>
/// <param name="y">The right hand side of the equality operation</param>
/// <returns>True if x and y are equal</returns>
public struct EqSeq<A> : Eq<Seq<A>>
{
    /// <summary>
    /// Equality check
    /// </summary>
    [Pure]
    public static bool Equals(Seq<A> x, Seq<A> y) =>
        EqSeq<EqDefault<A>, A>.Equals(x, y);

    /// <summary>
    /// Get hash code of the value
    /// </summary>
    /// <param name="x">Value to get the hash code of</param>
    /// <returns>The hash code of x</returns>
    [Pure]
    public static int GetHashCode(Seq<A> x) =>
        HashableSeq<A>.GetHashCode(x);
}
