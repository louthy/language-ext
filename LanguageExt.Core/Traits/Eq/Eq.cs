using System.Collections.Generic;
using System.Diagnostics.Contracts;
using LanguageExt.Attributes;

namespace LanguageExt.Traits;

/// <summary>
/// Equality trait
/// </summary>
/// <typeparam name="A">
/// The type for which equality is defined
/// </typeparam>
[Trait("Eq*")]
public interface Eq<in A> : Hashable<A>, Trait
{
    /// <summary>
    /// Equality test
    /// </summary>
    /// <param name="x">The left hand side of the equality operation</param>
    /// <param name="y">The right hand side of the equality operation</param>
    /// <returns>True if x and y are equal</returns>
    [Pure]
    public static abstract bool Equals(A x, A y);
}
