using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances.Pred;

public struct GreaterThan<ORD, A, CONST> : Pred<A>
    where ORD : Ord<A>
    where CONST : Const<A>
{
    [Pure]
    public static bool True(A value) =>
        ORD.Compare(value, CONST.Value) > 0;
}

public struct LessThan<ORD, A, CONST> : Pred<A>
    where ORD : Ord<A>
    where CONST : Const<A>
{
    [Pure]
    public static bool True(A value) =>
        ORD.Compare(value, CONST.Value) < 0;
}

public struct GreaterOrEq<ORD, A, CONST> : Pred<A>
    where ORD : Ord<A>
    where CONST : Const<A>
{
    [Pure]
    public static bool True(A value) =>
        ORD.Compare(value, CONST.Value) >= 0;
}

public struct LessOrEq<ORD, A, CONST> : Pred<A>
    where ORD   : Ord<A>
    where CONST : Const<A>
{
    [Pure]
    public static bool True(A value) =>
        ORD.Compare(value, CONST.Value) <= 0;
}
