namespace LanguageExt.Traits;

public static class StateTExtensions
{
    extension<S, X, M, A>(K<StateT<S, M>, A> _)
        where M : Monad<M>, Final<M>
    {
        /// <summary>
        /// Run a `finally` operation after the main operation regardless of whether it succeeds or not.
        /// </summary>
        /// <param name="lhs">Primary operation</param>
        /// <param name="rhs">Finally operation</param>
        /// <returns>Result of primary operation</returns>
        public static StateT<S, M, A> operator |(K<StateT<S, M>, A> lhs, Finally<M, X> rhs) =>
            new(s => lhs.As().runState(s) | rhs);
    }
}
