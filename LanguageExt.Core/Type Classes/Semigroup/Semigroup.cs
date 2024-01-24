#nullable enable
using System.Diagnostics.Contracts;
using LanguageExt.Attributes;

namespace LanguageExt.TypeClasses;

[Trait("Semi*")]
public interface Semigroup<A> : Trait
{
    /// <summary>
    /// An associative binary operation.
    /// </summary>
    /// <param name="x">The first operand to the operation</param>
    /// <param name="y">The second operand to the operation</param>
    /// <returns>The result of the operation</returns>
    [Pure]
    public static abstract A Append(A x, A y);
}
