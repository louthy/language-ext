namespace LanguageExt.Traits;

public static class RWSTExtensions
{
    extension<R, W, S, X, M, A>(K<RWST<R, W, S, M>, A> _)
        where M : Monad<M>, Final<M>
        where W : Monoid<W>
    {
        /// <summary>
        /// Run a `finally` operation after the main operation regardless of whether it succeeds or not.
        /// </summary>
        /// <param name="lhs">Primary operation</param>
        /// <param name="rhs">Finally operation</param>
        /// <returns>Result of primary operation</returns>
        public static RWST<R, W, S, M, A> operator |(K<RWST<R, W, S, M>, A> lhs, Finally<M, X> rhs) =>
            new(env => lhs.As().runRWST(env) | rhs);
    }
}
