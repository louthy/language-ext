using System.Diagnostics.Contracts;
using LanguageExt.Traits;

namespace LanguageExt.ClassInstances;

/// <summary>
/// Ordering class instance for all record types
/// </summary>
/// <typeparam name="A">Record type</typeparam>
public struct OrdRecord<A> : Ord<A> where A : Record<A>
{
    [Pure]
    public static int Compare(A x, A y) =>
        RecordType<A>.Compare(x, y);

    [Pure]
    public static bool Equals(A x, A y) =>
        RecordType<A>.EqualityTyped(x, y);

    [Pure]
    public static int GetHashCode(A x) =>
        RecordType<A>.Hash(x);
}
