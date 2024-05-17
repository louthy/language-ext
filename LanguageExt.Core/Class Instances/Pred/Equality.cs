using LanguageExt.Traits;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances.Pred;

public struct Equal<EQ, A, CONST> : Pred<A>
    where EQ    : Eq<A>
    where CONST : Const<A>
{
    [Pure]
    public static bool True(A value) =>
        EQ.Equals(value, CONST.Value);
}
