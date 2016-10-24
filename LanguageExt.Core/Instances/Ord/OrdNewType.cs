using LanguageExt.TypeClasses;
using static LanguageExt.TypeClass;

namespace LanguageExt.Instances
{
    /// <summary>
    /// Compare the equality and ordering of any type in the NewType
    /// type-class
    /// </summary>
    public struct OrdNewType<NEWTYPE, ORD, A> : Ord<NewType<NEWTYPE, A>>
        where ORD     : struct, Ord<A>
        where NEWTYPE : NewType<NEWTYPE, A>
    {
        public static readonly OrdNewType<NEWTYPE, ORD, A> Inst = default(OrdNewType<NEWTYPE, ORD, A>);

        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        public bool Equals(NewType<NEWTYPE, A> x, NewType<NEWTYPE, A> y) =>
            default(EqNewType<NEWTYPE, ORD, A>).Equals(x, y);

        /// <summary>
        /// Compare two values
        /// </summary>
        /// <param name="x">Left hand side of the compare operation</param>
        /// <param name="y">Right hand side of the compare operation</param>
        /// <returns>
        /// if x greater than y : 1
        /// if x less than y    : -1
        /// if x equals y       : 0
        /// </returns>
        public int Compare(NewType<NEWTYPE, A> mx, NewType<NEWTYPE, A> my)
        {
            if (ReferenceEquals(mx, my)) return 0;
            if (ReferenceEquals(mx, null)) return -1;
            if (ReferenceEquals(my, null)) return 1;
            return default(ORD).Compare(mx.Value, my.Value);
        }
    }

    /// <summary>
    /// Compare the equality and ordering of any type in the NewType
    /// type-class
    /// </summary>
    public struct OrdNewTypeNum<NEWTYPE, NUM, A> : Ord<NewType<NEWTYPE, NUM, A>>
        where NUM : struct, Num<A>
        where NEWTYPE : NewType<NEWTYPE, NUM, A>
    {
        public static readonly OrdNewTypeNum<NEWTYPE, NUM, A> Inst = default(OrdNewTypeNum<NEWTYPE, NUM, A>);

        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        public bool Equals(NewType<NEWTYPE, NUM, A> x, NewType<NEWTYPE, NUM, A> y) =>
            default(EqNewType<NEWTYPE, NUM, NUM, A>).Equals(x, y);

        /// <summary>
        /// Compare two values
        /// </summary>
        /// <param name="x">Left hand side of the compare operation</param>
        /// <param name="y">Right hand side of the compare operation</param>
        /// <returns>
        /// if x greater than y : 1
        /// if x less than y    : -1
        /// if x equals y       : 0
        /// </returns>
        public int Compare(NewType<NEWTYPE, NUM, A> mx, NewType<NEWTYPE, NUM, A> my)
        {
            if (ReferenceEquals(mx, my)) return 0;
            if (ReferenceEquals(mx, null)) return -1;
            if (ReferenceEquals(my, null)) return 1;
            return default(NUM).Compare(mx.Value, my.Value);
        }
    }
    /// <summary>
    /// Compare the equality and ordering of any type in the NewType
    /// type-class
    /// </summary>
    public struct OrdNewType<NEWTYPE, SEMI, ORD, A> : Ord<NewType<NEWTYPE, SEMI, ORD, A>>
        where SEMI : struct, Semigroup<A>
        where ORD  : struct, Ord<A>
        where NEWTYPE : NewType<NEWTYPE, SEMI, ORD, A>
    {
        public static readonly OrdNewType<NEWTYPE, SEMI, ORD, A> Inst = default(OrdNewType<NEWTYPE, SEMI, ORD, A>);

        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        public bool Equals(NewType<NEWTYPE, SEMI, ORD, A> x, NewType<NEWTYPE, SEMI, ORD, A> y) =>
            default(EqNewType<NEWTYPE, SEMI, ORD, ORD, A>).Equals(x, y);

        /// <summary>
        /// Compare two values
        /// </summary>
        /// <param name="x">Left hand side of the compare operation</param>
        /// <param name="y">Right hand side of the compare operation</param>
        /// <returns>
        /// if x greater than y : 1
        /// if x less than y    : -1
        /// if x equals y       : 0
        /// </returns>
        public int Compare(NewType<NEWTYPE, SEMI, ORD, A> mx, NewType<NEWTYPE, SEMI, ORD, A> my)
        {
            if (ReferenceEquals(mx, my)) return 0;
            if (ReferenceEquals(mx, null)) return -1;
            if (ReferenceEquals(my, null)) return 1;
            return default(ORD).Compare(mx.Value, my.Value);
        }
    }
}
