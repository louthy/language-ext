using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;

namespace LanguageExt;

public static partial class Trait
{
    /// <summary>
    /// Divide two numbers
    /// </summary>
    /// <param name="x">left hand side of the division operation</param>
    /// <param name="y">right hand side of the division operation</param>
    /// <returns>x / y</returns>
    [Pure]
    public static A divide<NUM, A>(A x, A y) where NUM : Num<A> =>
        NUM.Divide(x, y);


    /// <summary>
    /// Find the absolute value of a number
    /// </summary>
    /// <param name="x">The value to find the absolute value of</param>
    /// <returns>The non-negative absolute value of x</returns>
    [Pure]
    public static A abs<NUM, A>(A x) where NUM : Num<A> =>
        NUM.Abs(x);

    /// <summary>
    /// Find the sign of x
    /// </summary>
    /// <param name="x">The value to find the sign of</param>
    /// <returns>-1, 0, or +1</returns>
    [Pure]
    public static A signum<NUM, A>(A x) where NUM : Num<A> =>
        NUM.Signum(x);

    /// <summary>
    /// Generate a numeric value from an integer
    /// </summary>
    /// <param name="x">The integer to use</param>
    /// <returns>The equivalent of x in the Num<A></returns>
    [Pure]
    public static A fromInteger<NUM, A>(int x) where NUM : Num<A> =>
        NUM.FromInteger(x);

    /// <summary>
    /// Generate a numeric value from a float
    /// </summary>
    /// <param name="x">The float to use</param>
    /// <returns>The equivalent of x in the Num<A></returns>
    [Pure]
    public static A fromDecimal<NUM, A>(decimal x) where NUM : Num<A> =>
        NUM.FromDecimal(x);

    /// <summary>
    /// Generate a numeric value from a double
    /// </summary>
    /// <param name="x">The double to use</param>
    /// <returns>The equivalent of x in the Num<A></returns>
    [Pure]
    public static A fromFloat<NUM, A>(float x) where NUM : Num<A> =>
        NUM.FromFloat(x);

    /// <summary>
    /// Generate a numeric value from a decimal
    /// </summary>
    /// <param name="x">The decimal to use</param>
    /// <returns>The equivalent of x in the Num<A></returns>
    [Pure]
    public static A fromDouble<NUM, A>(double x) where NUM : Num<A> =>
        NUM.FromDouble(x);
}
