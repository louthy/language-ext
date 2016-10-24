using LanguageExt;
using LanguageExt.TypeClasses;
using static LanguageExt.TypeClass;

namespace LanguageExt.Instances
{
    /// <summary>
    /// Compare the equality of any type in the NewType type-class
    /// </summary>
    public struct EqNewType<NEWTYPE, EQ, A> : Eq<NewType<NEWTYPE, A>>
        where EQ      : struct, Eq<A>
        where NEWTYPE : NewType<NEWTYPE, A>
    {
        public static readonly EqNewType<NEWTYPE, EQ, A> Inst = default(EqNewType<NEWTYPE, EQ, A>);

        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        public bool Equals(NewType<NEWTYPE, A> x, NewType<NEWTYPE, A> y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            return default(EQ).Equals(x.Value, y.Value);
        }
    }

    /// <summary>
    /// Compare the equality of any type in the NewType type-class
    /// </summary>
    public struct EqNewType<NEWTYPE, SEMI, ORD, EQ, A> : Eq<NewType<NEWTYPE, SEMI, ORD, A>>
        where ORD     : struct, Ord<A>
        where SEMI    : struct, Semigroup<A>
        where EQ      : struct, Eq<A>
        where NEWTYPE : NewType<NEWTYPE, SEMI, ORD, A>
    {
        public static readonly EqNewType<NEWTYPE, SEMI, ORD, EQ, A> Inst = default(EqNewType<NEWTYPE, SEMI, ORD, EQ, A>);

        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        public bool Equals(NewType<NEWTYPE, SEMI, ORD, A> x, NewType<NEWTYPE, SEMI, ORD, A> y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            return default(EQ).Equals(x.Value, y.Value);
        }
    }

    /// <summary>
    /// Compare the equality of any type in the NewType type-class
    /// </summary>
    public struct EqNewType<NEWTYPE, NUM, EQ, A> : Eq<NewType<NEWTYPE, NUM, A>>
        where NUM     : struct, Num<A>
        where EQ      : struct, Eq<A>
        where NEWTYPE : NewType<NEWTYPE, NUM, A>
    {
        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        public bool Equals(NewType<NEWTYPE, NUM, A> x, NewType<NEWTYPE, NUM, A> y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            return default(EQ).Equals(x.Value, y.Value);
        }
    }
}
