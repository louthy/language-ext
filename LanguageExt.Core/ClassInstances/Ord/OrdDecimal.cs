using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Equality and ordering
    /// </summary>
    public struct OrdDecimal : Ord<decimal>
    {
        public static readonly OrdDecimal Inst = default(OrdDecimal);

        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        [Pure]
        public bool Equals(decimal x, decimal y) =>
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
        public int Compare(decimal x, decimal y) =>
            x.CompareTo(y);

        /// <summary>
        /// Get the hash-code of the provided value
        /// </summary>
        /// <returns>Hash code of x</returns>
        [Pure]
        public int GetHashCode(decimal x) =>
            x.GetHashCode();
   
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<bool> EqualsAsync(decimal x, decimal y) =>
            Equals(x, y).AsTask();

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<int> GetHashCodeAsync(decimal x) =>
            GetHashCode(x).AsTask();        
         
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<int> CompareAsync(decimal x, decimal y) =>
            Compare(x, y).AsTask();     
    }
}
