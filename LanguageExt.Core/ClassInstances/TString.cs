using LanguageExt.TypeClasses;
using System;

namespace LanguageExt.ClassInstances
{
    public struct TString : Ord<string>, Monoid<string>
    {
        public static readonly TString Inst = default(TString);

        /// <summary>
        /// Append
        /// </summary>
        public string Append(string x, string y) =>
            x + y;

        /// <summary>
        /// Empty
        /// </summary>
        /// <returns></returns>
        public string Empty() => 
            "";

        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
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
        public int Compare(string x, string y) =>
            default(OrdDefault<string>).Compare(x,y);

        /// <summary>
        /// Get the hash-code of the provided value
        /// </summary>
        /// <returns>Hash code of x</returns>
        public int GetHashCode(string x) =>
            x.IsNull() ? 0 : x.GetHashCode();
    }

    public struct TStringOrdinalIgnoreCase : Ord<string>, Monoid<string>
    {
        public static readonly TStringOrdinalIgnoreCase Inst = default(TStringOrdinalIgnoreCase);

        /// <summary>
        /// Append
        /// </summary>
        public string Append(string x, string y) =>
            x + y;

        /// <summary>
        /// Empty
        /// </summary>
        /// <returns></returns>
        public string Empty() =>
            "";

        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
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
        public int Compare(string x, string y) =>
            default(OrdDefault<string>).Compare(x, y);

        /// <summary>
        /// Get the hash-code of the provided value
        /// </summary>
        /// <returns>Hash code of x</returns>
        public int GetHashCode(string x) => 
            x.IsNull() ? 0 : x.GetHashCode();
    }
}
