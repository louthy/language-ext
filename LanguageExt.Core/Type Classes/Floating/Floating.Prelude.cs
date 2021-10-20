using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;

namespace LanguageExt
{
    public static partial class TypeClass
    {
        /// <summary>
        /// Ratio constructor
        /// </summary>
        /// <typeparam name="A">Value type</typeparam>
        /// <param name="num">Numerator</param>
        /// <param name="den">Denominator</param>
        /// <returns>Ratio struct</returns>
        [Pure]
        public static Ratio<A> Ratio<A>(A num, A den) =>
            new Ratio<A>(num, den);

        /// <summary>
        /// Returns an approximation of pi.
        /// </summary>
        /// <returns>A reasonable approximation of pi in this type</returns>
        [Pure]
        public static A pi<FLOAT, A>() where FLOAT : struct, Floating<A> =>
            default(FLOAT).Pi();

        /// <summary>
        /// The exponential function.
        /// </summary>
        /// <param name="x">The value for which we are calculating the exponential</param>
        /// <returns>The value of <c>e^x</c></returns>
        [Pure]
        public static A exp<FLOAT, A>(A x) where FLOAT : struct, Floating<A> =>
            default(FLOAT).Exp(x);

        /// <summary>
        /// Calculates the square root of a value.
        /// </summary>
        /// <param name="x">The value for which we are calculating the square root.</param>
        /// <returns>The value of <c>sqrt(x)</c>.</returns>
        [Pure]
        public static A sqrt<FLOAT, A>(A x) where FLOAT : struct, Floating<A> =>
            default(FLOAT).Sqrt(x);

        /// <summary>
        /// Calculates the natural logarithm of a value.
        /// </summary>
        /// <param name="x">
        /// The value for which we are calculating the natural logarithm.
        /// </param>
        /// <returns>The value of <c>ln(x)</c>.</returns>
        [Pure]
        public static A log<FLOAT, A>(A x) where FLOAT : struct, Floating<A> =>
            default(FLOAT).Log(x);

        /// <summary>Raises x to the power y
        /// </summary>
        /// <param name="x">The base to be raised to y</param>
        /// <param name="y">The exponent to which we are raising x</param>
        /// <returns>The value of <c>x^y</c>.</returns>
        [Pure]
        public static A pow<FLOAT, A>(A x, A y) where FLOAT : struct, Floating<A> =>
            default(FLOAT).Pow(x, y);

        /// <summary>
        /// Calculates the logarithm of a value with respect to an arbitrary base.
        /// </summary>
        /// <param name="b">The base to use for the logarithm of x</param>
        /// <param name="x">The value for which we are calculating the logarithm.</param>
        /// <returns>The value of <c>log b (x)</c>.</returns>
        [Pure]
        public static A logBase<FLOAT, A>(A x, A y) where FLOAT : struct, Floating<A> =>
            default(FLOAT).LogBase(x, y);

        /// <summary>
        /// Calculates the sine of an angle.
        /// </summary>
        /// <param name="x">An angle, in radians</param>
        /// <returns>The value of <c>sin(x)</c></returns>
        [Pure]
        public static A sin<FLOAT, A>(A x) where FLOAT : struct, Floating<A> =>
            default(FLOAT).Sin(x);

        /// <summary>
        /// Calculates the cosine of an angle.
        /// </summary>
        /// <param name="x">An angle, in radians</param>
        /// <returns>The value of <c>cos(x)</c></returns>
        [Pure]
        public static A cos<FLOAT, A>(A x) where FLOAT : struct, Floating<A> =>
            default(FLOAT).Cos(x);

        /// <summary>
        ///     Calculates the tangent of an angle.
        /// </summary>
        /// <param name="x">An angle, in radians</param>
        /// <returns>The value of <c>tan(x)</c></returns>
        [Pure]
        public static A tan<FLOAT, A>(A x) where FLOAT : struct, Floating<A> =>
            default(FLOAT).Tan(x);

        /// <summary>
        /// Calculates an arcsine.
        /// </summary>
        /// <param name="x">The value for which an arcsine is to be calculated.</param>
        /// <returns>The value of <c>asin(x)</c>, in radians.</returns>
        [Pure]
        public static A asin<FLOAT, A>(A x) where FLOAT : struct, Floating<A> =>
            default(FLOAT).Asin(x);

        /// <summary>
        /// Calculates an arc-cosine.
        /// </summary>
        /// <param name="x">The value for which an arc-cosine is to be calculated</param>
        /// <returns>The value of <c>acos(x)</c>, in radians</returns>
        [Pure]
        public static A acos<FLOAT, A>(A x) where FLOAT : struct, Floating<A> =>
            default(FLOAT).Acos(x);

        /// <summary>
        /// Calculates an arc-tangent.
        /// </summary>
        /// <param name="x">The value for which an arc-tangent is to be calculated</param>
        /// <returns>The value of <c>atan(x)</c>, in radians</returns>
        [Pure]
        public static A atan<FLOAT, A>(A x) where FLOAT : struct, Floating<A> =>
            default(FLOAT).Atan(x);

        /// <summary>
        /// Calculates a hyperbolic sine.
        /// </summary>
        /// <param name="x">The value for which a hyperbolic sine is to be calculated</param>
        /// <returns>The value of <c>sinh(x)</c></returns>
        [Pure]
        public static A sinh<FLOAT, A>(A x) where FLOAT : struct, Floating<A> =>
            default(FLOAT).Sinh(x);

        /// <summary>
        /// Calculates a hyperbolic cosine.
        /// </summary>
        /// <param name="x">The value for which a hyperbolic cosine is to be calculated</param>
        /// <returns>The value of <c>cosh(x)</c></returns>
        [Pure]
        public static A cosh<FLOAT, A>(A x) where FLOAT : struct, Floating<A> =>
            default(FLOAT).Cosh(x);

        /// <summary>
        /// Calculates a hyperbolic tangent.
        /// </summary>
        /// <param name="x">
        /// The value for which a hyperbolic tangent is to be calculated.
        /// </param>
        /// <returns>The value of <c>tanh(x)</c></returns>
        [Pure]
        public static A tanh<FLOAT, A>(A x) where FLOAT : struct, Floating<A> =>
            default(FLOAT).Tanh(x);

        /// <summary>Calculates an area hyperbolic sine</summary>
        /// <param name="x">The value for which an area hyperbolic sine is to be calculated.
        /// </param>
        /// <returns>The value of <c>asinh(x)</c>.</returns>
        [Pure]
        public static A asinh<FLOAT, A>(A x) where FLOAT : struct, Floating<A> =>
            default(FLOAT).Asinh(x);

        /// <summary>
        /// Calculates an area hyperbolic cosine.
        /// </summary>
        /// <param name="x">The value for which an area hyperbolic cosine is to be calculated.
        /// </param>
        /// <returns>The value of <c>acosh(x)</c>.</returns>
        [Pure]
        public static A acosh<FLOAT, A>(A x) where FLOAT : struct, Floating<A> =>
            default(FLOAT).Acosh(x);

        /// <summary>
        /// Calculates an area hyperbolic tangent.
        /// </summary>
        /// <param name="x">The value for which an area hyperbolic tangent is to be calculated.
        /// </param>
        /// <returns>The value of <c>atanh(x)</c></returns>
        [Pure]
        public static A atanh<FLOAT, A>(A x) where FLOAT : struct, Floating<A> =>
            default(FLOAT).Atanh(x);
    }
}
