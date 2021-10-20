using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using LanguageExt.TypeClasses;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Either type hashing
    /// </summary>
    public struct OrdEither<OrdL, OrdR, L, R> : Ord<Either<L, R>>
        where OrdL : struct, Ord<L>
        where OrdR : struct, Ord<R>
    {
        /// <summary>
        /// Ordering test
        /// </summary>
        [Pure]
        public int Compare(Either<L, R> x, Either<L, R> y) => x.State switch
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
        public bool Equals(Either<L, R> x, Either<L, R> y) =>
            default(EqEither<OrdL, OrdR, L, R>).Equals(x, y);

        /// <summary>
        /// Get hash code of the value
        /// </summary>
        /// <param name="x">Value to get the hash code of</param>
        /// <returns>The hash code of x</returns>
        [Pure]
        public int GetHashCode(Either<L, R> x) => 
            default(HashableEither<OrdL, OrdR, L, R>).GetHashCode(x);

        /// <summary>
        /// Get hash code of the value
        /// </summary>
        /// <param name="x">Value to get the hash code of</param>
        /// <returns>The hash code of x</returns>
        [Pure]
        public Task<int> GetHashCodeAsync(Either<L, R> x) =>
            GetHashCode(x).AsTask();

        /// <summary>
        /// Equality test
        /// </summary>
        [Pure]
        public Task<bool> EqualsAsync(Either<L, R> x, Either<L, R> y) => 
            Equals(x, y).AsTask();
        
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<int> CompareAsync(Either<L, R> x, Either<L, R> y) =>
            Compare(x, y).AsTask();
    }
    
    /// <summary>
    /// Either type hashing
    /// </summary>
    public struct OrdEither<L, R> : Ord<Either<L, R>>
    {
        /// <summary>
        /// Ordering test
        /// </summary>
        [Pure]
        public int Compare(Either<L, R> x, Either<L, R> y) => 
            default(OrdEither<OrdDefault<L>, OrdDefault<R>, L, R>).Compare(x, y);
 
        /// <summary>
        /// Equality test
        /// </summary>
        [Pure]
        public bool Equals(Either<L, R> x, Either<L, R> y) => 
            default(EqEither<EqDefault<L>, EqDefault<R>, L, R>).Equals(x, y);
        
        /// <summary>
        /// Get hash code of the value
        /// </summary>
        /// <param name="x">Value to get the hash code of</param>
        /// <returns>The hash code of x</returns>
        [Pure] 
        public int GetHashCode(Either<L, R> x) =>
            default(HashableEither<HashableDefault<L>, HashableDefault<R>, L, R>).GetHashCode(x);

        /// <summary>
        /// Get hash code of the value
        /// </summary>
        /// <param name="x">Value to get the hash code of</param>
        /// <returns>The hash code of x</returns>
        [Pure]
        public Task<int> GetHashCodeAsync(Either<L, R> x) =>
            GetHashCode(x).AsTask();

        /// <summary>
        /// Equality test
        /// </summary>
        [Pure]
        public Task<bool> EqualsAsync(Either<L, R> x, Either<L, R> y) => 
            Equals(x, y).AsTask();    
        
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<int> CompareAsync(Either<L, R> x, Either<L, R> y) =>
            Compare(x, y).AsTask();
    }
}
