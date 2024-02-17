#nullable enable
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;

namespace LanguageExt;

public static partial class Trait
{
    /// <summary>
    /// Find the sum of two numbers
    /// </summary>
    /// <param name="x">left hand side of the addition operation</param>
    /// <param name="y">right hand side of the addition operation</param>
    /// <returns>The sum of x and y</returns>
    [Pure]
    public static A plus<ARITH, A>(A x, A y) where ARITH : Arithmetic<A> =>
        ARITH.Plus(x, y);

    /// <summary>
    /// Find the subtract between two numbers
    /// </summary>
    /// <param name="x">left hand side of the subtraction operation</param>
    /// <param name="y">right hand side of the subtraction operation</param>
    /// <returns>The sum subtract between x and y</returns>
    [Pure]
    public static A subtract<ARITH, A>(A x, A y) where ARITH : Arithmetic<A> =>
        ARITH.Subtract(x, y);

    /// <summary>
    /// Find the product of two numbers
    /// </summary>
    /// <param name="x">left hand side of the product operation</param>
    /// <param name="y">right hand side of the product operation</param>
    /// <returns>The product of x and y</returns>
    [Pure]
    public static A product<ARITH, A>(A x, A y) where ARITH : Arithmetic<A> =>
        ARITH.Product(x, y);

    /// <summary>
    /// Negate the value
    /// </summary>
    /// <param name="x">Value to negate</param>
    /// <returns>The negated source value</returns>
    [Pure]
    public static A negate<ARITH, A>(A x) where ARITH : Arithmetic<A> =>
        ARITH.Negate(x);
}
