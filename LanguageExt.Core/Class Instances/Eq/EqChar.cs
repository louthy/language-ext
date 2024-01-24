using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances;

/// <summary>
/// Char equality
/// </summary>
public struct EqChar : Eq<char>
{
    /// <summary>
    /// Equality test
    /// </summary>
    /// <param name="x">The left hand side of the equality operation</param>
    /// <param name="y">The right hand side of the equality operation</param>
    /// <returns>True if x and y are equal</returns>
    [Pure]
    public static bool Equals(char a, char b) =>
        a == b;


    /// <summary>
    /// Get hash code of the value
    /// </summary>
    /// <param name="x">Value to get the hash code of</param>
    /// <returns>The hash code of x</returns>
    [Pure]
    public static int GetHashCode(char x) =>
        HashableChar.GetHashCode(x);
}

/// <summary>
/// Char equality (ordinal, ignore case)
/// </summary>
public struct EqCharOrdinalIgnoreCase : Eq<char>
{
    /// <summary>
    /// Equality test
    /// </summary>
    /// <param name="a">The left hand side of the equality operation</param>
    /// <param name="b">The right hand side of the equality operation</param>
    /// <returns>True if a and b are equal</returns>
    [Pure]
    public static bool Equals(char a, char b)
    {
        if (a >= 'a' && a <= 'z') a = (char)(a - 0x20);
        if (b >= 'a' && b <= 'z') b = (char)(b - 0x20);
        return a == b;
    }

    /// <summary>
    /// Get hash code of the value
    /// </summary>
    /// <param name="x">Value to get the hash code of</param>
    /// <returns>The hash code of x</returns>
    [Pure]
    public static int GetHashCode(char x) =>
        HashableCharOrdinalIgnoreCase.GetHashCode(x);
}
