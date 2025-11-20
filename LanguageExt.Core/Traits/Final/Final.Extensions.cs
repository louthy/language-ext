namespace LanguageExt.Traits;

public static class FinalExtensions
{
    /// <summary>
    /// Run a `finally` operation after the `ma` operation regardless of whether `ma` succeeds or not.
    /// </summary>
    /// <param name="ma">Primary operation</param>
    /// <param name="finally">Finally operation</param>
    /// <returns>Result of primary operation</returns>
    public static K<F, A> Finally<X, F, A>(this K<F, A> ma, K<F, X> @finally)
        where F : Final<F> =>
        F.Finally(ma, @finally);
}
