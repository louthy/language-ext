using System;
using LanguageExt.TypeClasses;
using LanguageExt.ClassInstances.Pred;
using System.Diagnostics.Contracts;
using System.Runtime.Serialization;

namespace LanguageExt
{
    /// <summary>
    /// NumType - inspired by Haskell's 'newtype' keyword.  This is setup for numeric
    /// types,and expects a Num<A> class-instance as an argument (TInt, TDouble, etc.)
    /// 
    /// [wiki.haskell.org/Newtype](https://wiki.haskell.org/Newtype)
    /// 
    /// Derive type from this one to get: Equatable, Comparable, Appendable, Foldable, 
    /// Functor, Iterable: strongly typed values.
    ///
    /// For example:
    ///
    ///     class Metres : NumType<Metres, TDouble, double> { public Metres(double x) : base(x) {} }
    ///
    /// Will not accept null values
    /// </summary>
    /// <typeparam name="SELF">Self reference type - i.e. class Metres : NumType<Metres, ... ></typeparam>
    /// <typeparam name="NUM">Num of A, e.g. TInt, TDouble, TFloat, etc.</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    [Serializable]
    public abstract class NumType<SELF, NUM, A> : NumType<SELF, NUM, A, True<A>>
        where NUM  : struct, Num<A>
        where SELF : NumType<SELF, NUM, A>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="value">Value to bind</param>
        /// <exception cref="ArgumentNullException">Null values are not accepted</exception>
        public NumType(A value) : base(value)
        {
        }


        /// <summary>
        /// Deserialisation ctor
        /// </summary>
        protected NumType(SerializationInfo info, StreamingContext context) : base((A)info.GetValue("Value", typeof(A)))
        {
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context) =>
            info.AddValue("Value", Value);

        /// <summary>
        /// Explicit conversion operator for extracting the bound value
        /// </summary>
        [Pure]
        public static explicit operator A(NumType<SELF, NUM, A> type) =>
            ReferenceEquals(type,null)
                ? default(NUM).Empty()
                : type.Value;
    }
}
