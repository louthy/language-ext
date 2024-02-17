using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;
using static LanguageExt.Trait;

namespace LanguageExt.ClassInstances;

/// <summary>
/// Array equality
/// </summary>
public struct EqArray<EqA, A> : Eq<A[]> where EqA : Eq<A>
{
    /// <summary>
    /// Equality test
    /// </summary>
    /// <param name="x">The left hand side of the equality operation</param>
    /// <param name="y">The right hand side of the equality operation</param>
    /// <returns>True if x and y are equal</returns>
    [Pure]
    public static bool Equals(A[] x, A[] y)
    {
        if (x.Length != y.Length) return false;
        for (var i = 0; i < x.Length; i++)
        {
            if (!equals<EqA, A>(x[i], y[i])) return false;
        }
        return true;
    }

    /// <summary>
    /// Get hash code of the value
    /// </summary>
    /// <param name="x">Value to get the hash code of</param>
    /// <returns>The hash code of x</returns>
    [Pure]
    public static int GetHashCode(A[] x) =>
        HashableArray<EqA, A>.GetHashCode(x);
}

/// <summary>
/// Array equality
/// </summary>
public struct EqArray<A> : Eq<A[]>
{
    /// <summary>
    /// Equality test
    /// </summary>
    /// <param name="x">The left hand side of the equality operation</param>
    /// <param name="y">The right hand side of the equality operation</param>
    /// <returns>True if x and y are equal</returns>
    [Pure]
    public static bool Equals(A[] x, A[] y) =>
        EqArray<EqDefault<A>, A>.Equals(x, y);

    /// <summary>
    /// Get hash code of the value
    /// </summary>
    /// <param name="x">Value to get the hash code of</param>
    /// <returns>The hash code of x</returns>
    [Pure]
    public static int GetHashCode(A[] x) =>
        HashableArray<HashableDefault<A>, A>.GetHashCode(x);
}
