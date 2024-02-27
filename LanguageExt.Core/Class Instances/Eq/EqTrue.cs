using System.Diagnostics.Contracts;
using LanguageExt.Traits;

namespace LanguageExt.ClassInstances;

/// <summary>
/// Always returns true for equality checks
/// </summary>
public struct EqTrue<A> : Eq<A>
{
    [Pure]
    public static bool Equals(A x, A y) =>
        true;

    [Pure]
    public static int GetHashCode(A x) =>
        EqDefault<A>.GetHashCode(x);
}
