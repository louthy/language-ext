using System.Diagnostics.Contracts;
using LanguageExt.TypeClasses;

namespace LanguageExt.ClassInstances;

/// <summary>
/// Option type equality
/// </summary>
public struct EqOption<A> : Eq<Option<A>>
{
    [Pure]
    public static bool Equals(Option<A> x, Option<A> y) =>
        EqOptional<MOption<A>, Option<A>, A>.Equals(x, y);

    [Pure]
    public static int GetHashCode(Option<A> x) =>
        HashableOption<A>.GetHashCode(x);
}
