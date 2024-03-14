using System;
using LanguageExt.Traits;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances;

/// <summary>
/// Integer number 
/// </summary>
public struct TInt : Num<int>, Bool<int>
{
    /// <summary>
    /// Equality test
    /// </summary>
    /// <param name="x">The left hand side of the equality operation</param>
    /// <param name="y">The right hand side of the equality operation</param>
    /// <returns>True if x and y are equal</returns>
    [Pure]
    public static bool Equals(int x, int y) =>
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
    public static int Compare(int x, int y) =>
        x.CompareTo(y);

    /// <summary>
    /// Find the sum of two numbers
    /// </summary>
    /// <param name="x">left hand side of the addition operation</param>
    /// <param name="y">right hand side of the addition operation</param>
    /// <returns>The sum of x and y</returns>
    [Pure]
    public static int Add(int x, int y) =>
        x + y;

    /// <summary>
    /// Find the difference between two values
    /// </summary>
    /// <param name="x">left hand side of the subtraction operation</param>
    /// <param name="y">right hand side of the subtraction operation</param>
    /// <returns>The difference between x and y</returns>
    [Pure]
    public static int Subtract(int x, int y) =>
        x - y;

    /// <summary>
    /// Find the product of two numbers
    /// </summary>
    /// <param name="x">left hand side of the product operation</param>
    /// <param name="y">right hand side of the product operation</param>
    /// <returns>The product of x and y</returns>
    [Pure]
    public static int Multiply(int x, int y) =>
        x * y;

    /// <summary>
    /// Divide two numbers
    /// </summary>
    /// <param name="x">left hand side of the division operation</param>
    /// <param name="y">right hand side of the division operation</param>
    /// <returns>x / y</returns>
    [Pure]
    public static int Divide(int x, int y) =>
        x / y;

    /// <summary>
    /// Find the absolute value of a number
    /// </summary>
    /// <param name="x">The value to find the absolute value of</param>
    /// <returns>The non-negative absolute value of x</returns>
    [Pure]
    public static int Abs(int x) =>
        Math.Abs(x);

    /// <summary>
    /// Find the sign of x
    /// </summary>
    /// <param name="x">The value to find the sign of</param>
    /// <returns>-1, 0, or +1</returns>
    [Pure]
    public static int Signum(int x) =>
        Math.Sign(x);

    /// <summary>
    /// Generate a numeric value from an integer
    /// </summary>
    /// <param name="x">The integer to use</param>
    /// <returns>The equivalent of x in the Num<A></returns>
    [Pure]
    public static int FromInteger(int x) =>
        x;

    /// <summary>
    /// Generate a numeric value from a float
    /// </summary>
    /// <param name="x">The float to use</param>
    /// <returns>The equivalent of x in the Num<A></returns>
    [Pure]
    public static int FromDecimal(decimal x) =>
        (int)x;

    /// <summary>
    /// Generate a numeric value from a double
    /// </summary>
    /// <param name="x">The double to use</param>
    /// <returns>The equivalent of x in the Num<A></returns>
    [Pure]
    public static int FromFloat(float x) =>
        (int)x;

    /// <summary>
    /// Generate a numeric value from a decimal
    /// </summary>
    /// <param name="x">The decimal to use</param>
    /// <returns>The equivalent of x in the Num<A></returns>
    [Pure]
    public static int FromDouble(double x) =>
        (int)x;

    /// <summary>
    /// Negate the value
    /// </summary>
    /// <param name="x">Value to negate</param>
    /// <returns>The negated source value</returns>
    [Pure]
    public static int Negate(int x) => -x;

    /// <summary>
    /// Get the hash-code of the provided value
    /// </summary>
    /// <returns>Hash code of x</returns>
    [Pure]
    public static int GetHashCode(int x) =>
        x.GetHashCode();

    /// <summary>
    /// Returns True
    /// </summary>
    /// <returns>True</returns>
    [Pure]
    public static int True() =>
        -1;

    /// <summary>
    /// Returns False
    /// </summary>
    /// <returns>False</returns>
    [Pure]
    public static int False() =>
        0;

    /// <summary>
    /// Returns the result of the bitwise AND operation between `a` and `b`
    /// </summary>
    /// <returns>The result of the bitwise AND operation between `a` and `b`</returns>
    [Pure]
    public static int And(int a, int b) =>
        a & b;

    /// <summary>
    /// Returns the result of the bitwise OR operation between `a` and `b`
    /// </summary>
    /// <returns>The result of the bitwise OR operation between `a` and `b`</returns>
    [Pure]
    public static int Or(int a, int b) =>
        a | b;

    /// <summary>
    /// Returns the result of the bitwise NOT operation on `a`
    /// </summary>
    /// <returns>The result of the bitwise NOT operation on `a`</returns>
    [Pure]
    public static int Not(int a) =>
        ~a;

    /// <summary>
    /// Returns the result of the bitwise exclusive-OR operation between `a` and `b`
    /// </summary>
    /// <returns>The result of the bitwise exclusive-OR operation between `a` and `b`</returns>
    [Pure]
    public static int XOr(int a, int b) =>
        a ^ b;

    /// <summary>
    /// Logical implication
    /// </summary>
    /// <returns>If `a` is true that implies `b`, else `true`</returns>
    [Pure]
    public static int Implies(int a, int b) =>
        And(a, True()) == False()
            ? True()
            : b;

    /// <summary>
    /// Bitwise bi-conditional. 
    /// </summary>
    /// <returns>`Not(XOr(a, b))`</returns>
    [Pure]
    public static int BiCondition(int a, int b) =>
        Not(XOr(a, b));
}
