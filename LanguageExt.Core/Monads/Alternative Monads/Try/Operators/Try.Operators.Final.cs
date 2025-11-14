namespace LanguageExt.Traits;

public static class TryExtensions
{
    extension<X, A>(K<Try, A> _)
    {
        /// <summary>
        /// Run a `finally` operation after the main operation regardless of whether it succeeds or not.
        /// </summary>
        /// <param name="lhs">Primary operation</param>
        /// <param name="rhs">Finally operation</param>
        /// <returns>Result of primary operation</returns>
        public static Try<A> operator |(K<Try, A> lhs, Finally<Try, X> rhs) =>
            +lhs.Finally(rhs.Operation);
    }
}
