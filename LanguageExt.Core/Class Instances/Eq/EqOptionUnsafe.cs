using System.Diagnostics.Contracts;
using LanguageExt.TypeClasses;

namespace LanguageExt.ClassInstances;

/// <summary>
/// Option type equality
/// </summary>
public struct EqOptionUnsafe<A> : Eq<OptionUnsafe<A>>
{
    [Pure]
    public static bool Equals(OptionUnsafe<A> x, OptionUnsafe<A> y) =>
        EqOptionalUnsafe<MOptionUnsafe<A>, OptionUnsafe<A>, A>.Equals(x, y);

    [Pure]
    public static int GetHashCode(OptionUnsafe<A> x) =>
        HashableOptionUnsafe<A>.GetHashCode(x);
}
