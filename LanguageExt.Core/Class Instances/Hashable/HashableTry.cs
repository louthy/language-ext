using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace LanguageExt.ClassInstances;

/// <summary>
/// Hash of any values bound by the Try monad
/// </summary>
public struct HashableTry<HashA, A> : Hashable<Try<A>>
    where HashA : Hashable<A>
{
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetHashCode(Try<A> x)
    {
        var res = x.Try();
        return res.IsFaulted ? 0 : HashA.GetHashCode(res.Value);
    }
}

/// <summary>
/// Hash of any values bound by the Try monad
/// </summary>
public struct HashableTry<A> : Hashable<Try<A>>
{
    [Pure]
    public static int GetHashCode(Try<A> x) =>
        HashableTry<HashableDefault<A>, A>.GetHashCode(x);
}
