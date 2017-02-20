using System;
using LanguageExt.ClassInstances;
using LanguageExt.ClassInstances.Pred;
using System.Diagnostics.Contracts;

namespace LanguageExt
{
    /// <summary>
    /// NewType - inspired by Haskell's 'newtype' keyword.
    /// https://wiki.haskell.org/Newtype
    /// Derive type from this one to get: Equatable, Comparable, Appendable, Subtractable, 
    /// Multiplicable, Divisible, Foldable, Functor, Interable: strongly typed values.  
    ///
    /// For example:
    ///
    ///     class Metres : NewType<Metres, double> { public Metres(double x) : base(x) {} }
    ///     class Hours : NewType<Hours, double> { public Hours(double x) : base(x) {} }
    ///
    /// Will not accept null values
    ///
    /// </summary>
    [Serializable]
    public abstract class NewType<NEWTYPE, A> : NewType<NEWTYPE, A, True<A>>
        where NEWTYPE : NewType<NEWTYPE, A, True<A>>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="value">Value to bind</param>
        /// <exception cref="ArgumentNullException">Null values are not accepted</exception>
        public NewType(A value) : base(value) {}

        /// <summary>
        /// Explicit conversion operator for extracting the bound value
        /// </summary>
        [Pure]
        public static explicit operator A(NewType<NEWTYPE, A> type) =>
            type.Value;
    }
}