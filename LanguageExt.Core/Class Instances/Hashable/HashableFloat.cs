using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances;

/// <summary>
/// Floating point hash
/// </summary>
public struct HashableFloat : Hashable<float>
{
    /// <summary>
    /// Get hash code of the value
    /// </summary>
    /// <param name="x">Value to get the hash code of</param>
    /// <returns>The hash code of x</returns>
    [Pure]
    public static int GetHashCode(float x) =>
        x.GetHashCode();
}
