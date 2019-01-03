using System;
using LanguageExt.ClassInstances;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using LanguageExt.TypeClasses;
using System.Runtime.Serialization;

namespace LanguageExt
{
    /// <summary>
    /// NewType - inspired by Haskell's 'newtype' keyword.
    /// https://wiki.haskell.org/Newtype
    /// Derive type from this one to get: Equatable, Comparable, Appendable, Subtractable, 
    /// Multiplicable, Divisible, Foldable, Monadic, Functor, Interable: strongly typed values.  
    ///
    /// For example:
    ///
    ///     class Metres : NewType<Metres, double> { public Metres(double x) : base(x) {} }
    ///     class Hours : NewType<Hours, double> { public Hours(double x) : base(x) {} }
    ///
    /// Will not accept null values
    ///
    /// </summary>
    /// <typeparam name="NEWTYPE">Implementing class</typeparam>
    /// <typeparam name="A">Inner bound value type</typeparam>
    /// <typeparam name="PRED">Predicate instance to run when the type is constructed</typeparam>
    [Serializable]
    public abstract class NewType<NEWTYPE, A, PRED> : NewType<NEWTYPE, A, PRED, OrdDefault<A>>
        where PRED    : struct, Pred<A>
        where NEWTYPE : NewType<NEWTYPE, A, PRED>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="value">Value to bind</param>
        /// <exception cref="ArgumentNullException">Null values are not accepted</exception>
        public NewType(A value) : base(value) {}

        /// <summary>
        /// Deserialisation ctor
        /// </summary>
        protected NewType(SerializationInfo info, StreamingContext context) : base((A)info.GetValue("Value", typeof(A)))
        { }

        public override void GetObjectData(SerializationInfo info, StreamingContext context) =>
            info.AddValue("Value", Value);

        /// <summary>
        /// Explicit conversion operator for extracting the bound value
        /// </summary>
        [Pure]
        public static explicit operator A(NewType<NEWTYPE, A, PRED> type) =>
            ReferenceEquals(type, null)
                ? throw new ArgumentException($"Can't explicitly convert from a null {typeof(NEWTYPE).Name}")
                : type.Value;
    }
}
