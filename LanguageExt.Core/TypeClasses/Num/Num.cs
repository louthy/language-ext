using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageExt.TypeClasses
{
    /// <summary>
    /// Numerical value type-class
    /// </summary>
    /// <typeparam name="A">The type for which the number operations are
    /// defined.</typeparam>
    [Typeclass]
    public interface Num<A> : Addition<A>, Difference<A>, Product<A>, Divisible<A>
    {
        /// <summary>
        /// Find the absolute value of a number
        /// </summary>
        /// <param name="x">The value to find the absolute value of</param>
        /// <returns>The non-negative absolute value of x</returns>
        A Abs(A x);

        /// <summary>
        /// Find the sign of x
        /// </summary>
        /// <param name="x">The value to find the sign of</param>
        /// <returns>-1, 0, or +1</returns>
        A Signum(A x);

        /// <summary>
        /// Generate a numeric value from an integer
        /// </summary>
        /// <param name="x">The integer to use</param>
        /// <returns>The equivalent of x in the Num<A></returns>
        A FromInteger(int x);

        /// <summary>
        /// Generate a numeric value from a float
        /// </summary>
        /// <param name="x">The float to use</param>
        /// <returns>The equivalent of x in the Num<A></returns>
        A FromDecimal(decimal x);

        /// <summary>
        /// Generate a numeric value from a double
        /// </summary>
        /// <param name="x">The double to use</param>
        /// <returns>The equivalent of x in the Num<A></returns>
        A FromFloat(float x);

        /// <summary>
        /// Generate a numeric value from a decimal
        /// </summary>
        /// <param name="x">The decimal to use</param>
        /// <returns>The equivalent of x in the Num<A></returns>
        A FromDouble(double x);
    }

    /// <summary>
    /// Addition operation type-class
    /// </summary>
    /// <typeparam name="A">The type for which the operation is defined.</typeparam>
    [Typeclass]
    public interface Addition<A>
    {
        /// <summary>
        /// Find the sum of two values
        /// </summary>
        /// <param name="x">left hand side of the addition operation</param>
        /// <param name="y">right hand side of the addition operation</param>
        /// <returns>The sum of x and y</returns>
        A Add(A x, A y);
    }

    /// <summary>
    /// Difference operation type-class
    /// </summary>
    /// <typeparam name="A">The type for which the operation is defined.</typeparam>
    [Typeclass]
    public interface Difference<A>
    {
        /// <summary>
        /// Find the difference between two values
        /// </summary>
        /// <param name="x">left hand side of the subtraction operation</param>
        /// <param name="y">right hand side of the subtraction operation</param>
        /// <returns>The sum difference between x and y</returns>
        A Difference(A x, A y);
    }

    /// <summary>
    /// Product operation type-class
    /// </summary>
    /// <typeparam name="A">The type for which the operation is defined.</typeparam>
    [Typeclass]
    public interface Product<A>
    {
        /// <summary>
        /// Find the product of two values
        /// </summary>
        /// <param name="x">left hand side of the product operation</param>
        /// <param name="y">right hand side of the product operation</param>
        /// <returns>The product of x and y</returns>
        A Product(A x, A y);
    }

    /// <summary>
    /// Division operation type-class
    /// </summary>
    /// <typeparam name="A">The type for which the operation is defined.</typeparam>
    [Typeclass]
    public interface Divisible<A>
    {
        /// <summary>
        /// Divide two numbers
        /// </summary>
        /// <param name="x">left hand side of the division operation</param>
        /// <param name="y">right hand side of the division operation</param>
        /// <returns>x / y</returns>
        A Divide(A x, A y);
    }
}
