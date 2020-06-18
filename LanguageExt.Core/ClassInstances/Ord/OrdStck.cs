using LanguageExt;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using static LanguageExt.TypeClass;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Equality and ordering
    /// </summary>
    public struct OrdStck<OrdA, A> : Ord<Stck<A>>
        where OrdA : struct, Ord<A>
    {
        public static readonly OrdStck<OrdA, A> Inst = default(OrdStck<OrdA, A>);

        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        [Pure]
        public bool Equals(Stck<A> x, Stck<A> y) =>
            default(EqStck<OrdA, A>).Equals(x, y);

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
        public int Compare(Stck<A> x, Stck<A> y)
        {
            var cmp = x.Count.CompareTo(y.Count);
            if (cmp == 0)
            {
                using var enumx = x.GetEnumerator();
                using var enumy = y.GetEnumerator();
                var count = x.Count;

                for (int i = 0; i < count; i++)
                {
                    enumx.MoveNext();
                    enumy.MoveNext();
                    cmp = default(OrdA).Compare(enumx.Current, enumy.Current);
                    if (cmp != 0) return cmp;
                }
                return 0;
            }
            else
            {
                return cmp;
            }
        }

        /// <summary>
        /// Get the hash-code of the provided value
        /// </summary>
        /// <returns>Hash code of x</returns>
        [Pure]
        public int GetHashCode(Stck<A> x) =>
            x.GetHashCode();

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<bool> EqualsAsync(Stck<A> x, Stck<A> y) =>
            Equals(x, y).AsTask();

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<int> GetHashCodeAsync(Stck<A> x) =>
            GetHashCode(x).AsTask();       
            
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<int> CompareAsync(Stck<A> x, Stck<A> y) =>
            Compare(x, y).AsTask();   
    }

    /// <summary>
    /// Equality and ordering
    /// </summary>
    public struct OrdStck<A> : Ord<Stck<A>>
    {
        public static readonly OrdStck<A> Inst = default(OrdStck<A>);

        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        [Pure]
        public bool Equals(Stck<A> x, Stck<A> y) =>
            default(OrdStck<OrdDefault<A>, A>).Equals(x, y);

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
        public int Compare(Stck<A> x, Stck<A> y) =>
            default(OrdStck<OrdDefault<A>, A>).Compare(x, y);

        /// <summary>
        /// Get the hash-code of the provided value
        /// </summary>
        /// <returns>Hash code of x</returns>
        [Pure]
        public int GetHashCode(Stck<A> x) =>
            default(OrdStck<OrdDefault<A>, A>).GetHashCode(x);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<bool> EqualsAsync(Stck<A> x, Stck<A> y) =>
            Equals(x, y).AsTask();

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<int> GetHashCodeAsync(Stck<A> x) =>
            GetHashCode(x).AsTask();       
            
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<int> CompareAsync(Stck<A> x, Stck<A> y) =>
            Compare(x, y).AsTask();  
    }
}
