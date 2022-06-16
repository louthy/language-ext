using System;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using LanguageExt.TypeClasses;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// String comparison
    /// </summary>
    public struct OrdString : Ord<string>
    {
        public static readonly OrdString Inst = default(OrdString);

        /// <summary>
        /// Compare two values
        /// </summary>
        /// <param name="x">Left hand side of the compare operation</param>
        /// <param name="y">Right hand side of the compare operation</param>
        /// <returns>
        /// if a greater than b : 1
        /// if a less than b    : -1
        /// if a equals b       : 0
        /// </returns>
        public int Compare(string a, string b) =>
            a?.CompareTo(b) ?? 1;

        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="a">The left hand side of the equality operation</param>
        /// <param name="b">The right hand side of the equality operation</param>
        /// <returns>True if a and b are equal</returns>
        public bool Equals(string a, string b) =>
            default(EqString).Equals(a, b);

        /// <summary>
        /// Get the hash-code of the provided value
        /// </summary>
        /// <returns>Hash code of x</returns>
        public int GetHashCode(string x) =>
            default(HashableString).GetHashCode(x);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<bool> EqualsAsync(string x, string y) =>
            Equals(x, y).AsTask();

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<int> GetHashCodeAsync(string x) =>
            GetHashCode(x).AsTask();       
        
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<int> CompareAsync(string x, string y) =>
            Compare(x, y).AsTask();    
    }

    /// <summary>
    /// String comparison (ordinal, ignore case)
    /// </summary>
    public struct OrdStringOrdinalIgnoreCase : Ord<string>
    {
        public static readonly OrdStringOrdinalIgnoreCase Inst = default(OrdStringOrdinalIgnoreCase);

        /// <summary>
        /// Compare two values
        /// </summary>
        /// <param name="x">Left hand side of the compare operation</param>
        /// <param name="y">Right hand side of the compare operation</param>
        /// <returns>
        /// if a greater than b : 1
        /// if a less than b    : -1
        /// if a equals b       : 0
        /// </returns>
        public int Compare(string a, string b) =>
            StringComparer.OrdinalIgnoreCase.Compare(a, b);

        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="a">The left hand side of the equality operation</param>
        /// <param name="b">The right hand side of the equality operation</param>
        /// <returns>True if a and b are equal</returns>
        public bool Equals(string a, string b) =>
            default(EqStringOrdinalIgnoreCase).Equals(a, b);

        /// <summary>
        /// Get the hash-code of the provided value
        /// </summary>
        /// <returns>Hash code of x</returns>
        public int GetHashCode(string x) =>
            default(HashableStringOrdinalIgnoreCase).GetHashCode(x);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<bool> EqualsAsync(string x, string y) =>
            Equals(x, y).AsTask();

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<int> GetHashCodeAsync(string x) =>
            GetHashCode(x).AsTask();       
        
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<int> CompareAsync(string x, string y) =>
            Compare(x, y).AsTask();    
    }

    /// <summary>
    /// String comparison (ordinal)
    /// </summary>
    public struct OrdStringOrdinal : Ord<string>
    {
        public static readonly OrdStringOrdinal Inst = default(OrdStringOrdinal);

        /// <summary>
        /// Compare two values
        /// </summary>
        /// <param name="x">Left hand side of the compare operation</param>
        /// <param name="y">Right hand side of the compare operation</param>
        /// <returns>
        /// if a greater than b : 1
        /// if a less than b    : -1
        /// if a equals b       : 0
        /// </returns>
        public int Compare(string a, string b) =>
            StringComparer.Ordinal.Compare(a, b);

        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="a">The left hand side of the equality operation</param>
        /// <param name="b">The right hand side of the equality operation</param>
        /// <returns>True if a and b are equal</returns>
        public bool Equals(string a, string b) =>
            default(EqStringOrdinal).Equals(a, b);

        /// <summary>
        /// Get the hash-code of the provided value
        /// </summary>
        /// <returns>Hash code of x</returns>
        public int GetHashCode(string x) =>
            default(HashableStringOrdinal).GetHashCode(x);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<bool> EqualsAsync(string x, string y) =>
            Equals(x, y).AsTask();

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<int> GetHashCodeAsync(string x) =>
            GetHashCode(x).AsTask();       
        
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<int> CompareAsync(string x, string y) =>
            Compare(x, y).AsTask();    
    }

    /// <summary>
    /// String comparison (current culture, ignore case)
    /// </summary>
    public struct OrdStringCurrentCultureIgnoreCase : Ord<string>
    {
        public static readonly OrdStringCurrentCultureIgnoreCase Inst = default(OrdStringCurrentCultureIgnoreCase);

        /// <summary>
        /// Compare two values
        /// </summary>
        /// <param name="x">Left hand side of the compare operation</param>
        /// <param name="y">Right hand side of the compare operation</param>
        /// <returns>
        /// if a greater than b : 1
        /// if a less than b    : -1
        /// if a equals b       : 0
        /// </returns>
        public int Compare(string a, string b) =>
            StringComparer.CurrentCultureIgnoreCase.Compare(a, b);

        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="a">The left hand side of the equality operation</param>
        /// <param name="b">The right hand side of the equality operation</param>
        /// <returns>True if a and b are equal</returns>
        public bool Equals(string a, string b) =>
            default(EqStringCurrentCultureIgnoreCase).Equals(a, b);

        /// <summary>
        /// Get the hash-code of the provided value
        /// </summary>
        /// <returns>Hash code of x</returns>
        public int GetHashCode(string x) =>
            default(HashableStringCurrentCultureIgnoreCase).GetHashCode(x);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<bool> EqualsAsync(string x, string y) =>
            Equals(x, y).AsTask();

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<int> GetHashCodeAsync(string x) =>
            GetHashCode(x).AsTask();       
        
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<int> CompareAsync(string x, string y) =>
            Compare(x, y).AsTask();    
    }

    /// <summary>
    /// String comparison (current culture)
    /// </summary>
    public struct OrdStringCurrentCulture : Ord<string>
    {
        public static readonly OrdStringCurrentCulture Inst = default(OrdStringCurrentCulture);

        /// <summary>
        /// Compare two values
        /// </summary>
        /// <param name="x">Left hand side of the compare operation</param>
        /// <param name="y">Right hand side of the compare operation</param>
        /// <returns>
        /// if a greater than b : 1
        /// if a less than b    : -1
        /// if a equals b       : 0
        /// </returns>
        public int Compare(string a, string b) =>
            StringComparer.CurrentCulture.Compare(a, b);

        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="a">The left hand side of the equality operation</param>
        /// <param name="b">The right hand side of the equality operation</param>
        /// <returns>True if a and b are equal</returns>
        public bool Equals(string a, string b) =>
            default(EqStringCurrentCulture).Equals(a, b);

        /// <summary>
        /// Get the hash-code of the provided value
        /// </summary>
        /// <returns>Hash code of x</returns>
        public int GetHashCode(string x) =>
            default(HashableStringCurrentCulture).GetHashCode(x);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<bool> EqualsAsync(string x, string y) =>
            Equals(x, y).AsTask();

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<int> GetHashCodeAsync(string x) =>
            GetHashCode(x).AsTask();       
        
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<int> CompareAsync(string x, string y) =>
            Compare(x, y).AsTask();    
    }
}
