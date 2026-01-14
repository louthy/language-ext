using System.Numerics;
using LanguageExt.Ranges;
using System.Diagnostics.Contracts;

namespace LanguageExt;

public partial class Range
{
    /// <summary>
    /// A single value range
    /// </summary>
    /// <param name="value">From and To</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <returns>Range that will yield a single value</returns>
    public static Range<A> singleton<A>(A value)
        where A : struct, INumber<A> =>
        singleton<Numbers<A>, A, A>(value);
    
    /// <summary>
    /// Construct a new range
    /// </summary>
    /// <param name="from">The minimum value in the range</param>
    /// <param name="to">The maximum value in the range</param>
    [Pure]
    public static Range<A> fromMinMax<A>(A from, A to) 
        where A : struct, INumber<A> =>
        fromMinMax<Numbers<A>, A, A>(from, to);
   
    /// <summary>
    /// Construct a new range
    /// </summary>
    /// <param name="from">The minimum value in the range</param>
    /// <param name="to">The maximum value in the range</param>
    /// <param name="step">The size of each step in the range</param>
    [Pure]
    public static Range<A> fromMinMax<A>(A from, A to, A step) 
        where A : struct, INumber<A> =>
        fromMinMax<Numbers<A>, A, A>(from, to, to);

    /// <summary>
    /// Construct a new range
    /// </summary>
    /// <param name="from">The minimum value in the range</param>
    /// <param name="count">The number of items in the range</param>
    [Pure]
    public static Range<A> fromCount<A>(A from, long count) 
        where A : struct, INumber<A> =>
        fromCount<Numbers<A>, A, A>(from, count);
        
    /// <summary>
    /// Construct a new range
    /// </summary>
    /// <param name="from">The minimum value in the range</param>
    /// <param name="count">The number of items in the range</param>
    /// <param name="step">The size of each step in the range</param>
    [Pure]
    public static Range<A> fromCount<A>(A from, long count, A step) 
        where A : struct, INumber<A> =>
        fromCount<Numbers<A>, A, A>(from, count, step);
}
