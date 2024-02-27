using System.Diagnostics.Contracts;
using LanguageExt.Traits;

namespace LanguageExt.ClassInstances;

/// <summary>
/// Equality class instance for all record types
/// </summary>
/// <typeparam name="A">Record type</typeparam>
public struct EqRecord<A> : Eq<A> where A : Record<A>
{
    [Pure]
    public static bool Equals(A x, A y) =>
        RecordType<A>.EqualityTyped(x, y);

    [Pure]
    public static int GetHashCode(A x) =>
        HashableRecord<A>.GetHashCode(x);
}
