namespace LanguageExt.Traits;

public static class Final
{
    /// <summary>
    /// Create a `finally` operation that can be used as the right-hand side of a `|` operator to
    /// cause a final operation to be run regardless of whether the primary operation succeeds or not.
    /// </summary>
    /// <param name="finally">Finally operation</param>
    /// <returns>Result of primary operation</returns>
    public static Finally<F, X> final<F, X>(K<F, X> @finally)
        where F : Final<F> =>
        new(@finally);
}
