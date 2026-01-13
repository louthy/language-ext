using System.Diagnostics.Contracts;
using LanguageExt.Ranges;
using LanguageExt.Traits;

namespace LanguageExt;

/// <summary>
/// Range module
/// </summary>
public partial class Range
{
    /// <summary>
    /// A zero element (void) range
    /// </summary>
    /// <typeparam name="R">Range trait self-type</typeparam>
    /// <typeparam name="A">Value type</typeparam>
    /// <typeparam name="S">Step type</typeparam>
    /// <returns>Range that will yield a single value</returns>
    public static Range<A> zero<R, A, S>()
        where R : Range<R, A, S> =>
        VoidRange<A, S>.Default;
    
    /// <summary>
    /// A single value range
    /// </summary>
    /// <param name="value">From and To</param>
    /// <typeparam name="R">Range trait self-type</typeparam>
    /// <typeparam name="A">Value type</typeparam>
    /// <typeparam name="S">Step type</typeparam>
    /// <returns>Range that will yield a single value</returns>
    public static Range<A> singleton<R, A, S>(A value) 
        where R : Range<R, A, S> =>
        R.FromMinMax(value, value);
    
    /// <summary>
    /// Construct a new range
    /// </summary>
    /// <param name="from">The minimum value in the range</param>
    /// <param name="to">The maximum value in the range</param>
    /// <typeparam name="R">Range trait self-type</typeparam>
    /// <typeparam name="A">Value type</typeparam>
    /// <typeparam name="S">Step type</typeparam>
    [Pure]
    public static Range<A> fromMinMax<R, A, S>(A from, A to)
        where R : Range<R, A, S> =>
        R.FromMinMax(from, to);
    
    /// <summary>
    /// Construct a new range
    /// </summary>
    /// <param name="from">The minimum value in the range</param>
    /// <param name="to">The maximum value in the range</param>
    /// <param name="step">The size of each step in the range</param>
    /// <typeparam name="R">Range trait self-type</typeparam>
    /// <typeparam name="A">Value type</typeparam>
    /// <typeparam name="S">Step type</typeparam>
    [Pure]
    public static Range<A> fromMinMax<R, A, S>(A from, A to, S step)
        where R : Range<R, A, S> =>
        R.FromMinMax(from, to);

    /// <summary>
    /// Construct a new range
    /// </summary>
    /// <param name="from">The minimum value in the range</param>
    /// <param name="count">The number of items in the range</param>
    /// <typeparam name="R">Range trait self-type</typeparam>
    /// <typeparam name="A">Value type</typeparam>
    /// <typeparam name="S">Step type</typeparam>
    [Pure]
    public static Range<A> fromCount<R, A, S>(A from, long count)
        where R : Range<R, A, S> =>
        R.FromCount(from, count);
        
    /// <summary>
    /// Construct a new range
    /// </summary>
    /// <param name="from">The minimum value in the range</param>
    /// <param name="count">The number of items in the range</param>
    /// <param name="step">The size of each step in the range</param>
    /// <typeparam name="R">Range trait self-type</typeparam>
    /// <typeparam name="A">Value type</typeparam>
    /// <typeparam name="S">Step type</typeparam>
    [Pure]
    public static Range<A> fromCount<R, A, S>(A from, long count, S step)
        where R : Range<R, A, S> =>
        R.FromCount(from, count, step);
}
