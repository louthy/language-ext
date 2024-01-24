using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances;

/// <summary>
/// Set<T> equality
/// </summary>
public struct EqSet<EQ, A> : Eq<Set<A>> where EQ : Eq<A>
{
    /// <summary>
    /// Equality test
    /// </summary>
    /// <param name="x">The left hand side of the equality operation</param>
    /// <param name="y">The right hand side of the equality operation</param>
    /// <returns>True if x and y are equal</returns>
    [Pure]
    public static bool Equals(Set<A> x, Set<A> y)
    {
        if (x.Count != y.Count) return false;

        using var enumx = x.GetEnumerator();
        using var enumy = y.GetEnumerator();
        for (var i = 0; i < x.Count; i++)
        {
            enumx.MoveNext();
            enumy.MoveNext();
            if (!EQ.Equals(enumx.Current, enumy.Current)) return false;
        }
        return true;
    }


    /// <summary>
    /// Get hash code of the value
    /// </summary>
    /// <param name="x">Value to get the hash code of</param>
    /// <returns>The hash code of x</returns>
    [Pure]
    public static int GetHashCode(Set<A> x) =>
        HashableSet<EQ, A>.GetHashCode(x);
}

/// <summary>
/// Set<T> equality
/// </summary>
public struct EqSet<A> : Eq<Set<A>>
{
    /// <summary>
    /// Equality test
    /// </summary>
    /// <param name="x">The left hand side of the equality operation</param>
    /// <param name="y">The right hand side of the equality operation</param>
    /// <returns>True if x and y are equal</returns>
    [Pure]
    public static bool Equals(Set<A> x, Set<A> y) =>
        EqSet<EqDefault<A>, A>.Equals(x, y);

    /// <summary>
    /// Get hash code of the value
    /// </summary>
    /// <param name="x">Value to get the hash code of</param>
    /// <returns>The hash code of x</returns>
    [Pure]
    public static int GetHashCode(Set<A> x) =>
        HashableSet<A>.GetHashCode(x);
}
