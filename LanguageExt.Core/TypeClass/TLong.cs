using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageExt.TypeClass
{
    /// <summary>
    /// Long integer number
    /// </summary>
    public struct TLong : Ord<long>, Num<long>
    {
        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        public bool Equals(long x, long y) =>
            x == y;

        /// <summary>
        /// Returns true if x is greater than y
        /// </summary>
        /// <param name="x">The first item to compare</param>
        /// <param name="y">The second item to compare</param>
        /// <returns>True if x is greater than y
        public bool GreaterThan(long x, long y) =>
            x > y;

        /// <summary>
        /// Returns true if x is greater than or equal to y
        /// </summary>
        /// <param name="x">The first item to compare</param>
        /// <param name="y">The second item to compare</param>
        /// <returns>True if x is greater than or equal to y
        public bool GreaterOrEq(long x, long y) =>
            x >= y;

        /// <summary>
        /// Returns true if x is less than y
        /// </summary>
        /// <param name="x">The first item to compare</param>
        /// <param name="y">The second item to compare</param>
        /// <returns>True if x is less than y
        public bool LessThan(long x, long y) =>
            x < y;

        /// <summary>
        /// Returns true if x is less than or equal to y
        /// </summary>
        /// <param name="x">The first item to compare</param>
        /// <param name="y">The second item to compare</param>
        /// <returns>True if x is less than or equal to y
        public bool LessOrEq(long x, long y) =>
            x <= y;

        /// <summary>
        /// Find the sum of two numbers
        /// </summary>
        /// <param name="x">left hand side of the addition operation</param>
        /// <param name="y">right hand side of the addition operation</param>
        /// <returns>The sum of x and y</returns>
        public long Add(long x, long y) =>
            x + y;

        /// <summary>
        /// Find the difference between two numbers
        /// </summary>
        /// <param name="x">left hand side of the subtraction operation</param>
        /// <param name="y">right hand side of the subtraction operation</param>
        /// <returns>The sum difference between x and y</returns>
        public long Difference(long x, long y) =>
            x - y;

        /// <summary>
        /// Fund the product of two numbers
        /// </summary>
        /// <param name="x">left hand side of the product operation</param>
        /// <param name="y">right hand side of the product operation</param>
        /// <returns>The product of x and y</returns>
        public long Product(long x, long y) =>
            x * y;

        /// <summary>
        /// Divide x by y
        /// </summary>
        /// <param name="x">left hand side of the division operation</param>
        /// <param name="y">right hand side of the division operation</param>
        /// <returns>x / y</returns>
        public long Divide(long x, long y) =>
            x / y;

        /// <summary>
        /// Find the absolute value of a number
        /// </summary>
        /// <param name="x">The value to find the absolute value of</param>
        /// <returns>The non-negative absolute value of x</returns>
        public long Abs(long x) =>
            Math.Abs(x);

        /// <summary>
        /// Find the sign of x
        /// </summary>
        /// <param name="x">The value to find the sign of</param>
        /// <returns>-1, 0, or +1</returns>
        public long Signum(long x) =>
            Math.Sign(x);

        /// <summary>
        /// Generate a numeric value from an integer
        /// </summary>
        /// <param name="x">The integer to use</param>
        /// <returns>The equivalent of x in the Num<A></returns>
        public long FromInteger(int x) =>
            x;

        /// <summary>
        /// Generate a numeric value from a float
        /// </summary>
        /// <param name="x">The float to use</param>
        /// <returns>The equivalent of x in the Num<A></returns>
        public long FromDecimal(decimal x) =>
            (long)x;

        /// <summary>
        /// Generate a numeric value from a double
        /// </summary>
        /// <param name="x">The double to use</param>
        /// <returns>The equivalent of x in the Num<A></returns>
        public long FromFloat(float x) =>
            (long)x;

        /// <summary>
        /// Generate a numeric value from a decimal
        /// </summary>
        /// <param name="x">The decimal to use</param>
        /// <returns>The equivalent of x in the Num<A></returns>
        public long FromDouble(double x) =>
            (long)x;
    }
}
