using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using LanguageExt.TypeClasses;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Either type hashing
    /// </summary>
    public struct EqEitherUnsafe<EqL, EqR, L, R> : Eq<EitherUnsafe<L, R>>
        where EqL : struct, Eq<L>
        where EqR : struct, Eq<R>
    {
        /// <summary>
        /// Equality test
        /// </summary>
        [Pure]
        public bool Equals(EitherUnsafe<L, R> x, EitherUnsafe<L, R> y) => x.State switch
        {
            EitherStatus.IsRight => y.State switch
            {
                EitherStatus.IsRight => default(EqR).Equals(x.RightValue, y.RightValue),
                _ => false
            },
            EitherStatus.IsLeft => y.State switch
            {
                EitherStatus.IsLeft => default(EqL).Equals(x.LeftValue, y.LeftValue),
                _ => false
            },
            EitherStatus.IsBottom => y.State == EitherStatus.IsBottom,
            _ => false
        };


        /// <summary>
        /// Get hash code of the value
        /// </summary>
        /// <param name="x">Value to get the hash code of</param>
        /// <returns>The hash code of x</returns>
        [Pure]
        public int GetHashCode(EitherUnsafe<L, R> x) => 
            default(HashableEitherUnsafe<EqL, EqR, L, R>).GetHashCode(x);

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
    }
    
    /// <summary>
    /// Either type hashing
    /// </summary>
    public struct EqEitherUnsafe<L, R> : Eq<EitherUnsafe<L, R>>
    {
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
    }
}
