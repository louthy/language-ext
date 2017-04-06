using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;

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
}
