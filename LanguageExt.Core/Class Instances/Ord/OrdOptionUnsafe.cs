using System.Diagnostics.Contracts;
using LanguageExt.TypeClasses;

namespace LanguageExt.ClassInstances;

public struct OrdOptionUnsafe<OrdA, A> : Ord<OptionUnsafe<A>>
    where OrdA : Ord<A?>
{
    [Pure]
    public static int Compare(OptionUnsafe<A> x, OptionUnsafe<A> y) =>
        OrdOptionalUnsafe<OrdA, MOptionUnsafe<A>, OptionUnsafe<A>, A>.Compare(x, y);

    [Pure]
    public static bool Equals(OptionUnsafe<A> x, OptionUnsafe<A> y) =>
        OrdOptionalUnsafe<OrdA, MOptionUnsafe<A>, OptionUnsafe<A>, A>.Equals(x, y);

    [Pure]
    public static int GetHashCode(OptionUnsafe<A> x) =>
        OrdOptionalUnsafe<OrdA, MOptionUnsafe<A>, OptionUnsafe<A>, A>.GetHashCode(x);
}

public struct OrdOptionUnsafe<A> : Ord<OptionUnsafe<A>>
{
    [Pure]
    public static int Compare(OptionUnsafe<A> x, OptionUnsafe<A> y) =>
        OrdOptionalUnsafe<MOptionUnsafe<A>, OptionUnsafe<A>, A>.Compare(x, y);

    [Pure]
    public static bool Equals(OptionUnsafe<A> x, OptionUnsafe<A> y) =>
        OrdOptionalUnsafe<MOptionUnsafe<A>, OptionUnsafe<A>, A>.Equals(x, y);

    [Pure]
    public static int GetHashCode(OptionUnsafe<A> x) =>
        OrdOptionalUnsafe<MOptionUnsafe<A>, OptionUnsafe<A>, A>.GetHashCode(x);
}
