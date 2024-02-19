using System;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances;

public struct TDecimal : Floating<decimal>
{
    /// <summary>
    /// Equality test
    /// </summary>
    /// <param name="x">Left hand side of the equality operation</param>
    /// <param name="y">Right hand side of the equality operation</param>
    /// <returns>True if parameters are equal</returns>
    [Pure]
    public static bool Equals(decimal x, decimal y) => x == y;

    /// <summary>
    /// Compare two values
    /// </summary>
    /// <param name="x">Left hand side of the compare operation</param>
    /// <param name="y">Right hand side of the compare operation</param>
    /// <returns>
    /// if x greater than y : 1
    /// 
    /// if x less than y    : -1
    /// 
    /// if x equals y       : 0
    /// </returns>
    [Pure]
    public static int Compare(decimal x, decimal y) =>
        x.CompareTo(y);

    /// <summary>
    /// Find the sum of two values
    /// </summary>
    /// <param name="x">left hand side of the addition operation</param>
    /// <param name="y">right hand side of the addition operation</param>
    /// <returns>The sum of x and y</returns>
    [Pure]
    public static decimal Plus(decimal x, decimal y) => x + y;

    /// <summary>
    /// Find the difference between two values
    /// </summary>
    /// <param name="x">left hand side of the subtraction operation</param>
    /// <param name="y">right hand side of the subtraction operation</param>
    /// <returns>The difference between x and y</returns>
    [Pure]
    public static decimal Subtract(decimal x, decimal y) => x - y;

    /// <summary>
    /// Find the product of two values
    /// </summary>
    /// <param name="x">left hand side of the product operation</param>
    /// <param name="y">right hand side of the product operation</param>
    /// <returns>The product of x and y</returns>
    [Pure]
    public static decimal Product(decimal x, decimal y) => x * y;

    /// <summary>
    /// Divide two numbers
    /// </summary>
    /// <param name="x">left hand side of the division operation</param>
    /// <param name="y">right hand side of the division operation</param>
    /// <returns>x / y</returns>
    [Pure]
    public static decimal Divide(decimal x, decimal y) =>
        x / y;

    /// <summary>
    /// Find the absolute value of a number
    /// </summary>
    /// <param name="x">The value to find the absolute value of</param>
    /// <returns>The non-negative absolute value of x</returns>
    [Pure]
    public static decimal Abs(decimal x) => Math.Abs(x);

    /// <summary>
    /// Find the sign of x
    /// </summary>
    /// <param name="x">The value to find the sign of</param>
    /// <returns>-1, 0, or +1</returns>
    [Pure]
    public static decimal Signum(decimal x) => 
        Math.Sign(x);

    /// <summary>
    /// Generate a numeric value from an integer
    /// </summary>
    /// <param name="x">The integer to use</param>
    /// <returns>The equivalent of x in the Num<A></returns>
    [Pure]
    public static decimal FromInteger(int x) => 
        x;

    /// <summary>
    /// Generate a numeric value from a decimal
    /// </summary>
    /// <param name="x">The decimal to use</param>
    /// <returns>The equivalent of x in the Num<A></returns>
    [Pure]
    public static decimal FromDecimal(decimal x) => 
        x;

    /// <summary>
    /// Generate a numeric value from a float
    /// </summary>
    /// <param name="x">The float to use</param>
    /// <returns>The equivalent of x in the Num<A></returns>
    [Pure]
    public static decimal FromFloat(float x) => 
        (decimal)x;

    /// <summary>
    /// Generate a numeric value from a double
    /// </summary>
    /// <param name="x">The double to use</param>
    /// <returns>The equivalent of x in the Num<A></returns>
    [Pure]
    public static decimal FromDouble(double x) => (decimal)x;

    /// <summary>
    /// Generates a fractional value from an integer ratio.
    /// </summary>
    /// <param name="x">The ratio to convert</param>
    /// <returns>The equivalent of x in the implementing type.</returns>
    [Pure]
    public static decimal FromRational(Ratio<int> x) => 
        (decimal)x.Numerator / x.Denominator;

    /// <summary>
    /// Returns an approximation of pi.
    /// </summary>
    /// <returns>A reasonable approximation of pi in this type</returns>
    [Pure]
    public static decimal Pi() => 
        (decimal)Math.PI;

    /// <summary>
    /// The exponential function.
    /// </summary>
    /// <param name="x">The value for which we are calculating the exponential</param>
    /// <returns>The value of <c>e^x</c></returns>
    [Pure]
    public static decimal Exp(decimal x) => 
        (decimal)Math.Exp((double)x);

    /// <summary>
    /// Calculates the square root of a value.
    /// </summary>
    /// <param name="x">The value for which we are calculating the square root.</param>
    /// <returns>The value of <c>sqrt(x)</c>.</returns>
    [Pure]
    public static decimal Sqrt(decimal x) => 
        (decimal)Math.Sqrt((double)x);

    /// <summary>
    /// Calculates the natural logarithm of a value.
    /// </summary>
    /// <param name="x">
    /// The value for which we are calculating the natural logarithm.
    /// </param>
    /// <returns>The value of <c>ln(x)</c>.</returns>
    [Pure]
    public static decimal Log(decimal x) => 
        (decimal)Math.Log((double)x);

    /// <summary>Raises x to the power y
    /// </summary>
    /// <param name="x">The base to be raised to y</param>
    /// <param name="y">The exponent to which we are raising x</param>
    /// <returns>The value of <c>x^y</c>.</returns>
    [Pure]
    public static decimal Pow(decimal x, decimal y) => 
        (decimal)Math.Pow((double)x, (double)y);

    /// <summary>
    /// Calculates the logarithm of a value with respect to an arbitrary base.
    /// </summary>
    /// <param name="x">The base to use for the logarithm of t</param>
    /// <param name="y">The value for which we are calculating the logarithm.</param>
    /// <returns>The value of <c>log x (y)</c>.</returns>
    [Pure]
    public static decimal LogBase(decimal b, decimal x) => 
        (decimal)Math.Log((double)x, (double)b);

    /// <summary>
    /// Calculates the sine of an angle.
    /// </summary>
    /// <param name="x">An angle, in radians</param>
    /// <returns>The value of <c>sin(x)</c></returns>
    [Pure]
    public static decimal Sin(decimal x) => 
        (decimal)Math.Sin((double)x);

    /// <summary>
    /// Calculates the cosine of an angle.
    /// </summary>
    /// <param name="x">An angle, in radians</param>
    /// <returns>The value of <c>cos(x)</c></returns>
    [Pure]
    public static decimal Cos(decimal x) => 
        (decimal)Math.Cos((double)x);

    /// <summary>
    ///     Calculates the tangent of an angle.
    /// </summary>
    /// <param name="x">An angle, in radians</param>
    /// <returns>The value of <c>tan(x)</c></returns>
    [Pure]
    public static decimal Tan(decimal x) => 
        (decimal)Math.Tan((double)x);

    /// <summary>
    /// Calculates an arcsine.
    /// </summary>
    /// <param name="x">The value for which an arcsine is to be calculated.</param>
    /// <returns>The value of <c>asin(x)</c>, in radians.</returns>
    [Pure]
    public static decimal Asin(decimal x) => 
        (decimal)Math.Asin((double)x);

    /// <summary>
    /// Calculates an arc-cosine.
    /// </summary>
    /// <param name="x">The value for which an arc-cosine is to be calculated</param>
    /// <returns>The value of <c>acos(x)</c>, in radians</returns>
    [Pure]
    public static decimal Acos(decimal x) => 
        (decimal)Math.Acos((double)x);

    /// <summary>
    /// Calculates an arc-tangent.
    /// </summary>
    /// <param name="x">The value for which an arc-tangent is to be calculated</param>
    /// <returns>The value of <c>atan(x)</c>, in radians</returns>
    [Pure]
    public static decimal Atan(decimal x) => 
        (decimal)Math.Atan((double)x);

    /// <summary>
    /// Calculates a hyperbolic sine.
    /// </summary>
    /// <param name="x">The value for which a hyperbolic sine is to be calculated</param>
    /// <returns>The value of <c>sinh(x)</c></returns>
    [Pure]
    public static decimal Sinh(decimal x) => 
        (decimal)Math.Sinh((double)x);

    /// <summary>
    /// Calculates a hyperbolic cosine.
    /// </summary>
    /// <param name="x">The value for which a hyperbolic cosine is to be calculated</param>
    /// <returns>The value of <c>cosh(x)</c></returns>
    [Pure]
    public static decimal Cosh(decimal x) => 
        (decimal)Math.Cosh((double)x);

    /// <summary>
    /// Calculates a hyperbolic tangent.
    /// </summary>
    /// <param name="x">
    /// The value for which a hyperbolic tangent is to be calculated.
    /// </param>
    /// <returns>The value of <c>tanh(x)</c></returns>
    [Pure]
    public static decimal Tanh(decimal x) => 
        (decimal)Math.Tanh((double)x);

    /// <summary>Calculates an area hyperbolic sine</summary>
    /// <param name="x">The value for which an area hyperbolic sine is to be calculated.
    /// </param>
    /// <returns>The value of <c>asinh(x)</c>.</returns>
    [Pure]
    public static decimal Asinh(decimal x) => 
        Log(x + Sqrt((x * x) + 1m));

    /// <summary>
    /// Calculates an area hyperbolic cosine.
    /// </summary>
    /// <param name="x">The value for which an area hyperbolic cosine is to be calculated.
    /// </param>
    /// <returns>The value of <c>acosh(x)</c>.</returns>
    [Pure]
    public static decimal Acosh(decimal x) => 
        Log(x + Sqrt((x * x) - 1m));

    /// <summary>
    /// Calculates an area hyperbolic tangent.
    /// </summary>
    /// <param name="x">The value for which an area hyperbolic tangent is to be calculated.
    /// </param>
    /// <returns>The value of <c>atanh(x)</c></returns>
    [Pure]
    public static decimal Atanh(decimal x) => 
        0.5m * Log((1m + x) / (1m - x));

    /// <summary>
    /// Negate the value
    /// </summary>
    /// <param name="x">Value to negate</param>
    /// <returns>The negated source value</returns>
    [Pure]
    public static decimal Negate(decimal x) => 
        -x;

    /// <summary>
    /// Get the hash-code of the provided value
    /// </summary>
    /// <returns>Hash code of x</returns>
    [Pure]
    public static int GetHashCode(decimal x) =>
        x.GetHashCode();
}
