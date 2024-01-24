using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances.Pred
{
    public struct CountRange<MIN, MAX> : Pred<ListInfo>
        where MIN : Const<int>
        where MAX : Const<int>
    {
        [Pure]
        public static bool True(ListInfo value) =>
            value.Count >= MIN.Value && value.Count <= MAX.Value;
    }
}
