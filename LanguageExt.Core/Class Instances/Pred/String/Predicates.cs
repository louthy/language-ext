using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;
using System.Text.RegularExpressions;
using static LanguageExt.Prelude;

namespace LanguageExt.ClassInstances.Pred
{
    public struct StrLen<NMin, NMax> : Pred<string>
            where NMin : struct, Const<int>
            where NMax : struct, Const<int>
    {
        public static readonly StrLen<NMin, NMax> Is = default(StrLen<NMin, NMax>);

        [Pure]
        public bool True(string value) =>
            Range<TInt, int, NMin, NMax>.Is.True(value?.Length ?? 0);
    }

    public struct Matches<PATTERN> : Pred<string>
    where PATTERN : Const<string>
    {
        public static readonly Matches<PATTERN> Is = default;

        [Pure]
        public bool True(string value) =>
            Regex.IsMatch(value, default(PATTERN).Value, RegexOptions.Compiled, 3 * seconds);
    }

}
