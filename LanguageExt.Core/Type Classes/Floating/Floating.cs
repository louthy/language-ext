using System.Diagnostics.Contracts;
using LanguageExt.Attributes;

namespace LanguageExt.TypeClasses
{
    /// <summary>
    /// Floating point number type-class
    /// </summary>
    /// <typeparam name="A">The floating point value type</typeparam>
    [Typeclass("Float*")]
    public interface Floating<A> : Fraction<A>
    {
        /// <summary>
        /// Returns an approximation of pi.
        /// </summary>
        /// <returns>A reasonable approximation of pi in this type</returns>
        [Pure]
        A Pi();

        /// <summary>
        /// The exponential function.
        /// </summary>
        /// <param name="x">The value for which we are calculating the exponential</param>
        /// <returns>The value of <c>e^x</c></returns>
        [Pure]
        A Exp(A x);

        /// <summary>
        /// Calculates the square root of a value.
        /// </summary>
        /// <param name="x">The value for which we are calculating the square root.</param>
        /// <returns>The value of <c>sqrt(x)</c>.</returns>
        [Pure]
        A Sqrt(A x);

        /// <summary>
        /// Calculates the natural logarithm of a value.
        /// </summary>
        /// <param name="x">
        /// The value for which we are calculating the natural logarithm.
        /// </param>
        /// <returns>The value of <c>ln(x)</c>.</returns>
        [Pure]
        A Log(A x);

        /// <summary>Raises x to the power y
        /// </summary>
        /// <param name="x">The base to be raised to y</param>
        /// <param name="y">The exponent to which we are raising x</param>
        /// <returns>The value of <c>x^y</c>.</returns>
        [Pure]
        A Pow(A x, A y);

        /// <summary>
        /// Calculates the logarithm of a value with respect to an arbitrary base.
        /// </summary>
        /// <param name="x">The base to use for the logarithm of t</param>
        /// <param name="y">The value for which we are calculating the logarithm.</param>
        /// <returns>The value of <c>log x (y)</c>.</returns>
        [Pure]
        A LogBase(A x, A y);

        /// <summary>
        /// Calculates the sine of an angle.
        /// </summary>
        /// <param name="x">An angle, in radians</param>
        /// <returns>The value of <c>sin(x)</c></returns>
        [Pure]
        A Sin(A x);

        /// <summary>
        /// Calculates the cosine of an angle.
        /// </summary>
        /// <param name="x">An angle, in radians</param>
        /// <returns>The value of <c>cos(x)</c></returns>
        [Pure]
        A Cos(A x);

        /// <summary>
        ///     Calculates the tangent of an angle.
        /// </summary>
        /// <param name="x">An angle, in radians</param>
        /// <returns>The value of <c>tan(x)</c></returns>
        [Pure]
        A Tan(A x);

        /// <summary>
        /// Calculates an arcsine.
        /// </summary>
        /// <param name="x">The value for which an arcsine is to be calculated.</param>
        /// <returns>The value of <c>asin(x)</c>, in radians.</returns>
        [Pure]
        A Asin(A x);

        /// <summary>
        /// Calculates an arc-cosine.
        /// </summary>
        /// <param name="x">The value for which an arc-cosine is to be calculated</param>
        /// <returns>The value of <c>acos(x)</c>, in radians</returns>
        [Pure]
        A Acos(A x);

        /// <summary>
        /// Calculates an arc-tangent.
        /// </summary>
        /// <param name="x">The value for which an arc-tangent is to be calculated</param>
        /// <returns>The value of <c>atan(x)</c>, in radians</returns>
        [Pure]
        A Atan(A x);

        /// <summary>
        /// Calculates a hyperbolic sine.
        /// </summary>
        /// <param name="x">The value for which a hyperbolic sine is to be calculated</param>
        /// <returns>The value of <c>sinh(x)</c></returns>
        [Pure]
        A Sinh(A x);

        /// <summary>
        /// Calculates a hyperbolic cosine.
        /// </summary>
        /// <param name="x">The value for which a hyperbolic cosine is to be calculated</param>
        /// <returns>The value of <c>cosh(x)</c></returns>
        [Pure]
        A Cosh(A x);

        /// <summary>
        /// Calculates a hyperbolic tangent.
        /// </summary>
        /// <param name="x">
        /// The value for which a hyperbolic tangent is to be calculated.
        /// </param>
        /// <returns>The value of <c>tanh(x)</c></returns>
        [Pure]
        A Tanh(A x);

        /// <summary>Calculates an area hyperbolic sine</summary>
        /// <param name="x">The value for which an area hyperbolic sine is to be calculated.
        /// </param>
        /// <returns>The value of <c>asinh(x)</c>.</returns>
        [Pure]
        A Asinh(A x);

        /// <summary>
        /// Calculates an area hyperbolic cosine.
        /// </summary>
        /// <param name="x">The value for which an area hyperbolic cosine is to be calculated.
        /// </param>
        /// <returns>The value of <c>acosh(x)</c>.</returns>
        [Pure]
        A Acosh(A x);

        /// <summary>
        /// Calculates an area hyperbolic tangent.
        /// </summary>
        /// <param name="x">The value for which an area hyperbolic tangent is to be calculated.
        /// </param>
        /// <returns>The value of <c>atanh(x)</c></returns>
        [Pure]
        A Atanh(A x);
    }
}
