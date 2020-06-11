using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using static LanguageExt.Prelude;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Identity equality
    /// </summary>
    public struct OrdIdentity<A> : Ord<Identity<A>>
    {
        public static readonly OrdIdentity<A> Inst = default(OrdIdentity<A>);

        /// <summary>
        /// Ordering test
        /// </summary>
        /// <param name="x">The left hand side of the ordering operation</param>
        /// <param name="y">The right hand side of the ordering operation</param>
        /// <returns>-1 if less than, 0 if equal, 1 if greater than</returns>
        [Pure]
        public int Compare(Identity<A> a, Identity<A> b)  => 
            a.CompareTo(b);

        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        [Pure]
        public bool Equals(Identity<A> a, Identity<A> b)  => 
            default(EqIdentity<A>).Equals(a, b);

        /// <summary>
        /// Get hash code of the value
        /// </summary>
        /// <param name="x">Value to get the hash code of</param>
        /// <returns>The hash code of x</returns>
        [Pure]
        public int GetHashCode(Identity<A> x) =>
            default(HashableIdentity<A>).GetHashCode(x);
       
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<bool> EqualsAsync(Identity<A> x, Identity<A> y) =>
            Equals(x, y).AsTask();

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<int> GetHashCodeAsync(Identity<A> x) =>
            GetHashCode(x).AsTask();        
        
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<int> CompareAsync(Identity<A> x, Identity<A> y) =>
            Compare(x, y).AsTask();    
    }
}
