using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances.Pred;

public struct MaxCount<MAX> : Pred<ListInfo>
    where MAX : Const<int>
{
    [Pure]
    public static bool True(ListInfo value) =>
        value.Count <= MAX.Value;
}
