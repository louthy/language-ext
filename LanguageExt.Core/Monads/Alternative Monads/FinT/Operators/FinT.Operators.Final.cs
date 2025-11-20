namespace LanguageExt.Traits;

public static class FinTExtensions
{
    extension<X, M, A>(K<FinT<M>, A> _)
        where M : Monad<M>, Final<M>
    {
        /// <summary>
        /// Run a `finally` operation after the main operation regardless of whether it succeeds or not.
        /// </summary>
        /// <param name="lhs">Primary operation</param>
        /// <param name="rhs">Finally operation</param>
        /// <returns>Result of primary operation</returns>
        public static FinT<M, A> operator |(K<FinT<M>, A> lhs, Finally<M, X> rhs) =>
            new (lhs.As().runFin.Finally(rhs.Operation));
    }
}
