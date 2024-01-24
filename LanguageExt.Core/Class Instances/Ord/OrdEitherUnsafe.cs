using System.Diagnostics.Contracts;
using LanguageExt.TypeClasses;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Either type hashing
    /// </summary>
    public struct OrdEitherUnsafe<OrdL, OrdR, L, R> : Ord<EitherUnsafe<L, R>>
        where OrdL : Ord<L?>
        where OrdR : Ord<R?>
    {
        /// <summary>
        /// Ordering test
        /// </summary>
        [Pure]
        public static int Compare(EitherUnsafe<L, R> x, EitherUnsafe<L, R> y) =>
            x.State switch
            {
                EitherStatus.IsRight => y.State switch
                                        {
                                            EitherStatus.IsRight => OrdR.Compare(x.RightValue, y.RightValue),
                                            EitherStatus.IsLeft  => 1,
                                            _                    => 1
                                        },
                EitherStatus.IsLeft => y.State switch
                                       {
                                           EitherStatus.IsLeft  => OrdL.Compare(x.LeftValue, y.LeftValue),
                                           EitherStatus.IsRight => -1,
                                           _                    => 1
                                       },
                EitherStatus.IsBottom => y.State == EitherStatus.IsBottom ? 0 : -1,
                _                     => 0
            };

        /// <summary>
        /// Equality test
        /// </summary>
        [Pure]
        public static bool Equals(EitherUnsafe<L, R> x, EitherUnsafe<L, R> y) =>
            EqEitherUnsafe<OrdL, OrdR, L, R>.Equals(x, y);

        /// <summary>
        /// Get hash code of the value
        /// </summary>
        /// <param name="x">Value to get the hash code of</param>
        /// <returns>The hash code of x</returns>
        [Pure]
        public static int GetHashCode(EitherUnsafe<L, R> x) => 
            HashableEitherUnsafe<OrdL, OrdR, L, R>.GetHashCode(x);
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
        public static int Compare(EitherUnsafe<L, R> x, EitherUnsafe<L, R> y) => 
            OrdEitherUnsafe<OrdDefault<L?>, OrdDefault<R?>, L, R>.Compare(x, y);
 
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
