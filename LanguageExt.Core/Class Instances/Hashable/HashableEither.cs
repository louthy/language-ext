using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using LanguageExt.TypeClasses;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Either type hashing
    /// </summary>
    public struct HashableEither<HashableL, HashableR, L, R> : Hashable<Either<L, R>>
        where HashableL : struct, Hashable<L>
        where HashableR : struct, Hashable<R>
    {
        /// <summary>
        /// Get hash code of the value
        /// </summary>
        /// <param name="x">Value to get the hash code of</param>
        /// <returns>The hash code of x</returns>
        [Pure] 
        public int GetHashCode(Either<L, R> x) => x.State switch
        {
            EitherStatus.IsRight => default(HashableR).GetHashCode(x.RightValue),
            EitherStatus.IsLeft => default(HashableL).GetHashCode(x.LeftValue),
            _ => 0
        };

        /// <summary>
        /// Get hash code of the value
        /// </summary>
        /// <param name="x">Value to get the hash code of</param>
        /// <returns>The hash code of x</returns>
        [Pure]
        public Task<int> GetHashCodeAsync(Either<L, R> x) =>
            GetHashCode(x).AsTask();
    }
    
    /// <summary>
    /// Either type hashing
    /// </summary>
    public struct HashableEither<L, R> : Hashable<Either<L, R>>
    {
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
    }
}
