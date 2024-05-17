using LanguageExt.Traits;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances;

/// <summary>
/// Integer number 
/// </summary>
public struct TBigInt : Num<bigint>, Bool<bigint>
{
    /// <summary>
    /// Equality test
    /// </summary>
    /// <param name="x">The left hand side of the equality operation</param>
    /// <param name="y">The right hand side of the equality operation</param>
    /// <returns>True if x and y are equal</returns>
    [Pure]
    public static bool Equals(bigint x, bigint y) =>
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
    public static int Compare(bigint x, bigint y) =>
        x.CompareTo(y);

    /// <summary>
    /// Find the sum of two numbers
    /// </summary>
    /// <param name="x">left hand side of the addition operation</param>
    /// <param name="y">right hand side of the addition operation</param>
    /// <returns>The sum of x and y</returns>
    [Pure]
    public static bigint Add(bigint x, bigint y) =>
        x + y;

    /// <summary>
    /// Find the difference between two values
    /// </summary>
    /// <param name="x">left hand side of the subtraction operation</param>
    /// <param name="y">right hand side of the subtraction operation</param>
    /// <returns>The difference between x and y</returns>
    [Pure]
    public static bigint Subtract(bigint x, bigint y) =>
        x - y;

    /// <summary>
    /// Find the product of two numbers
    /// </summary>
    /// <param name="x">left hand side of the product operation</param>
    /// <param name="y">right hand side of the product operation</param>
    /// <returns>The product of x and y</returns>
    [Pure]
    public static bigint Multiply(bigint x, bigint y) =>
        x * y;

    /// <summary>
    /// Divide two numbers
    /// </summary>
    /// <param name="x">left hand side of the division operation</param>
    /// <param name="y">right hand side of the division operation</param>
    /// <returns>x / y</returns>
    [Pure]
    public static bigint Divide(bigint x, bigint y) =>
        x / y;

    /// <summary>
    /// Find the absolute value of a number
    /// </summary>
    /// <param name="x">The value to find the absolute value of</param>
    /// <returns>The non-negative absolute value of x</returns>
    [Pure]
    public static bigint Abs(bigint x) =>
        x < bigint.Zero
            ? -x
            : x;

    /// <summary>
    /// Find the sign of x
    /// </summary>
    /// <param name="x">The value to find the sign of</param>
    /// <returns>-1, 0, or +1</returns>
    [Pure]
    public static bigint Signum(bigint x) =>
        x   == bigint.Zero ? 0
        : x < bigint.Zero  ? -1
                             : 1;

    /// <summary>
    /// Generate a numeric value from an integer
    /// </summary>
    /// <param name="x">The integer to use</param>
    /// <returns>The equivalent of x in the Num<A></returns>
    [Pure]
    public static bigint FromInteger(int x) =>
        x;

    /// <summary>
    /// Generate a numeric value from a float
    /// </summary>
    /// <param name="x">The float to use</param>
    /// <returns>The equivalent of x in the Num<A></returns>
    [Pure]
    public static bigint FromDecimal(decimal x) =>
        (bigint)x;

    /// <summary>
    /// Generate a numeric value from a double
    /// </summary>
    /// <param name="x">The double to use</param>
    /// <returns>The equivalent of x in the Num<A></returns>
    [Pure]
    public static bigint FromFloat(float x) =>
        (bigint)x;

    /// <summary>
    /// Generate a numeric value from a decimal
    /// </summary>
    /// <param name="x">The decimal to use</param>
    /// <returns>The equivalent of x in the Num<A></returns>
    [Pure]
    public static bigint FromDouble(double x) =>
        (bigint)x;

    /// <summary>
    /// Monoid empty value (0)
    /// </summary>
    /// <returns>0</returns>
    [Pure]
    public static bigint Empty => bigint.Zero;

    /// <summary>
    /// Negate the value
    /// </summary>
    /// <param name="x">Value to negate</param>
    /// <returns>The negated source value</returns>
    [Pure]
    public static bigint Negate(bigint x) => -x;

    /// <summary>
    /// Semigroup append (sum)
    /// </summary>
    /// <param name="x">left hand side of the append operation</param>
    /// <param name="y">right hand side of the append operation</param>
    /// <returns>x + y</returns>
    [Pure]
    public static bigint Append(bigint x, bigint y) => 
        x + y;

    /// <summary>
    /// Get the hash-code of the provided value
    /// </summary>
    /// <returns>Hash code of x</returns>
    [Pure]
    public static int GetHashCode(bigint x) =>
        x.GetHashCode();

    /// <summary>
    /// Returns True
    /// </summary>
    /// <returns>True</returns>
    [Pure]
    public static bigint True() =>
        bigint.MinusOne;

    /// <summary>
    /// Returns False
    /// </summary>
    /// <returns>False</returns>
    [Pure]
    public static bigint False() =>
        bigint.Zero;

    /// <summary>
    /// Returns the result of the bitwise AND operation between `a` and `b`
    /// </summary>
    /// <returns>The result of the bitwise AND operation between `a` and `b`</returns>
    [Pure]
    public static bigint And(bigint a, bigint b) =>
        a & b;

    /// <summary>
    /// Returns the result of the bitwise OR operation between `a` and `b`
    /// </summary>
    /// <returns>The result of the bitwise OR operation between `a` and `b`</returns>
    [Pure]
    public static bigint Or(bigint a, bigint b) =>
        a | b;

    /// <summary>
    /// Returns the result of the bitwise NOT operation on `a`
    /// </summary>
    /// <returns>The result of the bitwise NOT operation on `a`</returns>
    [Pure]
    public static bigint Not(bigint a) =>
        ~a;

    /// <summary>
    /// Returns the result of the bitwise exclusive-OR operation between `a` and `b`
    /// </summary>
    /// <returns>The result of the bitwise exclusive-OR operation between `a` and `b`</returns>
    [Pure]
    public static bigint XOr(bigint a, bigint b) =>
        a ^ b;

    /// <summary>
    /// Logical implication
    /// </summary>
    /// <returns>If `a` is true that implies `b`, else `true`</returns>
    [Pure]
    public static bigint Implies(bigint a, bigint b) =>
        And(a, True()) == False()
            ? True()
            : b;

    /// <summary>
    /// Bitwise bi-conditional. 
    /// </summary>
    /// <returns>`Not(XOr(a, b))`</returns>
    [Pure]
    public static bigint BiCondition(bigint a, bigint b) =>
        Not(XOr(a, b));
}
