using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace LanguageExt.ClassInstances;

/// <summary>
/// Hash of any type in the TryOption trait
/// </summary>
public struct HashableTryOption<HashA, A> : Hashable<TryOption<A>>
    where HashA : Hashable<A>
{
    /// <summary>
    /// Get hash code of the value
    /// </summary>
    /// <param name="x">Value to get the hash code of</param>
    /// <returns>The hash code of x</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetHashCode(TryOption<A> x)
    {
        var res = x.Try();
        #nullable disable
        return res.IsFaulted || res.Value.IsNone ? 0 : HashA.GetHashCode(res.Value.Value);
        #nullable restore
    }
}

/// <summary>
/// Hash of any type in the TryOption trait
/// </summary>
public struct HashableTryOption<A> : Hashable<TryOption<A>>
{
        
    /// <summary>
    /// Get hash code of the value
    /// </summary>
    /// <param name="x">Value to get the hash code of</param>
    /// <returns>The hash code of x</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetHashCode(TryOption<A> x) =>
        HashableTryOption<HashableDefault<A>, A>.GetHashCode(x);
}
