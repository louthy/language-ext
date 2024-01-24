using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace LanguageExt.ClassInstances;

/// <summary>
/// String hashing
/// </summary>
public struct HashableString : Hashable<string>
{
    /// <summary>
    /// Get hash code of the value
    /// </summary>
    /// <param name="x">Value to get the hash code of</param>
    /// <returns>The hash code of x</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetHashCode(string x) =>
        x == null ? 0 : x.GetHashCode();
}
    
/// <summary>
/// String hashing (invariant culture)
/// </summary>
public struct HashableStringInvariantCulture : Hashable<string>
{
    /// <summary>
    /// Get hash code of the value
    /// </summary>
    /// <param name="x">Value to get the hash code of</param>
    /// <returns>The hash code of x</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetHashCode(string x) =>
        StringComparer.InvariantCulture.GetHashCode(x);
}
    
        
/// <summary>
/// String hashing (invariant culture, ignore case)
/// </summary>
public struct HashableStringInvariantCultureIgnoreCase : Hashable<string>
{
    /// <summary>
    /// Get hash code of the value
    /// </summary>
    /// <param name="x">Value to get the hash code of</param>
    /// <returns>The hash code of x</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetHashCode(string x) =>
        x.IsNull() ? 0 : StringComparer.InvariantCultureIgnoreCase.GetHashCode(x);
}

/// <summary>
/// String equality (ordinal, ignore case)
/// </summary>
public struct HashableStringOrdinalIgnoreCase : Hashable<string>
{
    /// <summary>
    /// Get hash code of the value
    /// </summary>
    /// <param name="x">Value to get the hash code of</param>
    /// <returns>The hash code of x</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetHashCode(string x) =>
        x.IsNull() ? 0 : StringComparer.OrdinalIgnoreCase.GetHashCode(x);
}

/// <summary>
/// String equality (ordinal)
/// </summary>
public struct HashableStringOrdinal : Hashable<string>
{
    /// <summary>
    /// Get hash code of the value
    /// </summary>
    /// <param name="x">Value to get the hash code of</param>
    /// <returns>The hash code of x</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetHashCode(string x) =>
        x.IsNull() ? 0 : StringComparer.Ordinal.GetHashCode(x);
}

/// <summary>
/// String equality (current culture, ignore case)
/// </summary>
public struct HashableStringCurrentCultureIgnoreCase : Hashable<string>
{
    /// <summary>
    /// Get hash code of the value
    /// </summary>
    /// <param name="x">Value to get the hash code of</param>
    /// <returns>The hash code of x</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetHashCode(string x) =>
        x.IsNull() ? 0 : StringComparer.CurrentCultureIgnoreCase.GetHashCode(x);
}

/// <summary>
/// String equality (current culture)
/// </summary>
public struct HashableStringCurrentCulture : Hashable<string>
{
    /// <summary>
    /// Get hash code of the value
    /// </summary>
    /// <param name="x">Value to get the hash code of</param>
    /// <returns>The hash code of x</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetHashCode(string x) =>
        x.IsNull() ? 0 : StringComparer.CurrentCulture.GetHashCode(x);
}
