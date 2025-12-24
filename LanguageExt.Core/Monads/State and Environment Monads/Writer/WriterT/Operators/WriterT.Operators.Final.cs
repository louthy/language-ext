namespace LanguageExt.Traits;

public static class WriterTExtensions
{
    extension<W, X, M, A>(K<WriterT<W, M>, A> _)
        where M : Monad<M>, Final<M>
        where W : Monoid<W>
    {
        /// <summary>
        /// Run a `finally` operation after the main operation regardless of whether it succeeds or not.
        /// </summary>
        /// <param name="lhs">Primary operation</param>
        /// <param name="rhs">Finally operation</param>
        /// <returns>Result of primary operation</returns>
        public static WriterT<W, M, A> operator |(K<WriterT<W, M>, A> lhs, Finally<M, X> rhs) =>
            new(output => lhs.As().runWriter(output) | rhs);
    }
}
