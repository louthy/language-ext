#nullable enable
using System.Diagnostics.Contracts;
using LanguageExt.Attributes;

namespace LanguageExt.TypeClasses;

/// <summary>
/// Numerical value trait
/// </summary>
/// <typeparam name="A">The type for which the number operations are
/// defined.</typeparam>
[Trait("Num*")]
public interface Num<A> : Ord<A>, Monoid<A>, Arithmetic<A>
{
    /// <summary>
    /// Find the absolute value of a number
    /// </summary>
    /// <param name="x">The value to find the absolute value of</param>
    /// <returns>The non-negative absolute value of x</returns>
    [Pure]
    public static abstract A Abs(A x);

    /// <summary>
    /// Find the sign of x
    /// </summary>
    /// <param name="x">The value to find the sign of</param>
    /// <returns>-1, 0, or +1</returns>
    [Pure]
    public static abstract A Signum(A x);

    /// <summary>
    /// Generate a numeric value from an integer
    /// </summary>
    /// <param name="x">The integer to use</param>
    /// <returns>The equivalent of x in the Num<A></returns>
    [Pure]
    public static abstract A FromInteger(int x);

    /// <summary>
    /// Generate a numeric value from a decimal
    /// </summary>
    /// <param name="x">The decimal to use</param>
    /// <returns>The equivalent of x in the Num<A></returns>
    [Pure]
    public static abstract A FromDecimal(decimal x);

    /// <summary>
    /// Generate a numeric value from a float
    /// </summary>
    /// <param name="x">The float to use</param>
    /// <returns>The equivalent of x in the Num<A></returns>
    [Pure]
    public static abstract A FromFloat(float x);

    /// <summary>
    /// Generate a numeric value from a double
    /// </summary>
    /// <param name="x">The double to use</param>
    /// <returns>The equivalent of x in the Num<A></returns>
    [Pure]
    public static abstract A FromDouble(double x);

    /// <summary>
    /// Divide two numbers
    /// </summary>
    /// <param name="x">left hand side of the division operation</param>
    /// <param name="y">right hand side of the division operation</param>
    /// <returns>x / y</returns>
    [Pure]
    public static abstract A Divide(A x, A y);
}
