using System;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances;

/// <summary>
/// String equality
/// </summary>
public struct EqString : Eq<string>
{
    /// <summary>
    /// Equality test
    /// </summary>
    /// <param name="a">The left hand side of the equality operation</param>
    /// <param name="b">The right hand side of the equality operation</param>
    /// <returns>True if a and b are equal</returns>
    [Pure]
    public static bool Equals(string a, string b) =>
        a == b;

    /// <summary>
    /// Get hash code of the value
    /// </summary>
    /// <param name="x">Value to get the hash code of</param>
    /// <returns>The hash code of x</returns>
    [Pure]
    public static int GetHashCode(string x) =>
        HashableString.GetHashCode(x);
}
    
/// <summary>
/// String equality (invariant culture)
/// </summary>
public struct EqStringInvariantCulture : Eq<string>
{
    /// <summary>
    /// Equality test
    /// </summary>
    /// <param name="a">The left hand side of the equality operation</param>
    /// <param name="b">The right hand side of the equality operation</param>
    /// <returns>True if a and b are equal</returns>
    [Pure]
    public static bool Equals(string a, string b) =>
        StringComparer.InvariantCulture.Equals(a, b);

    /// <summary>
    /// Get hash code of the value
    /// </summary>
    /// <param name="x">Value to get the hash code of</param>
    /// <returns>The hash code of x</returns>
    [Pure]
    public static int GetHashCode(string x) =>
        HashableStringInvariantCulture.GetHashCode(x);
}
    
/// <summary>
/// String equality (invariant culture, ignore case)
/// </summary>
public struct EqStringInvariantCultureIgnoreCase : Eq<string>
{
    /// <summary>
    /// Equality test
    /// </summary>
    /// <param name="a">The left hand side of the equality operation</param>
    /// <param name="b">The right hand side of the equality operation</param>
    /// <returns>True if a and b are equal</returns>
    [Pure]
    public static bool Equals(string a, string b) =>
        StringComparer.InvariantCultureIgnoreCase.Equals(a, b);

    /// <summary>
    /// Get hash code of the value
    /// </summary>
    /// <param name="x">Value to get the hash code of</param>
    /// <returns>The hash code of x</returns>
    [Pure]
    public static int GetHashCode(string x) =>
        HashableStringInvariantCultureIgnoreCase.GetHashCode(x);
}

/// <summary>
/// String equality (ordinal, ignore case)
/// </summary>
public struct EqStringOrdinalIgnoreCase : Eq<string>
{
    /// <summary>
    /// Equality test
    /// </summary>
    /// <param name="a">The left hand side of the equality operation</param>
    /// <param name="b">The right hand side of the equality operation</param>
    /// <returns>True if a and b are equal</returns>
    [Pure]
    public static bool Equals(string a, string b) =>
        StringComparer.OrdinalIgnoreCase.Equals(a, b);

    /// <summary>
    /// Get hash code of the value
    /// </summary>
    /// <param name="x">Value to get the hash code of</param>
    /// <returns>The hash code of x</returns>
    [Pure]
    public static int GetHashCode(string x) =>
        HashableStringOrdinalIgnoreCase.GetHashCode(x);
}

/// <summary>
/// String equality (ordinal)
/// </summary>
public struct EqStringOrdinal : Eq<string>
{
    public static readonly EqStringOrdinal Inst = default;

    /// <summary>
    /// Equality test
    /// </summary>
    /// <param name="a">The left hand side of the equality operation</param>
    /// <param name="b">The right hand side of the equality operation</param>
    /// <returns>True if a and b are equal</returns>
    [Pure]
    public static bool Equals(string a, string b) =>
        StringComparer.Ordinal.Equals(a, b);

    /// <summary>
    /// Get hash code of the value
    /// </summary>
    /// <param name="x">Value to get the hash code of</param>
    /// <returns>The hash code of x</returns>
    [Pure]
    public static int GetHashCode(string x) =>
        HashableStringOrdinal.GetHashCode(x);
}

/// <summary>
/// String equality (current culture, ignore case)
/// </summary>
public struct EqStringCurrentCultureIgnoreCase : Eq<string>
{
    /// <summary>
    /// Equality test
    /// </summary>
    /// <param name="a">The left hand side of the equality operation</param>
    /// <param name="b">The right hand side of the equality operation</param>
    /// <returns>True if a and b are equal</returns>
    [Pure]
    public static bool Equals(string a, string b) =>
        StringComparer.CurrentCultureIgnoreCase.Equals(a, b);

    /// <summary>
    /// Get hash code of the value
    /// </summary>
    /// <param name="x">Value to get the hash code of</param>
    /// <returns>The hash code of x</returns>
    [Pure]
    public static int GetHashCode(string x) =>
        HashableStringCurrentCultureIgnoreCase.GetHashCode(x);
}

/// <summary>
/// String equality (current culture)
/// </summary>
public struct EqStringCurrentCulture : Eq<string>
{
    public static readonly EqStringCurrentCulture Inst = default;

    /// <summary>
    /// Equality test
    /// </summary>
    /// <param name="a">The left hand side of the equality operation</param>
    /// <param name="b">The right hand side of the equality operation</param>
    /// <returns>True if a and b are equal</returns>
    [Pure]
    public static bool Equals(string a, string b) =>
        StringComparer.CurrentCulture.Equals(a, b);

    /// <summary>
    /// Get hash code of the value
    /// </summary>
    /// <param name="x">Value to get the hash code of</param>
    /// <returns>The hash code of x</returns>
    [Pure]
    public static int GetHashCode(string x) =>
        HashableStringCurrentCulture.GetHashCode(x);
}
