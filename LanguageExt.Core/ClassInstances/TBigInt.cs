using System;
using LanguageExt.TypeClasses;
using System.Numerics;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Integer number 
    /// </summary>
    public struct TBigInt : Num<BigInteger>, Monoid<BigInteger>
    {
        public static readonly TBigInt Inst = default(TBigInt);

        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        public bool Equals(BigInteger x, BigInteger y) =>
            x == y;

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
        public int Compare(BigInteger x, BigInteger y) =>
            x.CompareTo(y);

        /// <summary>
        /// Find the sum of two numbers
        /// </summary>
        /// <param name="x">left hand side of the addition operation</param>
        /// <param name="y">right hand side of the addition operation</param>
        /// <returns>The sum of x and y</returns>
        public BigInteger Plus(BigInteger x, BigInteger y) =>
            x + y;

        /// <summary>
        /// Find the difference between two values
        /// </summary>
        /// <param name="x">left hand side of the subtraction operation</param>
        /// <param name="y">right hand side of the subtraction operation</param>
        /// <returns>The difference between x and y</returns>
        public BigInteger Subtract(BigInteger x, BigInteger y) =>
            x - y;

        /// <summary>
        /// Find the product of two numbers
        /// </summary>
        /// <param name="x">left hand side of the product operation</param>
        /// <param name="y">right hand side of the product operation</param>
        /// <returns>The product of x and y</returns>
        public BigInteger Product(BigInteger x, BigInteger y) =>
            x * y;

        /// <summary>
        /// Divide two numbers
        /// </summary>
        /// <param name="x">left hand side of the division operation</param>
        /// <param name="y">right hand side of the division operation</param>
        /// <returns>x / y</returns>
        public BigInteger Divide(BigInteger x, BigInteger y) =>
            x / y;

        /// <summary>
        /// Find the absolute value of a number
        /// </summary>
        /// <param name="x">The value to find the absolute value of</param>
        /// <returns>The non-negative absolute value of x</returns>
        public BigInteger Abs(BigInteger x) =>
            x < BigInteger.Zero
                ? -x
                : x;

        /// <summary>
        /// Find the sign of x
        /// </summary>
        /// <param name="x">The value to find the sign of</param>
        /// <returns>-1, 0, or +1</returns>
        public BigInteger Signum(BigInteger x) =>
            x == BigInteger.Zero ? 0
          : x <  BigInteger.Zero ? -1
          : 1;

        /// <summary>
        /// Generate a numeric value from an integer
        /// </summary>
        /// <param name="x">The integer to use</param>
        /// <returns>The equivalent of x in the Num<A></returns>
        public BigInteger FromInteger(int x) =>
            (BigInteger)x;

        /// <summary>
        /// Generate a numeric value from a float
        /// </summary>
        /// <param name="x">The float to use</param>
        /// <returns>The equivalent of x in the Num<A></returns>
        public BigInteger FromDecimal(decimal x) =>
            (BigInteger)x;

        /// <summary>
        /// Generate a numeric value from a double
        /// </summary>
        /// <param name="x">The double to use</param>
        /// <returns>The equivalent of x in the Num<A></returns>
        public BigInteger FromFloat(float x) =>
            (BigInteger)x;

        /// <summary>
        /// Generate a numeric value from a decimal
        /// </summary>
        /// <param name="x">The decimal to use</param>
        /// <returns>The equivalent of x in the Num<A></returns>
        public BigInteger FromDouble(double x) =>
            (BigInteger)x;

        /// <summary>
        /// Monoid empty value (0)
        /// </summary>
        /// <returns>0</returns>
        public BigInteger Empty() => 0;

        /// <summary>
        /// Negate the value
        /// </summary>
        /// <param name="x">Value to negate</param>
        /// <returns>The negated source value</returns>
        public BigInteger Negate(BigInteger x) => -x;

        /// <summary>
        /// Semigroup append (sum)
        /// </summary>
        /// <param name="x">left hand side of the append operation</param>
        /// <param name="y">right hand side of the append operation</param>
        /// <returns>x + y</returns>
        public BigInteger Append(BigInteger x, BigInteger y) => 
            x + y;

        /// <summary>
        /// Get the hash-code of the provided value
        /// </summary>
        /// <returns>Hash code of x</returns>
        public int GetHashCode(BigInteger x) =>
            x.GetHashCode();
    }
}
