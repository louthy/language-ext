using LanguageExt.Traits;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances.Pred;

public struct Range<ORD, A, MIN, MAX> : Pred<A>
    where ORD : Ord<A>
    where MIN : Const<A>
    where MAX : Const<A>
{
    [Pure]
    public static bool True(A value) =>
        GreaterOrEq<ORD, A, MIN>.True(value) && LessOrEq<ORD, A, MAX>.True(value);
}
