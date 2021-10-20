using System;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// String equality
    /// </summary>
    public struct EqString : Eq<string>
    {
        public static readonly EqString Inst = default(EqString);

        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="a">The left hand side of the equality operation</param>
        /// <param name="b">The right hand side of the equality operation</param>
        /// <returns>True if a and b are equal</returns>
        [Pure]
        public bool Equals(string a, string b) =>
            a == b;

        /// <summary>
        /// Get hash code of the value
        /// </summary>
        /// <param name="x">Value to get the hash code of</param>
        /// <returns>The hash code of x</returns>
        [Pure]
        public int GetHashCode(string x) =>
            default(HashableString).GetHashCode(x);
            
        [Pure]
        public Task<bool> EqualsAsync(string x, string y) =>
            Equals(x, y).AsTask();

        [Pure]
        public Task<int> GetHashCodeAsync(string x) =>
            GetHashCode(x).AsTask();
    }

    /// <summary>
    /// String equality (ordinal, ignore case)
    /// </summary>
    public struct EqStringOrdinalIgnoreCase : Eq<string>
    {
        public static readonly EqStringOrdinalIgnoreCase Inst = default(EqStringOrdinalIgnoreCase);

        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="a">The left hand side of the equality operation</param>
        /// <param name="b">The right hand side of the equality operation</param>
        /// <returns>True if a and b are equal</returns>
        [Pure]
        public bool Equals(string a, string b) =>
            StringComparer.OrdinalIgnoreCase.Equals(a, b);

        /// <summary>
        /// Get hash code of the value
        /// </summary>
        /// <param name="x">Value to get the hash code of</param>
        /// <returns>The hash code of x</returns>
        [Pure]
        public int GetHashCode(string x) =>
            default(HashableStringOrdinalIgnoreCase).GetHashCode(x);
            
        [Pure]
        public Task<bool> EqualsAsync(string x, string y) =>
            Equals(x, y).AsTask();

        [Pure]
        public Task<int> GetHashCodeAsync(string x) =>
            GetHashCode(x).AsTask();
    }

    /// <summary>
    /// String equality (ordinal)
    /// </summary>
    public struct EqStringOrdinal : Eq<string>
    {
        public static readonly EqStringOrdinal Inst = default(EqStringOrdinal);

        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="a">The left hand side of the equality operation</param>
        /// <param name="b">The right hand side of the equality operation</param>
        /// <returns>True if a and b are equal</returns>
        [Pure]
        public bool Equals(string a, string b) =>
            StringComparer.Ordinal.Equals(a, b);

        /// <summary>
        /// Get hash code of the value
        /// </summary>
        /// <param name="x">Value to get the hash code of</param>
        /// <returns>The hash code of x</returns>
        [Pure]
        public int GetHashCode(string x) =>
            default(HashableStringOrdinal).GetHashCode(x);
            
        [Pure]
        public Task<bool> EqualsAsync(string x, string y) =>
            Equals(x, y).AsTask();

        [Pure]
        public Task<int> GetHashCodeAsync(string x) =>
            GetHashCode(x).AsTask();
    }

    /// <summary>
    /// String equality (current culture, ignore case)
    /// </summary>
    public struct EqStringCurrentCultureIgnoreCase : Eq<string>
    {
        public static readonly EqStringCurrentCultureIgnoreCase Inst = default(EqStringCurrentCultureIgnoreCase);

        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="a">The left hand side of the equality operation</param>
        /// <param name="b">The right hand side of the equality operation</param>
        /// <returns>True if a and b are equal</returns>
        [Pure]
        public bool Equals(string a, string b) =>
            StringComparer.CurrentCultureIgnoreCase.Equals(a, b);

        /// <summary>
        /// Get hash code of the value
        /// </summary>
        /// <param name="x">Value to get the hash code of</param>
        /// <returns>The hash code of x</returns>
        [Pure]
        public int GetHashCode(string x) =>
            default(HashableStringCurrentCultureIgnoreCase).GetHashCode(x);
            
        [Pure]
        public Task<bool> EqualsAsync(string x, string y) =>
            Equals(x, y).AsTask();

        [Pure]
        public Task<int> GetHashCodeAsync(string x) =>
            GetHashCode(x).AsTask();
    }

    /// <summary>
    /// String equality (current culture)
    /// </summary>
    public struct EqStringCurrentCulture : Eq<string>
    {
        public static readonly EqStringCurrentCulture Inst = default(EqStringCurrentCulture);

        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="a">The left hand side of the equality operation</param>
        /// <param name="b">The right hand side of the equality operation</param>
        /// <returns>True if a and b are equal</returns>
        [Pure]
        public bool Equals(string a, string b) =>
            StringComparer.CurrentCulture.Equals(a, b);

        /// <summary>
        /// Get hash code of the value
        /// </summary>
        /// <param name="x">Value to get the hash code of</param>
        /// <returns>The hash code of x</returns>
        [Pure]
        public int GetHashCode(string x) =>
            default(HashableStringCurrentCulture).GetHashCode(x);
                
        [Pure]
        public Task<bool> EqualsAsync(string x, string y) =>
            Equals(x, y).AsTask();

        [Pure]
        public Task<int> GetHashCodeAsync(string x) =>
            GetHashCode(x).AsTask();
    }
}
