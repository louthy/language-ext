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
    public struct OrdQue<OrdA, A> : Ord<Que<A>>
        where OrdA : struct, Ord<A>
    {
        public static readonly OrdQue<OrdA, A> Inst = default(OrdQue<OrdA, A>);

        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        [Pure]
        public bool Equals(Que<A> x, Que<A> y) =>
            default(EqQue<OrdA, A>).Equals(x, y);

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
        public int Compare(Que<A> x, Que<A> y)
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
        public int GetHashCode(Que<A> x) =>
            x.GetHashCode();

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<bool> EqualsAsync(Que<A> x, Que<A> y) =>
            Equals(x, y).AsTask();

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<int> GetHashCodeAsync(Que<A> x) =>
            GetHashCode(x).AsTask();       
        
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<int> CompareAsync(Que<A> x, Que<A> y) =>
            Compare(x, y).AsTask();    
    }

    /// <summary>
    /// Equality and ordering
    /// </summary>
    public struct OrdQue<A> : Ord<Que<A>>
    {
        public static readonly OrdQue<A> Inst = default(OrdQue<A>);

        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        [Pure]
        public bool Equals(Que<A> x, Que<A> y) =>
            default(OrdQue<OrdDefault<A>, A>).Equals(x, y);

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
        public int Compare(Que<A> x, Que<A> y) =>
            default(OrdQue<OrdDefault<A>, A>).Compare(x, y);

        /// <summary>
        /// Get the hash-code of the provided value
        /// </summary>
        /// <returns>Hash code of x</returns>
        [Pure]
        public int GetHashCode(Que<A> x) =>
            default(OrdQue<OrdDefault<A>, A>).GetHashCode(x);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<bool> EqualsAsync(Que<A> x, Que<A> y) =>
            Equals(x, y).AsTask();

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<int> GetHashCodeAsync(Que<A> x) =>
            GetHashCode(x).AsTask();       
        
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<int> CompareAsync(Que<A> x, Que<A> y) =>
            Compare(x, y).AsTask();    
    }
}
