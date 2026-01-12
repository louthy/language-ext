using System.Diagnostics.Contracts;
using System.Numerics;
using L = LanguageExt;

namespace LanguageExt;

public static partial class Prelude
{
    /// <summary>
    /// Construct a new `Range`
    /// </summary>
    /// <param name="from">Initial value of the range</param>
    /// <param name="count">Total number of elements in the range</param>
    /// <typeparam name="A">Element type</typeparam>
    /// <returns>Range</returns>
    [Pure]
    public static Range<A> Range<A>(A from, A count)
        where A :
            IComparisonOperators<A, A, bool>,
            INumberBase<A> =>
        L.Range.fromCount(from, count);
    
    /// <summary>
    /// Construct a new `Range`
    /// </summary>
    /// <param name="from">Initial value of the range</param>
    /// <param name="count">Total number of elements in the range</param>
    /// <param name="step">Difference between each element in the range</param>
    /// <typeparam name="A">Element type</typeparam>
    /// <returns>Range</returns>
    [Pure]
    public static Range<A> Range<A>(A from, A count, A step) 
        where A :
            IComparisonOperators<A, A, bool>,
            INumberBase<A> =>
        L.Range.fromCount(from, count, step);

    /// <summary>
    /// Lazily generate a range of chars.  
    /// 
    ///   Remarks:
    ///     Can go in a positive direction ('a'..'z') as well as negative ('z'..'a')
    /// </summary>
    [Pure]
    public static Range<char> Range(char from, char to) =>
        L.Range.fromMinMax(from, to);
}
