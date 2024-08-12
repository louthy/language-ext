using LanguageExt.Traits;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances;

/// <summary>
/// Equality test
/// </summary>
/// <param name="x">The left hand side of the equality operation</param>
/// <param name="y">The right hand side of the equality operation</param>
/// <returns>True if x and y are equal</returns>
public struct EqIterable<EQ, A> : Eq<Iterable<A>>
    where EQ : Eq<A>
{
    /// <summary>
    /// Equality check
    /// </summary>
    [Pure]
    public static bool Equals(Iterable<A> x, Iterable<A> y)
    {
        using var enumx = x.GetEnumerator();
        using var enumy = y.GetEnumerator();
        while (true)
        {
            var a = enumx.MoveNext();
            var b = enumy.MoveNext();
            if (a != b) return false;
            if (!a && !b) return true;
            if (!EQ.Equals(enumx.Current, enumy.Current)) return false;
        }
    }

    /// <summary>
    /// Get hash code of the value
    /// </summary>
    /// <param name="x">Value to get the hash code of</param>
    /// <returns>The hash code of x</returns>
    [Pure]
    public static int GetHashCode(Iterable<A> x) =>
        HashableIterable<EQ, A>.GetHashCode(x);
}

/// <summary>
/// Equality test
/// </summary>
/// <param name="x">The left hand side of the equality operation</param>
/// <param name="y">The right hand side of the equality operation</param>
/// <returns>True if x and y are equal</returns>
public struct EqIterable<A> : Eq<Iterable<A>>
{
    /// <summary>
    /// Equality check
    /// </summary>
    [Pure]
    public static bool Equals(Iterable<A> x, Iterable<A> y) =>
        EqIterable<EqDefault<A>, A>.Equals(x, y);

    /// <summary>
    /// Get hash code of the value
    /// </summary>
    /// <param name="x">Value to get the hash code of</param>
    /// <returns>The hash code of x</returns>
    [Pure]
    public static int GetHashCode(Iterable<A> x) =>
        HashableIterable<A>.GetHashCode(x);
}
 
