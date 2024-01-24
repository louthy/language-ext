using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances;

/// <summary>
/// Option type hash
/// </summary>
public struct HashableOptionUnsafe<A> : Hashable<OptionUnsafe<A>>
{
    /// <summary>
    /// Get hash code of the value
    /// </summary>
    /// <param name="x">Value to get the hash code of</param>
    /// <returns>The hash code of x</returns>
    public static int GetHashCode(OptionUnsafe<A> x) =>
        HashableOptionalUnsafe<MOptionUnsafe<A>, OptionUnsafe<A>, A>.GetHashCode(x);
}
