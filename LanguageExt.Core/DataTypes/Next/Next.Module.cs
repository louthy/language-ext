namespace LanguageExt;

/// <summary>
/// Module for the `Next` data-type
/// </summary>
public static class Next
{
    /// <summary>
    /// Continue the recursion
    /// </summary>
    /// <param name="value">Value to pass back to the recursive function to 'go around'</param>
    /// <typeparam name="A">Continue value type</typeparam>
    /// <typeparam name="B">Complete value type</typeparam>
    /// <returns>Next structure</returns>
    public static Next<A, B> Cont<A, B>(A value) =>
        new (1, value, default!);
    
    /// <summary>
    /// Cancel the recursion and return
    /// </summary>
    /// <param name="value">Final value</param>
    /// <typeparam name="A">Continue value type</typeparam>
    /// <typeparam name="B">Complete value type</typeparam>
    /// <returns>Next structure</returns>
    public static Next<A, B> Done<A, B>(B value) =>
        new (2, default!, value);
}
