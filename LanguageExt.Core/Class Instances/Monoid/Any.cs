using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances;

/// <summary>
/// Booleans form a monoid under conjunction.
/// </summary>
public struct Any : Monoid<bool>, Bool<bool>
{
    [Pure]
    public static bool Append(bool x, bool y) => x || y;

    [Pure]
    public static bool Empty() => false;

    [Pure]
    public static bool And(bool a, bool b) =>
        TBool.And(a, b);

    [Pure]
    public static bool BiCondition(bool a, bool b) =>
        TBool.BiCondition(a, b);

    [Pure]
    public static bool False() => false;

    [Pure]
    public static bool Implies(bool a, bool b) =>
        TBool.Implies(a, b);

    [Pure]
    public static bool Not(bool a) =>
        TBool.Not(a);

    [Pure]
    public static bool Or(bool a, bool b) =>
        TBool.Or(a, b);

    [Pure]
    public static bool True() =>
        true;

    [Pure]
    public static bool XOr(bool a, bool b) =>
        TBool.XOr(a, b);
}
