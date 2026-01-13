namespace LanguageExt.Traits;

public interface Range<R, A, in S> : Range<A>
    where R : Range<R, A, S>
{
    /// <summary>
    /// Construct a range from min and max values
    /// </summary>
    /// <remarks>
    /// The `from` and `to` parameters are inclusive and allow for `from` being larger than `to`.
    /// </remarks>
    /// <param name="from">Beginning of the range</param>
    /// <param name="to">End of the range</param>
    /// <returns>Range</returns>
    static abstract Range<A> FromMinMax(A from, A to);
    
    /// <summary>
    /// Construct a range from min and max values
    /// </summary>
    /// <remarks>
    /// The `from` and `to` parameters are inclusive and allow for `from` being larger than `to`.
    /// </remarks>
    /// <param name="from">Beginning of the range</param>
    /// <param name="to">End of the range</param>
    /// <param name="step">The difference between elements in the range</param>
    /// <returns>Range</returns>
    static abstract Range<A> FromMinMax(A from, A to, S step);

    /// <summary>
    /// Construct a range from a starting value and a count
    /// </summary>
    /// <param name="from">Beginning of the range</param>
    /// <param name="count">The number of elements in the range</param>
    /// <returns>Range</returns>
    static abstract Range<A> FromCount(A from, long count);

    /// <summary>
    /// Construct a range from a starting value and a count
    /// </summary>
    /// <param name="from">Beginning of the range</param>
    /// <param name="count">The number of elements in the range</param>
    /// <param name="step">The difference between elements in the range</param>
    /// <returns>Range</returns>
    static abstract Range<A> FromCount(A from, long count, S step);
}
