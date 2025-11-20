namespace LanguageExt.Traits;

public static class OptionTExtensions
{
    extension<X, M, A>(K<OptionT<M>, A> _)
        where M : Monad<M>, Final<M>
    {
        /// <summary>
        /// Run a `finally` operation after the main operation regardless of whether it succeeds or not.
        /// </summary>
        /// <param name="lhs">Primary operation</param>
        /// <param name="rhs">Finally operation</param>
        /// <returns>Result of primary operation</returns>
        public static OptionT<M, A> operator |(K<OptionT<M>, A> lhs, Finally<M, X> rhs) =>
            new (lhs.As().runOption.Finally(rhs.Operation));
    }
}
