using System.Diagnostics.Contracts;
using System.Numerics;
using LanguageExt.Traits;

namespace LanguageExt;

public static class RangeExtensions
{
    /// <summary>
    /// Convert kind to concrete type
    /// </summary>
    public static Range<A> As<A>(this K<Range, A> ma) =>  
        (Range<A>)ma;
    
    /// <summary>
    /// Returns true if the value provided is in range
    /// </summary>
    /// <param name="value">Value to test</param>
    /// <returns>True if the value provided is in range</returns>
    [Pure]
    public static bool InRange<A>(this K<Range, A> ma, A value)
        where A : IComparisonOperators<A, A, bool>
    {
        var range = ma.As();
        var from  = range.From > range.To ? range.To : range.From;
        var to    = range.From > range.To ? range.From : range.To;
        return value >= from && value <= to;
    }

    /// <summary>
    /// Returns true if the range provided overlaps this range
    /// </summary>
    /// <param name="other">The range to test</param>
    /// <returns>True if the range provided overlaps this range</returns>
    [Pure]
    public static bool Overlaps<A>(this K<Range, A> ma, Range<A> other)
        where A : IComparisonOperators<A, A, bool>
    {
        var range = ma.As();
        var xfrom = range.From > range.To ? range.To : range.From;
        var xto   = range.From > range.To ? range.From : range.To;
        var yfrom = other.From > other.To ? other.To : other.From;
        var yto   = other.From > other.To ? other.From : other.To;
        return xfrom < yto && yfrom < xto;
    }
}
