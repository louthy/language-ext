using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using LanguageExt.Traits;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Either type hashing
    /// </summary>
    public struct EqEither<EqL, EqR, L, R> : Eq<Either<L, R>>
        where EqL : Eq<L>
        where EqR : Eq<R>
    {
        /// <summary>
        /// Equality test
        /// </summary>
        [Pure]
        public static bool Equals(Either<L, R> x, Either<L, R> y) =>
            x.Equals<EqL, EqR>(y);


        /// <summary>
        /// Get hash code of the value
        /// </summary>
        /// <param name="x">Value to get the hash code of</param>
        /// <returns>The hash code of x</returns>
        [Pure]
        public static int GetHashCode(Either<L, R> x) => 
            HashableEither<EqL, EqR, L, R>.GetHashCode(x);

        /// <summary>
        /// Get hash code of the value
        /// </summary>
        /// <param name="x">Value to get the hash code of</param>
        /// <returns>The hash code of x</returns>
        [Pure]
        public static Task<int> GetHashCodeAsync(Either<L, R> x) =>
            GetHashCode(x).AsTask();

        /// <summary>
        /// Equality test
        /// </summary>
        [Pure]
        public static Task<bool> EqualsAsync(Either<L, R> x, Either<L, R> y) => 
            Equals(x, y).AsTask();
    }
    
    /// <summary>
    /// Either type hashing
    /// </summary>
    public struct EqEither<L, R> : Eq<Either<L, R>>
    {
        /// <summary>
        /// Equality test
        /// </summary>
        [Pure]
        public static bool Equals(Either<L, R> x, Either<L, R> y) => 
            EqEither<EqDefault<L>, EqDefault<R>, L, R>.Equals(x, y);
        
        /// <summary>
        /// Get hash code of the value
        /// </summary>
        /// <param name="x">Value to get the hash code of</param>
        /// <returns>The hash code of x</returns>
        [Pure] 
        public static int GetHashCode(Either<L, R> x) =>
            HashableEither<HashableDefault<L>, HashableDefault<R>, L, R>.GetHashCode(x);
    }
}
