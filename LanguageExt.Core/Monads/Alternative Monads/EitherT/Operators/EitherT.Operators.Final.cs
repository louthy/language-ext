namespace LanguageExt.Traits;

public static class EitherTExtensions
{
    extension<X, L, M, A>(K<EitherT<L, M>, A> _)
        where M : Monad<M>, Final<M>
    {
        /// <summary>
        /// Run a `finally` operation after the main operation regardless of whether it succeeds or not.
        /// </summary>
        /// <param name="lhs">Primary operation</param>
        /// <param name="rhs">Finally operation</param>
        /// <returns>Result of primary operation</returns>
        public static EitherT<L, M, A> operator |(K<EitherT<L, M>, A> lhs, Finally<M, X> rhs) =>
            new (lhs.As().runEither.Finally(rhs.Operation));
    }
}
