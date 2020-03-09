using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using LanguageExt.TypeClasses;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Either type hashing
    /// </summary>
    public struct OrdEitherUnsafe<OrdL, OrdR, L, R> : Ord<EitherUnsafe<L, R>>
        where OrdL : struct, Ord<L>
        where OrdR : struct, Ord<R>
    {
        /// <summary>
        /// Ordering test
        /// </summary>
        [Pure]
        public int Compare(EitherUnsafe<L, R> x, EitherUnsafe<L, R> y) => x.State switch
        {
            EitherStatus.IsRight => y.State switch
            {
                EitherStatus.IsRight => default(OrdR).Compare(x.RightValue, y.RightValue),
                EitherStatus.IsLeft => 1,
                _ => 1
            },
            EitherStatus.IsLeft => y.State switch
            {
                EitherStatus.IsLeft => default(OrdL).Compare(x.LeftValue, y.LeftValue),
                EitherStatus.IsRight => -1,
                _ => 1
            },
            EitherStatus.IsBottom => y.State == EitherStatus.IsBottom ? 0 : -1,
            _ => 0
        };

        /// <summary>
        /// Equality test
        /// </summary>
        [Pure]
        public bool Equals(EitherUnsafe<L, R> x, EitherUnsafe<L, R> y) =>
            default(EqEitherUnsafe<OrdL, OrdR, L, R>).Equals(x, y);

        /// <summary>
        /// Get hash code of the value
        /// </summary>
        /// <param name="x">Value to get the hash code of</param>
        /// <returns>The hash code of x</returns>
        [Pure]
        public int GetHashCode(EitherUnsafe<L, R> x) => 
            default(HashableEitherUnsafe<OrdL, OrdR, L, R>).GetHashCode(x);

        /// <summary>
        /// Get hash code of the value
        /// </summary>
        /// <param name="x">Value to get the hash code of</param>
        /// <returns>The hash code of x</returns>
        [Pure]
        public Task<int> GetHashCodeAsync(EitherUnsafe<L, R> x) =>
            GetHashCode(x).AsTask();

        /// <summary>
        /// Equality test
        /// </summary>
        [Pure]
        public Task<bool> EqualsAsync(EitherUnsafe<L, R> x, EitherUnsafe<L, R> y) => 
            Equals(x, y).AsTask();
        
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<int> CompareAsync(EitherUnsafe<L, R> x, EitherUnsafe<L, R> y) =>
            Compare(x, y).AsTask();
    }
    
    /// <summary>
    /// Either type hashing
    /// </summary>
    public struct OrdEitherUnsafe<L, R> : Ord<EitherUnsafe<L, R>>
    {
        /// <summary>
        /// Ordering test
        /// </summary>
        [Pure]
        public int Compare(EitherUnsafe<L, R> x, EitherUnsafe<L, R> y) => 
            default(OrdEitherUnsafe<OrdDefault<L>, OrdDefault<R>, L, R>).Compare(x, y);
 
        /// <summary>
        /// Equality test
        /// </summary>
        [Pure]
        public bool Equals(EitherUnsafe<L, R> x, EitherUnsafe<L, R> y) => 
            default(EqEitherUnsafe<EqDefault<L>, EqDefault<R>, L, R>).Equals(x, y);
        
        /// <summary>
        /// Get hash code of the value
        /// </summary>
        /// <param name="x">Value to get the hash code of</param>
        /// <returns>The hash code of x</returns>
        [Pure] 
        public int GetHashCode(EitherUnsafe<L, R> x) =>
            default(HashableEitherUnsafe<HashableDefault<L>, HashableDefault<R>, L, R>).GetHashCode(x);

        /// <summary>
        /// Get hash code of the value
        /// </summary>
        /// <param name="x">Value to get the hash code of</param>
        /// <returns>The hash code of x</returns>
        [Pure]
        public Task<int> GetHashCodeAsync(EitherUnsafe<L, R> x) =>
            GetHashCode(x).AsTask();

        /// <summary>
        /// Equality test
        /// </summary>
        [Pure]
        public Task<bool> EqualsAsync(EitherUnsafe<L, R> x, EitherUnsafe<L, R> y) => 
            Equals(x, y).AsTask();    
        
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<int> CompareAsync(EitherUnsafe<L, R> x, EitherUnsafe<L, R> y) =>
            Compare(x, y).AsTask();
    }
}
