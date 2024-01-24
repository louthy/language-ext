using System;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances;

/// <summary>
/// DateTime hash
/// </summary>
public struct HashableDateTime : Hashable<DateTime>
{
    /// <summary>
    /// Get hash code of the value
    /// </summary>
    /// <param name="x">Value to get the hash code of</param>
    /// <returns>The hash code of x</returns>
    [Pure]
    public static int GetHashCode(DateTime x) =>
        x.GetHashCode();
}
