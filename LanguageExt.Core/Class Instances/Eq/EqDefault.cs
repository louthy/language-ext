using LanguageExt.Traits;
using static LanguageExt.Prelude;
using System;
using System.Diagnostics.Contracts;
using LanguageExt.Traits.Resolve;

namespace LanguageExt.ClassInstances;

/// <summary>
/// Finds an appropriate Eq from the loaded assemblies, if one can't be found then it
/// falls back to the standard .NET EqualityComparer〈A〉.Default.Equals(a,b) method to
/// provide equality testing.
/// </summary>
public readonly struct EqDefault<A> : Eq<A>
{
    /// <summary>
    /// Equality test
    /// </summary>
    /// <param name="x">The left hand side of the equality operation</param>
    /// <param name="y">The right hand side of the equality operation</param>
    /// <returns>True if x and y are equal</returns>
    [Pure]
    public static bool Equals(A a, A b)
    {
        if (a is null) return b is null;
        if (b is null) return false;
        if (ReferenceEquals(a, b)) return true;
        return EqResolve<A>.Equals(a, b);
    }

    /// <summary>
    /// Get hash code of the value
    /// </summary>
    /// <param name="x">Value to get the hash code of</param>
    /// <returns>The hash code of x</returns>
    [Pure]
    public static int GetHashCode(A x) =>
        HashableDefault<A>.GetHashCode(x);
}

/// <summary>
/// This is a utility type for when two generic types are used, but it's not clear if they
/// have the same underlying type.  We'd like to do structural equality if they are, and
/// return false if they're not.
/// </summary>
public static class EqDefault<A, B> 
{
    static readonly Func<A, B, bool> Eq;

    static EqDefault()
    {
        Eq = typeof(A).FullName == typeof(B).FullName
                 ? (x, y) => y is A y1 && EqDefault<A>.Equals(x, y1)
                 : (_, _) => false;
    }
        
    /// <summary>
    /// Equality test
    /// </summary>
    /// <param name="x">The left hand side of the equality operation</param>
    /// <param name="y">The right hand side of the equality operation</param>
    /// <returns>True if x and y are equal</returns>
    [Pure]
    public static bool Equals(A a, B b)
    {
        if (isnull(a)) return isnull(b);
        if (isnull(b)) return false;
        if (ReferenceEquals(a, b)) return true;
        return Eq(a, b);
    }
}
