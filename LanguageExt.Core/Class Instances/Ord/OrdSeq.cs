using LanguageExt;
using LanguageExt.TypeClasses;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using static LanguageExt.Prelude;
using static LanguageExt.TypeClass;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Equality and ordering
    /// </summary>
    public struct OrdSeq<OrdA, A> : Ord<Seq<A>>
        where OrdA : struct, Ord<A>
    {
        public static readonly OrdSeq<OrdA, A> Inst = default(OrdSeq<OrdA, A>);

        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        [Pure]
        public bool Equals(Seq<A> x, Seq<A> y) =>
            default(EqSeq<OrdA, A>).Equals(x, y);

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
        public int Compare(Seq<A> x, Seq<A> y)
        {
            using var enumx = x.GetEnumerator();
            using var enumy = y.GetEnumerator();

            while(true)
            {
                bool r1 = enumx.MoveNext();
                bool r2 = enumy.MoveNext();
                if (!r1 && !r2) return 0;
                if (!r1) return -1;
                if (!r2) return 1;

                var cmp = default(OrdA).Compare(enumx.Current, enumy.Current);
                if (cmp != 0) return cmp;
            }
        }

        /// <summary>
        /// Get the hash-code of the provided value
        /// </summary>
        /// <returns>Hash code of x</returns>
        [Pure]
        public int GetHashCode(Seq<A> x) =>
            hash(x);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<bool> EqualsAsync(Seq<A> x, Seq<A> y) =>
            Equals(x, y).AsTask();

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<int> GetHashCodeAsync(Seq<A> x) =>
            GetHashCode(x).AsTask();       
        
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<int> CompareAsync(Seq<A> x, Seq<A> y) =>
            Compare(x, y).AsTask();    
    }

    /// <summary>
    /// Equality and ordering
    /// </summary>
    public struct OrdSeq<A> : Ord<Seq<A>>
    {
        public static readonly OrdSeq<A> Inst = default(OrdSeq<A>);

        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        [Pure]
        public bool Equals(Seq<A> x, Seq<A> y) =>
            default(OrdSeq<OrdDefault<A>, A>).Equals(x, y);

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
        public int Compare(Seq<A> x, Seq<A> y) =>
            default(OrdSeq<OrdDefault<A>, A>).Compare(x, y);

        /// <summary>
        /// Get the hash-code of the provided value
        /// </summary>
        /// <returns>Hash code of x</returns>
        [Pure]
        public int GetHashCode(Seq<A> x) =>
            default(OrdSeq<OrdDefault<A>, A>).GetHashCode(x);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<bool> EqualsAsync(Seq<A> x, Seq<A> y) =>
            Equals(x, y).AsTask();

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<int> GetHashCodeAsync(Seq<A> x) =>
            GetHashCode(x).AsTask();       
        
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<int> CompareAsync(Seq<A> x, Seq<A> y) =>
            Compare(x, y).AsTask();    
    }
}
