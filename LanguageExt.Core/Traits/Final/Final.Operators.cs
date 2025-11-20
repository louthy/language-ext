using LanguageExt.Traits;

namespace LanguageExt;

public static partial class FinalExtensions
{
    extension<X, F, A>(K<F, A> _)
        where F : Final<F>
    {
        /// <summary>
        /// Run a `finally` operation after the main operation regardless of whether it succeeds or not.
        /// </summary>
        /// <param name="lhs">Primary operation</param>
        /// <param name="rhs">Finally operation</param>
        /// <returns>Result of primary operation</returns>
        public static K<F, A> operator |(K<F, A> lhs, Finally<F, X> rhs) =>
            lhs.Finally(rhs.Operation);
    }
}
