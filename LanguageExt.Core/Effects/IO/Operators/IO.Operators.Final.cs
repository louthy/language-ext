namespace LanguageExt.Traits;

public static class IOExtensions
{
    extension<X, A>(K<IO, A> _)
    {
        /// <summary>
        /// Run a `finally` operation after the main operation regardless of whether it succeeds or not.
        /// </summary>
        /// <param name="lhs">Primary operation</param>
        /// <param name="rhs">Finally operation</param>
        /// <returns>Result of primary operation</returns>
        public static IO<A> operator |(K<IO, A> lhs, Finally<IO, X> rhs) =>
            lhs.Finally(rhs.Operation).As();
    }
}
