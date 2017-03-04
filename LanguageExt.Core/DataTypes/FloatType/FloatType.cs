using System;
using LanguageExt.TypeClasses;
using LanguageExt.ClassInstances.Pred;
using System.Diagnostics.Contracts;

namespace LanguageExt
{
    /// <summary>
    /// FloatType - inspired by Haskell's 'newtype' keyword.  This is setup for floating point
    /// numeric types,and expects a Floating<A> class-instance as an argument (TFloat, TDouble, 
    /// TDecimal, etc.)
    /// 
    /// https://wiki.haskell.org/Newtype
    /// 
    /// Derive type from this one to get: Equatable, Comparable, Appendable, Foldable, 
    /// Functor, Iterable: strongly typed values.
    ///
    /// For example:
    ///
    ///     class Metres : FloatType<Metres, TDouble, double> { public Metres(double x) : base(x) {} }
    ///
    /// Will not accept null values
    /// </summary>
    /// <typeparam name="SELF">Self reference type - i.e. class Metres : FloatType<Metres, ... ></typeparam>
    /// <typeparam name="FLOATING">Floating of A, e.g. TDouble, TFloat, etc.</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    [Serializable]
    public abstract class FloatType<SELF, FLOATING, A> : FloatType<SELF, FLOATING, A, True<A>>
        where FLOATING     : struct, Floating<A>
        where SELF : FloatType<SELF, FLOATING, A, True<A>>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="value">Value to bind</param>
        /// <exception cref="ArgumentNullException">Null values are not accepted</exception>
        public FloatType(A value) : base(value)
        {
        }

        /// <summary>
        /// Explicit conversion operator for extracting the bound value
        /// </summary>
        [Pure]
        public static explicit operator A(FloatType<SELF, FLOATING, A> type) =>
            type.Value;
    }
}