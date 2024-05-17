using LanguageExt.Traits;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances.Pred;

public struct AnySize : Pred<ListInfo>
{
    [Pure]
    public static bool True(ListInfo value) =>
        true;
}
