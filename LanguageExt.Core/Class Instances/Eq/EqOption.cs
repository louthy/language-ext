using System.Diagnostics.Contracts;
using LanguageExt.Traits;

namespace LanguageExt.ClassInstances;

/// <summary>
/// Option type equality
/// </summary>
public struct EqOption<A> : Eq<Option<A>>
{
    [Pure]
    public static bool Equals(Option<A> x, Option<A> y) =>
        x.Equals(y);

    [Pure]
    public static int GetHashCode(Option<A> x) =>
        HashableOption<A>.GetHashCode(x);
}
