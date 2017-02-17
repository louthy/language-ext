using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;
using static LanguageExt.TypeClass;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Compare the equality and ordering of any type in the NewType
    /// type-class
    /// </summary>
    public struct OrdNewType<NEWTYPE, ORD, A> : Ord<NewType<NEWTYPE, A>>
        where ORD : struct, Ord<A>
        where NEWTYPE : NewType<NEWTYPE, A>
    {
        public static readonly OrdNewType<NEWTYPE, ORD, A> Inst = default(OrdNewType<NEWTYPE, ORD, A>);

        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        [Pure]
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
        [Pure]
        public int Compare(NewType<NEWTYPE, A> mx, NewType<NEWTYPE, A> my)
        {
            if (ReferenceEquals(mx, my)) return 0;
            if (ReferenceEquals(mx, null)) return -1;
            if (ReferenceEquals(my, null)) return 1;
            return default(ORD).Compare((A)mx, (A)my);
        }

        /// <summary>
        /// Get the hash-code of the provided value
        /// </summary>
        /// <returns>Hash code of x</returns>
        [Pure]
        public int GetHashCode(NewType<NEWTYPE, A> x) =>
            x.IsNull() ? 0 : x.GetHashCode();
    }

    /// <summary>
    /// Compare the equality and ordering of any type in the NewType
    /// type-class
    /// </summary>
    public struct OrdNewType<NEWTYPE, ORD, A, PRED> : Ord<NewType<NEWTYPE, A, PRED>>
        where ORD     : struct, Ord<A>
        where PRED    : struct, Pred<A>
        where NEWTYPE : NewType<NEWTYPE, A, PRED>
    {
        public static readonly OrdNewType<NEWTYPE, ORD, A, PRED> Inst = default(OrdNewType<NEWTYPE, ORD, A, PRED>);

        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        [Pure]
        public bool Equals(NewType<NEWTYPE, A, PRED> x, NewType<NEWTYPE, A, PRED> y) =>
            default(EqNewType<NEWTYPE, ORD, A, PRED>).Equals(x, y);

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
        [Pure]
        public int Compare(NewType<NEWTYPE, A, PRED> mx, NewType<NEWTYPE, A, PRED> my)
        {
            if (ReferenceEquals(mx, my)) return 0;
            if (ReferenceEquals(mx, null)) return -1;
            if (ReferenceEquals(my, null)) return 1;
            return default(ORD).Compare((A)mx, (A)my);
        }

        /// <summary>
        /// Get the hash-code of the provided value
        /// </summary>
        /// <returns>Hash code of x</returns>
        [Pure]
        public int GetHashCode(NewType<NEWTYPE, A, PRED> x) =>
            x.IsNull() ? 0 : x.GetHashCode();
    }
}
