using LanguageExt;
using LanguageExt.ClassInstances.Pred;
using LanguageExt.TypeClasses;
using System;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using static LanguageExt.TypeClass;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Compare the equality of any type in the NewType type-class
    /// </summary>
    [Obsolete("Use EqNewType<NEWTYPE, ORD, A, True<A>>")]
    public struct EqNewType<NEWTYPE, EQ, A> : Eq<NewType<NEWTYPE, A>>
        where EQ : struct, Eq<A>
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
  
        [Pure]
        public Task<bool> EqualsAsync(NewType<NEWTYPE, A> x, NewType<NEWTYPE, A> y) =>
            Equals(x, y).AsTask();

        [Pure]
        public Task<int> GetHashCodeAsync(NewType<NEWTYPE, A> x) => 
            GetHashCode(x).AsTask();      
    }

    /// <summary>
    /// Compare the equality of any type in the NewType type-class
    /// </summary>
    [Obsolete("Use EqNewType<NEWTYPE, OrdDefault<A>, A, True<A>>")]
    public struct EqNewType<NEWTYPE, A> : Eq<NewType<NEWTYPE, A>>
        where NEWTYPE : NewType<NEWTYPE, A>
    {
        public static readonly EqNewType<NEWTYPE, A> Inst = default(EqNewType<NEWTYPE, A>);

        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        public bool Equals(NewType<NEWTYPE, A> x, NewType<NEWTYPE, A> y) =>
            default(EqNewType<NEWTYPE, OrdDefault<A>, A, True<A>>).Equals(x, y);

        /// <summary>
        /// Get hash code of the value
        /// </summary>
        /// <param name="x">Value to get the hash code of</param>
        /// <returns>The hash code of x</returns>
        public int GetHashCode(NewType<NEWTYPE, A> x) =>
            default(EqNewType<NEWTYPE, OrdDefault<A>, A, True<A>>).GetHashCode(x);
  
        [Pure]
        public Task<bool> EqualsAsync(NewType<NEWTYPE, A> x, NewType<NEWTYPE, A> y) =>
            Equals(x, y).AsTask();

        [Pure]
        public Task<int> GetHashCodeAsync(NewType<NEWTYPE, A> x) => 
            GetHashCode(x).AsTask();      
    }

    /// <summary>
    /// Compare the equality of any type in the NewType type-class
    /// </summary>
    [Obsolete("Use OrdNewType<NEWTYPE, ORD, A, PRED>")]
    public struct EqNewType<NEWTYPE, ORD, A, PRED> : Eq<NewType<NEWTYPE, A, PRED, ORD>>
        where ORD : struct, Ord<A>
        where PRED : struct, Pred<A>
        where NEWTYPE : NewType<NEWTYPE, A, PRED, ORD>
    {
        public static readonly EqNewType<NEWTYPE, ORD, A, PRED> Inst = default(EqNewType<NEWTYPE, ORD, A, PRED>);

        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        [Pure]
        public bool Equals(NewType<NEWTYPE, A, PRED, ORD> x, NewType<NEWTYPE, A, PRED, ORD> y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            return default(ORD).Equals((A)x, (A)y);
        }

        /// <summary>
        /// Get hash code of the value
        /// </summary>
        /// <param name="x">Value to get the hash code of</param>
        /// <returns>The hash code of x</returns>
        [Pure]
        public int GetHashCode(NewType<NEWTYPE, A, PRED, ORD> x) =>
            x.IsNull() ? 0 : x.GetHashCode();
  
        [Pure]
        public Task<bool> EqualsAsync(NewType<NEWTYPE, A, PRED, ORD> x, NewType<NEWTYPE, A, PRED, ORD> y) =>
            Equals(x, y).AsTask();

        [Pure]
        public Task<int> GetHashCodeAsync(NewType<NEWTYPE, A, PRED, ORD> x) => 
            GetHashCode(x).AsTask();      
    }
}
