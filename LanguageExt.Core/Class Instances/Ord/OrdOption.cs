using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using LanguageExt.TypeClasses;

namespace LanguageExt.ClassInstances;

public struct OrdOption<OrdA, A> : Ord<Option<A>>
    where OrdA : Ord<A>
{
    [Pure]
    public static int Compare(Option<A> x, Option<A> y) =>
        OrdOptional<OrdA, MOption<A>, Option<A>, A>.Compare(x, y);

    [Pure]
    public static bool Equals(Option<A> x, Option<A> y) =>
        OrdOptional<OrdA, MOption<A>, Option<A>, A>.Equals(x, y);

    [Pure]
    public static int GetHashCode(Option<A> x) =>
        OrdOptional<OrdA, MOption<A>, Option<A>, A>.GetHashCode(x);
}

public struct OrdOption<A> : Ord<Option<A>>
{
    [Pure]
    public static int Compare(Option<A> x, Option<A> y) =>
        OrdOptional<MOption<A>, Option<A>, A>.Compare(x, y);

    [Pure]
    public static bool Equals(Option<A> x, Option<A> y) =>
        OrdOptional<MOption<A>, Option<A>, A>.Equals(x, y);

    [Pure]
    public static int GetHashCode(Option<A> x) =>
        OrdOptional<MOption<A>, Option<A>, A>.GetHashCode(x);
}
