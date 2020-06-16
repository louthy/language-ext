using LanguageExt.TypeClasses;
using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Compare the equality and ordering of any type in the NewType
    /// type-class
    /// </summary>
    [Obsolete("Use OrdNewType<NEWTYPE, ORD, A, True<A>>")]
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

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<bool> EqualsAsync(NewType<NEWTYPE, A> x, NewType<NEWTYPE, A> y) =>
            Equals(x, y).AsTask();

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<int> GetHashCodeAsync(NewType<NEWTYPE, A> x) =>
            GetHashCode(x).AsTask();        
           
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<int> CompareAsync(NewType<NEWTYPE, A> x, NewType<NEWTYPE, A> y) =>
            Compare(x, y).AsTask();   
    }

    /// <summary>
    /// Compare the equality and ordering of any type in the NewType
    /// type-class
    /// </summary>
    [Obsolete("Use OrdNewType<NEWTYPE, OrdDefault<A>, A, True<A>>")]
    public struct OrdNewType<NEWTYPE, A> : Ord<NewType<NEWTYPE, A>>
        where NEWTYPE : NewType<NEWTYPE, A>
    {
        public static readonly OrdNewType<NEWTYPE, A> Inst = default(OrdNewType<NEWTYPE, A>);

        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        [Pure]
        public bool Equals(NewType<NEWTYPE, A> x, NewType<NEWTYPE, A> y) =>
            default(OrdNewType<NEWTYPE, OrdDefault<A>, A>).Equals(x, y);

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
        public int Compare(NewType<NEWTYPE, A> mx, NewType<NEWTYPE, A> my) =>
            default(OrdNewType<NEWTYPE, OrdDefault<A>, A>).Compare(mx, my);

        /// <summary>
        /// Get the hash-code of the provided value
        /// </summary>
        /// <returns>Hash code of x</returns>
        [Pure]
        public int GetHashCode(NewType<NEWTYPE, A> x) =>
            default(OrdNewType<NEWTYPE, OrdDefault<A>, A>).GetHashCode(x);
        
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<bool> EqualsAsync(NewType<NEWTYPE, A> x, NewType<NEWTYPE, A> y) =>
            Equals(x, y).AsTask();

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<int> GetHashCodeAsync(NewType<NEWTYPE, A> x) =>
            GetHashCode(x).AsTask();         
           
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<int> CompareAsync(NewType<NEWTYPE, A> x, NewType<NEWTYPE, A> y) =>
            Compare(x, y).AsTask();   
    }

    /// <summary>
    /// Compare the equality and ordering of any type in the NewType
    /// type-class
    /// </summary>
    public struct OrdNewType<NEWTYPE, ORD, A, PRED> : Ord<NewType<NEWTYPE, A, PRED, ORD>>
        where ORD     : struct, Ord<A>
        where PRED    : struct, Pred<A>
        where NEWTYPE : NewType<NEWTYPE, A, PRED, ORD>
    {
        public static readonly OrdNewType<NEWTYPE, ORD, A, PRED> Inst = default(OrdNewType<NEWTYPE, ORD, A, PRED>);

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
        public int Compare(NewType<NEWTYPE, A, PRED, ORD> mx, NewType<NEWTYPE, A, PRED, ORD> my)
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
        public int GetHashCode(NewType<NEWTYPE, A, PRED, ORD> x) =>
            x.IsNull() ? 0 : default(ORD).GetHashCode(x.Value);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<bool> EqualsAsync(NewType<NEWTYPE, A, PRED, ORD> x, NewType<NEWTYPE, A, PRED, ORD> y) =>
            Equals(x, y).AsTask();

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<int> GetHashCodeAsync(NewType<NEWTYPE, A, PRED, ORD> x) =>
            GetHashCode(x).AsTask();        
            
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<int> CompareAsync(NewType<NEWTYPE, A, PRED, ORD> x, NewType<NEWTYPE, A, PRED, ORD> y) =>
            Compare(x, y).AsTask();   
    }
}
