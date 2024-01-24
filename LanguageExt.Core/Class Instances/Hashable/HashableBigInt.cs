using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances;

/// <summary>
/// bigint hash
/// </summary>
public struct HashableBigInt : Hashable<bigint>
{
    /// <summary>
    /// Get hash code of the value
    /// </summary>
    /// <param name="x">Value to get the hash code of</param>
    /// <returns>The hash code of x</returns>
    [Pure]
    public static int GetHashCode(bigint x) =>
        x.GetHashCode();
}
