#nullable enable
using System.Diagnostics.Contracts;
using LanguageExt.Attributes;

namespace LanguageExt.TypeClasses;

[Trait("Num*")]
public interface Arithmetic<A> : Trait
{
    /// <summary>
    /// Find the sum of two values
    /// </summary>
    /// <param name="x">left hand side of the addition operation</param>
    /// <param name="y">right hand side of the addition operation</param>
    /// <returns>The sum of x and y</returns>
    [Pure]
    public static abstract A Plus(A x, A y);

    /// <summary>
    /// Find the difference between two values
    /// </summary>
    /// <param name="x">left hand side of the subtraction operation</param>
    /// <param name="y">right hand side of the subtraction operation</param>
    /// <returns>The difference between x and y</returns>
    [Pure]
    public static abstract A Subtract(A x, A y);

    /// <summary>
    /// Find the product of two values
    /// </summary>
    /// <param name="x">left hand side of the product operation</param>
    /// <param name="y">right hand side of the product operation</param>
    /// <returns>The product of x and y</returns>
    [Pure]
    public static abstract A Product(A x, A y);

    /// <summary>
    /// Negate the value
    /// </summary>
    /// <param name="x">Value to negate</param>
    /// <returns>The negated source value</returns>
    [Pure]
    public static abstract A Negate(A x);
}
