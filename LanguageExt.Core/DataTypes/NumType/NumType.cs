using System;
using LanguageExt.TypeClasses;
using LanguageExt.ClassInstances.Pred;

namespace LanguageExt
{
    /// <summary>
    /// NumType - inspired by Haskell's 'newtype' keyword.  This is setup for numeric
    /// types,and expects a Num<A> class-instance as an argument (TInt, TDouble, etc.)
    /// 
    /// https://wiki.haskell.org/Newtype
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
    /// <typeparam name="NUMTYPE">Self reference type - i.e. class Metres : NumType<Metres, ... ></typeparam>
    /// <typeparam name="NUM">Num of A, e.g. TInt, TDouble, TFloat, etc.</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
#if !COREFX
    [Serializable]
#endif
    public abstract class NumType<NUMTYPE, NUM, A> : NumType<NUMTYPE, NUM, A, True<A>>
        where NUM     : struct, Num<A>
        where NUMTYPE : NumType<NUMTYPE, NUM, A, True<A>>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="value">Value to bind</param>
        /// <exception cref="ArgumentNullException">Null values are not accepted</exception>
        public NumType(A value) : base(value)
        {
        }
    }
}