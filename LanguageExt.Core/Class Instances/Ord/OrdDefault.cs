using LanguageExt.Traits;
using System.Diagnostics.Contracts;
using LanguageExt.Traits.Resolve;

namespace LanguageExt.ClassInstances;

/// <summary>
/// Uses the standard .NET  Comparer〈A〉.Default.Compare(a,b) method to
/// provide equality testing.
/// </summary>
public struct OrdDefault<A> : Ord<A>
{
    /// <summary>
    /// Equality test
    /// </summary>
    /// <param name="x">The left hand side of the equality operation</param>
    /// <param name="y">The right hand side of the equality operation</param>
    /// <returns>True if x and y are equal</returns>
    [Pure]
    public static int Compare(A x, A y) =>
        OrdResolve<A>.Compare(x, y);

    [Pure]
    public static bool Equals(A x, A y) =>
        EqDefault<A>.Equals(x, y);

    /// <summary>
    /// Get the hash-code of the provided value
    /// </summary>
    /// <returns>Hash code of x</returns>
    [Pure]
    public static int GetHashCode(A x) =>
        EqDefault<A>.GetHashCode(x);
}
