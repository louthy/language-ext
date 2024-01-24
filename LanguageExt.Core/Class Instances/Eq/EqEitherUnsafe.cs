using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using LanguageExt.TypeClasses;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Either type hashing
    /// </summary>
    public struct EqEitherUnsafe<EqL, EqR, L, R> : Eq<EitherUnsafe<L, R>>
        where EqL : Eq<L?>
        where EqR : Eq<R?>
    {
        /// <summary>
        /// Equality test
        /// </summary>
        [Pure]
        public static bool Equals(EitherUnsafe<L, R> x, EitherUnsafe<L, R> y) =>
            x.State switch
            {
                EitherStatus.IsRight => y.State switch
                                        {
                                            EitherStatus.IsRight => EqR.Equals(x.RightValue, y.RightValue),
                                            _                    => false
                                        },
                EitherStatus.IsLeft => y.State switch
                                       {
                                           EitherStatus.IsLeft => EqL.Equals(x.LeftValue, y.LeftValue),
                                           _                   => false
                                       },
                EitherStatus.IsBottom => y.State == EitherStatus.IsBottom,
                _                     => false
            };


        /// <summary>
        /// Get hash code of the value
        /// </summary>
        /// <param name="x">Value to get the hash code of</param>
        /// <returns>The hash code of x</returns>
        [Pure]
        public static int GetHashCode(EitherUnsafe<L, R> x) => 
            HashableEitherUnsafe<EqL, EqR, L, R>.GetHashCode(x);

        /// <summary>
        /// Get hash code of the value
        /// </summary>
        /// <param name="x">Value to get the hash code of</param>
        /// <returns>The hash code of x</returns>
        [Pure]
        public static Task<int> GetHashCodeAsync(EitherUnsafe<L, R> x) =>
            GetHashCode(x).AsTask();

        /// <summary>
        /// Equality test
        /// </summary>
        [Pure]
        public static Task<bool> EqualsAsync(EitherUnsafe<L, R> x, EitherUnsafe<L, R> y) => 
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
        public static bool Equals(EitherUnsafe<L, R> x, EitherUnsafe<L, R> y) => 
            EqEitherUnsafe<EqDefault<L?>, EqDefault<R?>, L, R>.Equals(x, y);
        
        /// <summary>
        /// Get hash code of the value
        /// </summary>
        /// <param name="x">Value to get the hash code of</param>
        /// <returns>The hash code of x</returns>
        [Pure] 
        public static int GetHashCode(EitherUnsafe<L, R> x) =>
            HashableEitherUnsafe<HashableDefault<L?>, HashableDefault<R?>, L, R>.GetHashCode(x);
    }
}
