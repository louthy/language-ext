using System;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace LanguageExt.ClassInstances
{
    public struct TFloat : Ord<float>, Floating<float>
    {
        public static readonly TFloat Inst = default(TFloat);

        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="x">Left hand side of the equality operation</param>
        /// <param name="y">Right hand side of the equality operation</param>
        /// <returns>True if parameters are equal</returns>
        [Pure]
        public bool Equals(float x, float y) => x == y;

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
        public int Compare(float x, float y) =>
            x.CompareTo(y);

        /// <summary>
        /// Find the sum of two values
        /// </summary>
        /// <param name="x">left hand side of the addition operation</param>
        /// <param name="y">right hand side of the addition operation</param>
        /// <returns>The sum of x and y</returns>
        [Pure]
        public float Plus(float x, float y) => x + y;

        /// <summary>
        /// Find the difference between two values
        /// </summary>
        /// <param name="x">left hand side of the subtraction operation</param>
        /// <param name="y">right hand side of the subtraction operation</param>
        /// <returns>The difference between x and y</returns>
        [Pure]
        public float Subtract(float x, float y) => 
            x - y;

        /// <summary>
        /// Find the product of two values
        /// </summary>
        /// <param name="x">left hand side of the product operation</param>
        /// <param name="y">right hand side of the product operation</param>
        /// <returns>The product of x and y</returns>
        [Pure]
        public float Product(float x, float y) => x * y;

        /// <summary>
        /// Divide two numbers
        /// </summary>
        /// <param name="x">left hand side of the division operation</param>
        /// <param name="y">right hand side of the division operation</param>
        /// <returns>x / y</returns>
        [Pure]
        public float Divide(float x, float y) =>
            x / y;

        /// <summary>
        /// Find the absolute value of a number
        /// </summary>
        /// <param name="x">The value to find the absolute value of</param>
        /// <returns>The non-negative absolute value of x</returns>
        [Pure]
        public float Abs(float x) => Math.Abs(x);

        /// <summary>
        /// Find the sign of x
        /// </summary>
        /// <param name="x">The value to find the sign of</param>
        /// <returns>-1, 0, or +1</returns>
        [Pure]
        public float Signum(float x) => Math.Sign(x);

        /// <summary>
        /// Generate a numeric value from an integer
        /// </summary>
        /// <param name="x">The integer to use</param>
        /// <returns>The equivalent of x in the Num<A></returns>
        [Pure]
        public float FromInteger(int x) => (float)x;

        /// <summary>
        /// Generate a numeric value from a decimal
        /// </summary>
        /// <param name="x">The decimal to use</param>
        /// <returns>The equivalent of x in the Num<A></returns>
        [Pure]
        public float FromDecimal(decimal x) => (float)x;

        /// <summary>
        /// Generate a numeric value from a float
        /// </summary>
        /// <param name="x">The float to use</param>
        /// <returns>The equivalent of x in the Num<A></returns>
        [Pure]
        public float FromFloat(float x) => (float)x;

        /// <summary>
        /// Generate a numeric value from a double
        /// </summary>
        /// <param name="x">The double to use</param>
        /// <returns>The equivalent of x in the Num<A></returns>
        [Pure]
        public float FromDouble(double x) => (float)x;

        /// <summary>
        /// Generates a fractional value from an integer ratio.
        /// </summary>
        /// <param name="x">The ratio to convert</param>
        /// <returns>The equivalent of x in the implementing type.</returns>
        [Pure]
        public float FromRational(Ratio<int> x) => x.Numerator / x.Denominator;

        /// <summary>
        /// Returns an approximation of pi.
        /// </summary>
        /// <returns>A reasonable approximation of pi in this type</returns>
        [Pure]
        public float Pi() => (float)Math.PI;

        /// <summary>
        /// The exponential function.
        /// </summary>
        /// <param name="x">The value for which we are calculating the exponential</param>
        /// <returns>The value of <c>e^x</c></returns>
        [Pure]
        public float Exp(float x) => (float)Math.Exp(x);

        /// <summary>
        /// Calculates the square root of a value.
        /// </summary>
        /// <param name="x">The value for which we are calculating the square root.</param>
        /// <returns>The value of <c>sqrt(x)</c>.</returns>
        [Pure]
        public float Sqrt(float x) => (float)Math.Sqrt(x);

        /// <summary>
        /// Calculates the natural logarithm of a value.
        /// </summary>
        /// <param name="x">
        /// The value for which we are calculating the natural logarithm.
        /// </param>
        /// <returns>The value of <c>ln(x)</c>.</returns>
        [Pure]
        public float Log(float x) => (float)Math.Log(x);

        /// <summary>Raises x to the power y
        /// </summary>
        /// <param name="x">The base to be raised to y</param>
        /// <param name="y">The exponent to which we are raising x</param>
        /// <returns>The value of <c>x^y</c>.</returns>
        [Pure]
        public float Pow(float x, float y) => (float)Math.Pow(x, y);

        /// <summary>
        /// Calculates the logarithm of a value with respect to an arbitrary base.
        /// </summary>
        /// <param name="x">The base to use for the logarithm of t</param>
        /// <param name="y">The value for which we are calculating the logarithm.</param>
        /// <returns>The value of <c>log x (y)</c>.</returns>
        [Pure]
        public float LogBase(float b, float x) => (float)Math.Log(x, b);

        /// <summary>
        /// Calculates the sine of an angle.
        /// </summary>
        /// <param name="x">An angle, in radians</param>
        /// <returns>The value of <c>sin(x)</c></returns>
        [Pure]
        public float Sin(float x) => (float)Math.Sin(x);

        /// <summary>
        /// Calculates the cosine of an angle.
        /// </summary>
        /// <param name="x">An angle, in radians</param>
        /// <returns>The value of <c>cos(x)</c></returns>
        [Pure]
        public float Cos(float x) => (float)Math.Cos(x);

        /// <summary>
        ///     Calculates the tangent of an angle.
        /// </summary>
        /// <param name="x">An angle, in radians</param>
        /// <returns>The value of <c>tan(x)</c></returns>
        [Pure]
        public float Tan(float x) => (float)Math.Tan(x);

        /// <summary>
        /// Calculates an arcsine.
        /// </summary>
        /// <param name="x">The value for which an arcsine is to be calculated.</param>
        /// <returns>The value of <c>asin(x)</c>, in radians.</returns>
        [Pure]
        public float Asin(float x) => (float)Math.Asin(x);

        /// <summary>
        /// Calculates an arc-cosine.
        /// </summary>
        /// <param name="x">The value for which an arc-cosine is to be calculated</param>
        /// <returns>The value of <c>acos(x)</c>, in radians</returns>
        [Pure]
        public float Acos(float x) => (float)Math.Acos(x);

        /// <summary>
        /// Calculates an arc-tangent.
        /// </summary>
        /// <param name="x">The value for which an arc-tangent is to be calculated</param>
        /// <returns>The value of <c>atan(x)</c>, in radians</returns>
        [Pure]
        public float Atan(float x) => (float)Math.Atan(x);

        /// <summary>
        /// Calculates a hyperbolic sine.
        /// </summary>
        /// <param name="x">The value for which a hyperbolic sine is to be calculated</param>
        /// <returns>The value of <c>sinh(x)</c></returns>
        [Pure]
        public float Sinh(float x) => (float)Math.Sinh(x);

        /// <summary>
        /// Calculates a hyperbolic cosine.
        /// </summary>
        /// <param name="x">The value for which a hyperbolic cosine is to be calculated</param>
        /// <returns>The value of <c>cosh(x)</c></returns>
        [Pure]
        public float Cosh(float x) => (float)Math.Cosh(x);

        /// <summary>
        /// Calculates a hyperbolic tangent.
        /// </summary>
        /// <param name="x">
        /// The value for which a hyperbolic tangent is to be calculated.
        /// </param>
        /// <returns>The value of <c>tanh(x)</c></returns>
        [Pure]
        public float Tanh(float x) => (float)Math.Tanh(x);

        /// <summary>Calculates an area hyperbolic sine</summary>
        /// <param name="x">The value for which an area hyperbolic sine is to be calculated.
        /// </param>
        /// <returns>The value of <c>asinh(x)</c>.</returns>
        [Pure]
        public float Asinh(float x) => (float)Math.Log(x + Math.Sqrt((x * x) + 1.0));

        /// <summary>
        /// Calculates an area hyperbolic cosine.
        /// </summary>
        /// <param name="x">The value for which an area hyperbolic cosine is to be calculated.
        /// </param>
        /// <returns>The value of <c>acosh(x)</c>.</returns>
        [Pure]
        public float Acosh(float x) => (float)Math.Log(x + Math.Sqrt((x * x) - 1.0));

        /// <summary>
        /// Calculates an area hyperbolic tangent.
        /// </summary>
        /// <param name="x">The value for which an area hyperbolic tangent is to be calculated.
        /// </param>
        /// <returns>The value of <c>atanh(x)</c></returns>
        [Pure]
        public float Atanh(float x) => 0.5f * (float)Math.Log((1.0 + x) / (1.0 - x));

        /// <summary>
        /// Negate the value
        /// </summary>
        /// <param name="x">Value to negate</param>
        /// <returns>The negated source value</returns>
        [Pure]
        public float Negate(float x) => -x;

        /// <summary>
        /// Semigroup append (sum)
        /// </summary>
        /// <param name="x">left hand side of the append operation</param>
        /// <param name="y">right hand side of the append operation</param>
        /// <returns>x + y</returns>
        [Pure]
        public float Append(float x, float y) => x + y;

        /// <summary>
        /// Zero
        /// </summary>
        [Pure]
        public float Empty() => 0.0f;

        /// <summary>
        /// Get the hash-code of the provided value
        /// </summary>
        /// <returns>Hash code of x</returns>
        [Pure]
        public int GetHashCode(float x) =>
            x.GetHashCode();
        
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<bool> EqualsAsync(float x, float y) =>
            Equals(x, y).AsTask();

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<int> GetHashCodeAsync(float x) =>
            GetHashCode(x).AsTask();         

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<int> CompareAsync(float x, float y) =>
            Compare(x, y).AsTask();
    }
}
