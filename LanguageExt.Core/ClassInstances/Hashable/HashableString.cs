using System;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances
{
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
        public int GetHashCode(string x) =>
            x == null ? 0 : x.GetHashCode();
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
        public int GetHashCode(string x) =>
            StringComparer.OrdinalIgnoreCase.GetHashCode(x);
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
        public int GetHashCode(string x) =>
            StringComparer.Ordinal.GetHashCode(x);
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
        public int GetHashCode(string x) =>
            StringComparer.CurrentCultureIgnoreCase.GetHashCode(x);
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
        public int GetHashCode(string x) =>
            StringComparer.CurrentCulture.GetHashCode(x);
    }
}
