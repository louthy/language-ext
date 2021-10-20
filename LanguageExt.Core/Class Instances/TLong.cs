using System;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Long integer number
    /// </summary>
    public struct TLong : Num<long>, Bool<long>
    {
        public static readonly TLong Inst = default(TLong);

        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        [Pure]
        public bool Equals(long x, long y) =>
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
        [Pure]
        public int Compare(long x, long y) =>
            x.CompareTo(y);

        /// <summary>
        /// Find the sum of two numbers
        /// </summary>
        /// <param name="x">left hand side of the addition operation</param>
        /// <param name="y">right hand side of the addition operation</param>
        /// <returns>The sum of x and y</returns>
        [Pure]
        public long Plus(long x, long y) =>
            x + y;

        /// <summary>
        /// Find the difference between two values
        /// </summary>
        /// <param name="x">left hand side of the subtraction operation</param>
        /// <param name="y">right hand side of the subtraction operation</param>
        /// <returns>The difference between x and y</returns>
        [Pure]
        public long Subtract(long x, long y) =>
            x - y;

        /// <summary>
        /// Fund the product of two numbers
        /// </summary>
        /// <param name="x">left hand side of the product operation</param>
        /// <param name="y">right hand side of the product operation</param>
        /// <returns>The product of x and y</returns>
        [Pure]
        public long Product(long x, long y) =>
            x * y;

        /// <summary>
        /// Divide x by y
        /// </summary>
        /// <param name="x">left hand side of the division operation</param>
        /// <param name="y">right hand side of the division operation</param>
        /// <returns>x / y</returns>
        [Pure]
        public long Divide(long x, long y) =>
            x / y;

        /// <summary>
        /// Find the absolute value of a number
        /// </summary>
        /// <param name="x">The value to find the absolute value of</param>
        /// <returns>The non-negative absolute value of x</returns>
        [Pure]
        public long Abs(long x) =>
            Math.Abs(x);

        /// <summary>
        /// Find the sign of x
        /// </summary>
        /// <param name="x">The value to find the sign of</param>
        /// <returns>-1, 0, or +1</returns>
        [Pure]
        public long Signum(long x) =>
            Math.Sign(x);

        /// <summary>
        /// Generate a numeric value from an integer
        /// </summary>
        /// <param name="x">The integer to use</param>
        /// <returns>The equivalent of x in the Num<A></returns>
        [Pure]
        public long FromInteger(int x) =>
            x;

        /// <summary>
        /// Generate a numeric value from a float
        /// </summary>
        /// <param name="x">The float to use</param>
        /// <returns>The equivalent of x in the Num<A></returns>
        [Pure]
        public long FromDecimal(decimal x) =>
            (long)x;

        /// <summary>
        /// Generate a numeric value from a double
        /// </summary>
        /// <param name="x">The double to use</param>
        /// <returns>The equivalent of x in the Num<A></returns>
        [Pure]
        public long FromFloat(float x) =>
            (long)x;

        /// <summary>
        /// Generate a numeric value from a decimal
        /// </summary>
        /// <param name="x">The decimal to use</param>
        /// <returns>The equivalent of x in the Num<A></returns>
        [Pure]
        public long FromDouble(double x) =>
            (long)x;

        /// <summary>
        /// Semigroup append (sum)
        /// </summary>
        /// <param name="x">left hand side of the append operation</param>
        /// <param name="y">right hand side of the append operation</param>
        /// <returns>x + y</returns>
        [Pure]
        public long Append(long x, long y) => 
            x + y;

        /// <summary>
        /// Negate the value
        /// </summary>
        /// <param name="x">Value to negate</param>
        /// <returns>The negated source value</returns>
        [Pure]
        public long Negate(long x) => -x;

        /// <summary>
        /// Zero
        /// </summary>
        [Pure]
        public long Empty() => 0;

        /// <summary>
        /// Get the hash-code of the provided value
        /// </summary>
        /// <returns>Hash code of x</returns>
        [Pure]
        public int GetHashCode(long x) =>
            x.GetHashCode();

        /// <summary>
        /// Returns True
        /// </summary>
        /// <returns>True</returns>
        [Pure]
        public long True() =>
            -1;

        /// <summary>
        /// Returns False
        /// </summary>
        /// <returns>False</returns>
        [Pure]
        public long False() =>
            0;

        /// <summary>
        /// Returns the result of the bitwise AND operation between `a` and `b`
        /// </summary>
        /// <returns>The result of the bitwise AND operation between `a` and `b`</returns>
        [Pure]
        public long And(long a, long b) =>
            a & b;

        /// <summary>
        /// Returns the result of the bitwise OR operation between `a` and `b`
        /// </summary>
        /// <returns>The result of the bitwise OR operation between `a` and `b`</returns>
        [Pure]
        public long Or(long a, long b) =>
            a | b;

        /// <summary>
        /// Returns the result of the bitwise NOT operation on `a`
        /// </summary>
        /// <returns>The result of the bitwise NOT operation on `a`</returns>
        [Pure]
        public long Not(long a) =>
            ~a;

        /// <summary>
        /// Returns the result of the bitwise exclusive-OR operation between `a` and `b`
        /// </summary>
        /// <returns>The result of the bitwise exclusive-OR operation between `a` and `b`</returns>
        [Pure]
        public long XOr(long a, long b) =>
            a ^ b;

        /// <summary>
        /// Logical implication
        /// </summary>
        /// <returns>If `a` is true that implies `b`, else `true`</returns>
        [Pure]
        public long Implies(long a, long b) =>
            And(a, True()) == False()
                ? True()
                : b;

        /// <summary>
        /// Bitwise bi-conditional. 
        /// </summary>
        /// <returns>`Not(XOr(a, b))`</returns>
        [Pure]
        public long BiCondition(long a, long b) =>
            Not(XOr(a, b));
        
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<bool> EqualsAsync(long x, long y) =>
            Equals(x, y).AsTask();

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<int> GetHashCodeAsync(long x) =>
            GetHashCode(x).AsTask();         
          
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<int> CompareAsync(long x, long y) =>
            Compare(x, y).AsTask();   
    }
}
