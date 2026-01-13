using LanguageExt.Ranges;
using LanguageExt.Traits;
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
    public static Range<char> singleton(char value) =>
        singleton<Chars, char, int>(value);

    /// <summary>
    /// Construct a new range
    /// </summary>
    /// <param name="from">The minimum value in the range</param>
    /// <param name="to">The maximum value in the range</param>
    [Pure]
    public static Range<char> fromMinMax(char from, char to) =>
        fromMinMax<Chars, char, int>(from, to);

    /// <summary>
    /// Construct a new range
    /// </summary>
    /// <param name="from">The minimum value in the range</param>
    /// <param name="to">The maximum value in the range</param>
    /// <param name="step">The size of each step in the range</param>
    [Pure]
    public static Range<char> fromMinMax(char from, char to, char step) =>
        fromMinMax<Chars, char, int>(from, to, to);

    /// <summary>
    /// Construct a new range
    /// </summary>
    /// <param name="from">The minimum value in the range</param>
    /// <param name="count">The number of items in the range</param>
    [Pure]
    public static Range<char> fromCount(char from, long count) =>
        fromCount<Chars, char, int>(from, count);

    /// <summary>
    /// Construct a new range
    /// </summary>
    /// <param name="from">The minimum value in the range</param>
    /// <param name="count">The number of items in the range</param>
    /// <param name="step">The size of each step in the range</param>
    [Pure]
    public static Range<char> fromCount(char from, long count, char step) =>
        fromCount<Chars, char, int>(from, count, step);
}
