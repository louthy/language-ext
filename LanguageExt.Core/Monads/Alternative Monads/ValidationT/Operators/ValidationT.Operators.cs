using LanguageExt.Traits;

namespace LanguageExt;

public static partial class ValidationTExtensions
{
    extension<FF, M, A>(K<ValidationT<FF, M>, A> _)
        where M : Monad<M>
    {
        /// <summary>
        /// Downcast operator
        /// </summary>
        public static ValidationT<FF, M, A> operator +(K<ValidationT<FF, M>, A> ma) =>
            (ValidationT<FF, M, A>)ma;
    }
}
