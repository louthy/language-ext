using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Equality and ordering
    /// </summary>
    public struct OrdArr<OrdA, A> : Ord<Arr<A>>
        where OrdA : struct, Ord<A>
    {
        public static readonly OrdArr<OrdA, A> Inst = default(OrdArr<OrdA, A>);

        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        [Pure]
        public bool Equals(Arr<A> x, Arr<A> y) =>
            default(EqArr<OrdA, A>).Equals(x, y);

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
        public int Compare(Arr<A> mx, Arr<A> my)
        {
            var cmp = mx.Count.CompareTo(my.Count);
            if (cmp == 0)
            {
                var xiter = mx.GetEnumerator();
                var yiter = my.GetEnumerator();
                while(xiter.MoveNext() && yiter.MoveNext())
                {
                    cmp = default(OrdA).Compare(xiter.Current, yiter.Current);
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
        public int GetHashCode(Arr<A> x) =>
            x.GetHashCode();

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<bool> EqualsAsync(Arr<A> x, Arr<A> y) =>
            Equals(x, y).AsTask();

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<int> GetHashCodeAsync(Arr<A> x) =>
            GetHashCode(x).AsTask();       
        
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<int> CompareAsync(Arr<A> x, Arr<A> y) =>
            Compare(x, y).AsTask();
    }

    /// <summary>
    /// Equality and ordering
    /// </summary>
    public struct OrdArr<A> : Ord<Arr<A>>
    {
        public static readonly OrdArr<A> Inst = default(OrdArr<A>);

        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        [Pure]
        public bool Equals(Arr<A> x, Arr<A> y) =>
            default(EqArr<A>).Equals(x, y);

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
        public int Compare(Arr<A> mx, Arr<A> my) =>
            default(OrdArr<OrdDefault<A>, A>).Compare(mx, my);

        /// <summary>
        /// Get the hash-code of the provided value
        /// </summary>
        /// <returns>Hash code of x</returns>
        [Pure]
        public int GetHashCode(Arr<A> x) =>
            default(OrdArr<OrdDefault<A>, A>).GetHashCode(x);
 
        [Pure]
        public Task<bool> EqualsAsync(Arr<A> x, Arr<A> y) =>
            Equals(x, y).AsTask();

        [Pure]
        public Task<int> GetHashCodeAsync(Arr<A> x) =>
            GetHashCode(x).AsTask();    
        
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<int> CompareAsync(Arr<A> x, Arr<A> y) =>
            Compare(x, y).AsTask();
    }
}
