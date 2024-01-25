using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;
using System.Text.RegularExpressions;
using static LanguageExt.UnitsOfMeasure;

namespace LanguageExt.ClassInstances.Pred;

public struct StrLen<NMin, NMax> : Pred<string>
    where NMin : Const<int>
    where NMax : Const<int>
{
    [Pure]
    public static bool True(string value) =>
        Range<TInt, int, NMin, NMax>.True(value?.Length ?? 0);
}

public struct Matches<PATTERN> : Pred<string>
    where PATTERN : Const<string>
{
    [Pure]
    public static bool True(string value) =>
        Regex.IsMatch(value, PATTERN.Value, RegexOptions.Compiled, 3 * seconds);
}
