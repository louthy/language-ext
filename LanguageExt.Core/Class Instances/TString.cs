using LanguageExt.TypeClasses;
using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace LanguageExt.ClassInstances
{
    public struct TString : Ord<string>, Monoid<string>
    {
        public static readonly TString Inst = default(TString);

        /// <summary>
        /// Append
        /// </summary>
        [Pure]
        public string Append(string x, string y) =>
            x + y;

        /// <summary>
        /// Empty
        /// </summary>
        /// <returns></returns>
        [Pure]
        public string Empty() => 
            "";

        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        [Pure]
        public bool Equals(string x, string y) =>
            default(EqString).Equals(x, y);

        /// <summary>
        /// Compare two values
        /// </summary>
        /// <param name="x">Left hand side of the compare operation</param>
        /// <param name="y">Right hand side of the compare operation</param>
        /// <returns>
        /// if x greater than y : 1
        /// 
        /// if x less than y    : -1
        /// 
        /// if x equals y       : 0
        /// </returns>
        [Pure]
        public int Compare(string x, string y) =>
            default(OrdString).Compare(x,y);

        /// <summary>
        /// Get the hash-code of the provided value
        /// </summary>
        /// <returns>Hash code of x</returns>
        [Pure]
        public int GetHashCode(string x) =>
            default(OrdString).GetHashCode(x);
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

    public struct TStringOrdinalIgnoreCase : Ord<string>, Monoid<string>
    {
        public static readonly TStringOrdinalIgnoreCase Inst = default(TStringOrdinalIgnoreCase);

        /// <summary>
        /// Append
        /// </summary>
        [Pure]
        public string Append(string x, string y) =>
            x + y;

        /// <summary>
        /// Empty
        /// </summary>
        /// <returns></returns>
        [Pure]
        public string Empty() =>
            "";

        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        [Pure]
        public bool Equals(string x, string y) =>
            default(EqStringOrdinalIgnoreCase).Equals(x, y);

        /// <summary>
        /// Compare two values
        /// </summary>
        /// <param name="x">Left hand side of the compare operation</param>
        /// <param name="y">Right hand side of the compare operation</param>
        /// <returns>
        /// if x greater than y : 1
        /// 
        /// if x less than y    : -1
        /// 
        /// if x equals y       : 0
        /// </returns>
        [Pure]
        public int Compare(string x, string y) =>
            default(OrdStringOrdinalIgnoreCase).Compare(x, y);

        /// <summary>
        /// Get the hash-code of the provided value
        /// </summary>
        /// <returns>Hash code of x</returns>
        [Pure]
        public int GetHashCode(string x) => 
            x.IsNull() ? 0 : x.GetHashCode();
        
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

    public struct TStringOrdinal : Ord<string>, Monoid<string>
    {
        public static readonly TStringOrdinal Inst = default(TStringOrdinal);

        /// <summary>
        /// Append
        /// </summary>
        [Pure]
        public string Append(string x, string y) =>
            x + y;

        /// <summary>
        /// Empty
        /// </summary>
        /// <returns></returns>
        [Pure]
        public string Empty() =>
            "";

        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        [Pure]
        public bool Equals(string x, string y) =>
            default(EqStringOrdinal).Equals(x, y);

        /// <summary>
        /// Compare two values
        /// </summary>
        /// <param name="x">Left hand side of the compare operation</param>
        /// <param name="y">Right hand side of the compare operation</param>
        /// <returns>
        /// if x greater than y : 1
        /// 
        /// if x less than y    : -1
        /// 
        /// if x equals y       : 0
        /// </returns>
        [Pure]
        public int Compare(string x, string y) =>
            default(OrdStringOrdinal).Compare(x, y);

        /// <summary>
        /// Get the hash-code of the provided value
        /// </summary>
        /// <returns>Hash code of x</returns>
        [Pure]
        public int GetHashCode(string x) =>
            x?.GetHashCode() ?? 0;
        
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

    public struct TStringCurrentCultureIgnoreCase : Ord<string>, Monoid<string>
    {
        public static readonly TStringCurrentCultureIgnoreCase Inst = default(TStringCurrentCultureIgnoreCase);

        /// <summary>
        /// Append
        /// </summary>
        [Pure]
        public string Append(string x, string y) =>
            x + y;

        /// <summary>
        /// Empty
        /// </summary>
        /// <returns></returns>
        [Pure]
        public string Empty() =>
            "";

        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        [Pure]
        public bool Equals(string x, string y) =>
            default(EqStringCurrentCultureIgnoreCase).Equals(x, y);

        /// <summary>
        /// Compare two values
        /// </summary>
        /// <param name="x">Left hand side of the compare operation</param>
        /// <param name="y">Right hand side of the compare operation</param>
        /// <returns>
        /// if x greater than y : 1
        /// 
        /// if x less than y    : -1
        /// 
        /// if x equals y       : 0
        /// </returns>
        [Pure]
        public int Compare(string x, string y) =>
            default(OrdStringCurrentCultureIgnoreCase).Compare(x, y);

        /// <summary>
        /// Get the hash-code of the provided value
        /// </summary>
        /// <returns>Hash code of x</returns>
        [Pure]
        public int GetHashCode(string x) =>
            x.IsNull() ? 0 : x.GetHashCode();
        
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

    public struct TStringCurrentCulture : Ord<string>, Monoid<string>
    {
        public static readonly TStringCurrentCulture Inst = default(TStringCurrentCulture);

        /// <summary>
        /// Append
        /// </summary>
        [Pure]
        public string Append(string x, string y) =>
            x + y;

        /// <summary>
        /// Empty
        /// </summary>
        /// <returns></returns>
        [Pure]
        public string Empty() =>
            "";

        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        [Pure]
        public bool Equals(string x, string y) =>
            default(EqStringCurrentCulture).Equals(x, y);

        /// <summary>
        /// Compare two values
        /// </summary>
        /// <param name="x">Left hand side of the compare operation</param>
        /// <param name="y">Right hand side of the compare operation</param>
        /// <returns>
        /// if x greater than y : 1
        /// 
        /// if x less than y    : -1
        /// 
        /// if x equals y       : 0
        /// </returns>
        [Pure]
        public int Compare(string x, string y) =>
            default(OrdStringCurrentCulture).Compare(x, y);

        /// <summary>
        /// Get the hash-code of the provided value
        /// </summary>
        /// <returns>Hash code of x</returns>
        [Pure]
        public int GetHashCode(string x) =>
            x.IsNull() ? 0 : x.GetHashCode();
        
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
