namespace LanguageExt.Traits;

/// <summary>
/// Mimics `finally` in a `try/finally` operation
/// </summary>
public interface Final<F>
    where F : Final<F>
{
    /// <summary>
    /// Run a `finally` operation after the `ma` operation regardless of whether `ma` succeeds or not.
    /// </summary>
    /// <param name="ma">Primary operation</param>
    /// <param name="finally">Finally operation</param>
    /// <returns>Result of primary operation</returns>
    public static abstract K<F, A> Finally<X, A>(K<F, A> fa, K<F, X> @finally);
}
