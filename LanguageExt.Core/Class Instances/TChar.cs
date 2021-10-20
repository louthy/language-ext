using System;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Integer number 
    /// </summary>
    public struct TChar : Eq<char>, Ord<char>, Monoid<char>, Arithmetic<char>
    {
        public static readonly TChar Inst = default(TChar);

        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        [Pure]
        public bool Equals(char x, char y) =>
            x == y;

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
        public int Compare(char x, char y) =>
            x.CompareTo(y);

        /// <summary>
        /// Monoid empty value (0)
        /// </summary>
        /// <returns>0</returns>
        [Pure]
        public char Empty() => (char)0;

        /// <summary>
        /// Semigroup append (sum)
        /// </summary>
        /// <param name="x">left hand side of the append operation</param>
        /// <param name="y">right hand side of the append operation</param>
        /// <returns>x + y</returns>
        [Pure]
        public char Append(char x, char y) => 
            (char)(x + y);

        /// <summary>
        /// Get the hash-code of the provided value
        /// </summary>
        /// <returns>Hash code of x</returns>
        [Pure]
        public int GetHashCode(char x) =>
            x.GetHashCode();

        [Pure]
        public char Plus(char x, char y) =>
            (char)(x + y);

        [Pure]
        public char Subtract(char x, char y) =>
            (char)(x - y);

        [Pure]
        public char Product(char x, char y) =>
            (char) (x * y);

        [Pure]
        public char Negate(char x) =>
            (char)(-x);
        
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<bool> EqualsAsync(char x, char y) =>
            Equals(x, y).AsTask();

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<int> GetHashCodeAsync(char x) =>
            GetHashCode(x).AsTask();         

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<int> CompareAsync(char x, char y) =>
            Compare(x, y).AsTask();
    }
}
