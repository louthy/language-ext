using LanguageExt;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;
using static LanguageExt.TypeClass;

namespace LanguageExt.ClassInstances
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
            return default(EQ).Equals((A)x, (A)y);
        }

        /// <summary>
        /// Get hash code of the value
        /// </summary>
        /// <param name="x">Value to get the hash code of</param>
        /// <returns>The hash code of x</returns>
        public int GetHashCode(NewType<NEWTYPE, A> x) =>
            x.IsNull() ? 0 : x.GetHashCode();
    }

    /// <summary>
    /// Compare the equality of any type in the NewType type-class
    /// </summary>
    public struct EqNewType<NEWTYPE, EQ, A, PRED> : Eq<NewType<NEWTYPE, A, PRED>>
        where EQ : struct, Eq<A>
        where PRED : struct, Pred<A>
        where NEWTYPE : NewType<NEWTYPE, A, PRED>
    {
        public static readonly EqNewType<NEWTYPE, EQ, A, PRED> Inst = default(EqNewType<NEWTYPE, EQ, A, PRED>);

        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        [Pure]
        public bool Equals(NewType<NEWTYPE, A, PRED> x, NewType<NEWTYPE, A, PRED> y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            return default(EQ).Equals((A)x, (A)y);
        }

        /// <summary>
        /// Get hash code of the value
        /// </summary>
        /// <param name="x">Value to get the hash code of</param>
        /// <returns>The hash code of x</returns>
        [Pure]
        public int GetHashCode(NewType<NEWTYPE, A, PRED> x) =>
            x.IsNull() ? 0 : x.GetHashCode();
    }
}
