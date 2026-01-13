using System.Diagnostics.Contracts;
using System.Numerics;
using LanguageExt.Ranges;
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
    public static Range<A> Range<A>(A from, long count)
        where A : struct, INumber<A> =>
        L.Range.fromCount<Numbers<A>, A, A>(from, count);
    
    /// <summary>
    /// Construct a new `Range`
    /// </summary>
    /// <param name="from">Initial value of the range</param>
    /// <param name="count">Total number of elements in the range</param>
    /// <param name="step">Difference between each element in the range</param>
    /// <typeparam name="A">Element type</typeparam>
    /// <returns>Range</returns>
    [Pure]
    public static Range<A> Range<A>(A from, long count, A step) 
        where A : struct, INumber<A> =>
        L.Range.fromCount<Numbers<A>, A, A>(from, count, step);

    /// <summary>
    /// Lazily generate a range of chars.  
    /// 
    ///   Remarks:
    ///     Can go in a positive direction ('a'..'z') as well as negative ('z'..'a')
    /// </summary>
    [Pure]
    public static Range<char> Range(char from, char to) =>
        L.Range.fromMinMax<Chars, char, int>(from, to);

    /// <summary>
    /// Lazily generate a range of chars.  
    /// 
    ///   Remarks:
    ///     Can go in a positive direction ('a'..'z') as well as negative ('z'..'a')
    /// </summary>
    [Pure]
    public static Range<char> Range(char from, char to, int step) =>
        L.Range.fromMinMax<Chars, char, int>(from, to, (char)step);

    /// <summary>
    /// Lazily generate a range of chars.  
    /// 
    ///   Remarks:
    ///     Can go in a positive direction ('a'..'z') as well as negative ('z'..'a')
    /// </summary>
    [Pure]
    public static Range<char> Range(char from, int count) =>
        L.Range.fromCount<Chars, char, int>(from, count);

    /// <summary>
    /// Lazily generate a range of chars.  
    /// 
    ///   Remarks:
    ///     Can go in a positive direction ('a'..'z') as well as negative ('z'..'a')
    /// </summary>
    [Pure]
    public static Range<char> Range(char from, int count, int step) =>
        L.Range.fromCount<Chars, char, int>(from, count, (char)step);
}
