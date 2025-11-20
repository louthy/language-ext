using LanguageExt.Traits;

namespace LanguageExt;

public static partial class EitherExtensions
{
    extension<L, A>(K<Either<L>, A> _)
    {
        /// <summary>
        /// Downcast operator
        /// </summary>
        public static Either<L, A> operator +(K<Either<L>, A> ma) =>
            (Either<L, A>)ma;
    }
}
