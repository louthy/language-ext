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
    public struct OrdEnumerable<OrdA, A> : Ord<IEnumerable<A>>
        where OrdA : struct, Ord<A>
    {
        public static readonly OrdEnumerable<OrdA, A> Inst = default(OrdEnumerable<OrdA, A>);

        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        [Pure]
        public bool Equals(IEnumerable<A> x, IEnumerable<A> y) =>
            default(EqEnumerable<OrdA, A>).Equals(x, y);

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
        public int Compare(IEnumerable<A> x, IEnumerable<A> y)
        {
            if (ReferenceEquals(x, y)) return 0;
            if (ReferenceEquals(x, null)) return -1;
            if (ReferenceEquals(y, null)) return 1;

            using var enumx = x.GetEnumerator();
            using var enumy = y.GetEnumerator();

            while (true)
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
        public int GetHashCode(IEnumerable<A> x) =>
            hash(x);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<bool> EqualsAsync(IEnumerable<A> x, IEnumerable<A> y) =>
            Equals(x, y).AsTask();

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<int> GetHashCodeAsync(IEnumerable<A> x) =>
            GetHashCode(x).AsTask();
        
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<int> CompareAsync(IEnumerable<A> x, IEnumerable<A> y) =>
            Compare(x, y).AsTask();
    }

    /// <summary>
    /// Equality and ordering
    /// </summary>
    public struct OrdEnumerable<A> : Ord<IEnumerable<A>>
    {
        public static readonly OrdEnumerable<A> Inst = default(OrdEnumerable<A>);

        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        [Pure]
        public bool Equals(IEnumerable<A> x, IEnumerable<A> y) =>
            default(OrdEnumerable<OrdDefault<A>, A>).Equals(x, y);

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
        public int Compare(IEnumerable<A> x, IEnumerable<A> y) =>
            default(OrdEnumerable<OrdDefault<A>, A>).Compare(x, y);

        /// <summary>
        /// Get the hash-code of the provided value
        /// </summary>
        /// <returns>Hash code of x</returns>
        [Pure]
        public int GetHashCode(IEnumerable<A> x) =>
            default(OrdEnumerable<OrdDefault<A>, A>).GetHashCode(x);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<bool> EqualsAsync(IEnumerable<A> x, IEnumerable<A> y) =>
            Equals(x, y).AsTask();

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<int> GetHashCodeAsync(IEnumerable<A> x) =>
            GetHashCode(x).AsTask();
        
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<int> CompareAsync(IEnumerable<A> x, IEnumerable<A> y) =>
            Compare(x, y).AsTask();
    }
}
