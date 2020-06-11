using LanguageExt;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using static LanguageExt.TypeClass;
using static LanguageExt.Prelude;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Equality and ordering
    /// </summary>
    public struct OrdHashSet<OrdA, A> : Ord<HashSet<A>>
        where OrdA : struct, Ord<A>
    {
        public static readonly OrdHashSet<OrdA, A> Inst = default(OrdHashSet<OrdA, A>);

        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        [Pure]
        public bool Equals(HashSet<A> x, HashSet<A> y) =>
            default(EqHashSet<OrdA, A>).Equals(x, y);

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
        public int Compare(HashSet<A> x, HashSet<A> y)
        {
            if (x.Count > y.Count) return 1;
            if (x.Count < y.Count) return -1;
            var sa = toSet<A>(x);
            var sb = toSet<A>(y);
            using (var iterA = sa.GetEnumerator())
            {
                using (var iterB = sb.GetEnumerator())
                {
                    while (iterA.MoveNext() && iterB.MoveNext())
                    {
                        var cmp = default(OrdA).Compare(iterA.Current, iterB.Current);
                        if (cmp != 0) return cmp;
                    }
                }
            }
            return 0;
        }

        /// <summary>
        /// Get the hash-code of the provided value
        /// </summary>
        /// <returns>Hash code of x</returns>
        [Pure]
        public int GetHashCode(HashSet<A> x) =>
            x.GetHashCode();

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<bool> EqualsAsync(HashSet<A> x, HashSet<A> y) =>
            Equals(x, y).AsTask();

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<int> GetHashCodeAsync(HashSet<A> x) =>
            GetHashCode(x).AsTask();       
          
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<int> CompareAsync(HashSet<A> x, HashSet<A> y) =>
            Compare(x, y).AsTask();   
    }

    /// <summary>
    /// Equality and ordering
    /// </summary>
    public struct OrdHashSet<A> : Ord<HashSet<A>>
    {
        public static readonly OrdHashSet<A> Inst = default(OrdHashSet<A>);

        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        [Pure]
        public bool Equals(HashSet<A> x, HashSet<A> y) =>
            default(OrdHashSet<OrdDefault<A>, A>).Equals(x, y);

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
        public int Compare(HashSet<A> x, HashSet<A> y) =>
            default(OrdHashSet<OrdDefault<A>, A>).Compare(x, y);

        /// <summary>
        /// Get the hash-code of the provided value
        /// </summary>
        /// <returns>Hash code of x</returns>
        [Pure]
        public int GetHashCode(HashSet<A> x) =>
            default(OrdHashSet<OrdDefault<A>, A>).GetHashCode(x);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<bool> EqualsAsync(HashSet<A> x, HashSet<A> y) =>
            Equals(x, y).AsTask();

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<int> GetHashCodeAsync(HashSet<A> x) =>
            GetHashCode(x).AsTask();       
          
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<int> CompareAsync(HashSet<A> x, HashSet<A> y) =>
            Compare(x, y).AsTask();   
    }
}
