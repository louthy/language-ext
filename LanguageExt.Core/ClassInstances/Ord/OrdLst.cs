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
    public struct OrdLst<OrdA, A> : Ord<Lst<A>>
        where OrdA : struct, Ord<A>
    {
        public static readonly OrdLst<OrdA, A> Inst = default(OrdLst<OrdA, A>);

        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        [Pure]
        public bool Equals(Lst<A> x, Lst<A> y) =>
            default(EqLst<OrdA, A>).Equals(x, y);

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
        public int Compare(Lst<A> mx, Lst<A> my)
        {
            var cmp = mx.Count.CompareTo(my.Count);
            if (cmp == 0)
            {
                using var xiter = mx.GetEnumerator();
                using var yiter = my.GetEnumerator();
                while (xiter.MoveNext() && yiter.MoveNext())
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
        public int GetHashCode(Lst<A> x) =>
            x.GetHashCode();
       
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<bool> EqualsAsync(Lst<A> x, Lst<A> y) =>
            Equals(x, y).AsTask();

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<int> GetHashCodeAsync(Lst<A> x) =>
            GetHashCode(x).AsTask();        
        
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<int> CompareAsync(Lst<A> x, Lst<A> y) =>
            Compare(x, y).AsTask();    
    }

    /// <summary>
    /// Equality and ordering
    /// </summary>
    public struct OrdLst<A> : Ord<Lst<A>>
    {
        public static readonly OrdLst<A> Inst = default(OrdLst<A>);

        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        [Pure]
        public bool Equals(Lst<A> x, Lst<A> y) =>
            default(OrdLst<OrdDefault<A>, A>).Equals(x, y);

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
        public int Compare(Lst<A> x, Lst<A> y) =>
            default(OrdLst<OrdDefault<A>, A>).Compare(x, y);

        /// <summary>
        /// Get the hash-code of the provided value
        /// </summary>
        /// <returns>Hash code of x</returns>
        [Pure]
        public int GetHashCode(Lst<A> x) =>
            default(OrdLst<OrdDefault<A>, A>).GetHashCode(x);
        
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<bool> EqualsAsync(Lst<A> x, Lst<A> y) =>
            Equals(x, y).AsTask();

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<int> GetHashCodeAsync(Lst<A> x) =>
            GetHashCode(x).AsTask();         
        
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<int> CompareAsync(Lst<A> x, Lst<A> y) =>
            Compare(x, y).AsTask();    
    }
}
