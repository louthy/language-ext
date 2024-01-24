using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace LanguageExt.ClassInstances;

/// <summary>
/// Identity equality
/// </summary>
public struct OrdIdentity<A> : Ord<Identity<A>>
{
    /// <summary>
    /// Ordering test
    /// </summary>
    /// <param name="x">The left hand side of the ordering operation</param>
    /// <param name="y">The right hand side of the ordering operation</param>
    /// <returns>-1 if less than, 0 if equal, 1 if greater than</returns>
    [Pure]
    public static int Compare(Identity<A> a, Identity<A> b)  => 
        a.CompareTo(b);

    /// <summary>
    /// Equality test
    /// </summary>
    /// <param name="x">The left hand side of the equality operation</param>
    /// <param name="y">The right hand side of the equality operation</param>
    /// <returns>True if x and y are equal</returns>
    [Pure]
    public static bool Equals(Identity<A> a, Identity<A> b)  => 
        EqIdentity<A>.Equals(a, b);

    /// <summary>
    /// Get hash code of the value
    /// </summary>
    /// <param name="x">Value to get the hash code of</param>
    /// <returns>The hash code of x</returns>
    [Pure]
    public static int GetHashCode(Identity<A> x) =>
        HashableIdentity<A>.GetHashCode(x);
}
