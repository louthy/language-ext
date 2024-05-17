using LanguageExt.Traits;

namespace LanguageExt.ClassInstances;

/// <summary>
/// Always returns true for equality checks and 0 for ordering
/// </summary>
public struct OrdTrue<A> : Ord<A>
{
    public static int Compare(A x, A y) =>
        0;

    public static bool Equals(A x, A y) =>
        true;

    public static int GetHashCode(A x) =>
        OrdDefault<A>.GetHashCode(x);
}
