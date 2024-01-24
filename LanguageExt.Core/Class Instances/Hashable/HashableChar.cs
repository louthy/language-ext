using System.Diagnostics.Contracts;
using System.Threading.Tasks;

namespace LanguageExt.ClassInstances;

/// <summary>
/// Char hash
/// </summary>
public struct HashableChar : Hashable<char>
{
    /// <summary>
    /// Get hash code of the value
    /// </summary>
    /// <param name="x">Value to get the hash code of</param>
    /// <returns>The hash code of x</returns>
    [Pure]
    public static int GetHashCode(char x) =>
        x.GetHashCode();
}

/// <summary>
/// Char hash (ordinal, ignore case)
/// </summary>
public struct HashableCharOrdinalIgnoreCase : Hashable<char>
{
    /// <summary>
    /// Get hash code of the value
    /// </summary>
    /// <param name="x">Value to get the hash code of</param>
    /// <returns>The hash code of x</returns>
    [Pure]
    public static int GetHashCode(char x) =>
        x is >= 'a' and <= 'z' ? x - 0x20 : x;
}
