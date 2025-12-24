using LanguageExt.Traits;

namespace LanguageExt;

public static partial class ValidationExtensions
{
    extension<F, A>(K<Validation<F>, A> _)
    {
        /// <summary>
        /// Downcast operator
        /// </summary>
        public static Validation<F ,A> operator +(K<Validation<F>, A> ma) =>
            (Validation<F, A>)ma;
        
        /// <summary>
        /// Downcast operator
        /// </summary>
        public static Validation<F, A> operator >> (K<Validation<F>, A> ma, Lower lower) =>
            +ma;
    }
}
