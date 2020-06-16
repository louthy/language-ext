using LanguageExt;
using LanguageExt.TypeClasses;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Bool class instance.  Implements
    /// 
    ///     Eq<bool>
    ///     Ord<bool>
    ///     Bool<bool>
    /// </summary>
    public struct TBool : Eq<bool>, Ord<bool>, Bool<bool>
    {
        public static readonly TBool Inst = new TBool();

        /// <summary>
        /// Returns the result of the logical AND operation between `a` and `b`
        /// </summary>
        /// <returns>The result of the logical AND operation between `a` and `b`</returns>
        [Pure]
        public bool And(bool a, bool b) =>
            a && b;

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
        public int Compare(bool x, bool y) =>
            x.CompareTo(y);

        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        [Pure]
        public bool Equals(bool x, bool y) =>
            x == y;

        /// <summary>
        /// Returns False
        /// </summary>
        /// <returns>False</returns>
        [Pure]
        public bool False() =>
            false;

        /// <summary>
        /// Get the hash-code of the provided value
        /// </summary>
        /// <returns>Hash code of x</returns>
        [Pure]
        public int GetHashCode(bool x) =>
            x.GetHashCode();

        /// <summary>
        /// Returns the result of the logical NOT operation on `a`
        /// </summary>
        /// <returns>The result of the logical NOT operation on `a`</returns>
        [Pure]
        public bool Not(bool a) =>
            !a;

        /// <summary>
        /// Returns the result of the logical OR operation between `a` and `b`
        /// </summary>
        /// <returns>The result of the logical OR operation between `a` and `b`</returns>
        [Pure]
        public bool Or(bool a, bool b) =>
            a || b;

        /// <summary>
        /// Returns True
        /// </summary>
        /// <returns>True</returns>
        [Pure]
        public bool True() =>
            true;

        /// <summary>
        /// Returns the result of the logical exclusive-OR operation between `a` and `b`
        /// </summary>
        /// <returns>The result of the logical exclusive-OR operation between `a` and `b`</returns>
        [Pure]
        public bool XOr(bool a, bool b) =>
            a ^ b;

        /// <summary>
        /// Logical implication
        /// </summary>
        /// <returns>If `a` is true that implies `b`, else `true`</returns>
        [Pure]
        public bool Implies(bool a, bool b) =>
            a ? b : true;

        /// <summary>
        /// Logical bi-conditional.  Both `a` and `b` must be `true`, or both `a` and `b` must
        /// be false.
        /// </summary>
        /// <returns>`true` if `a == b`, `false` otherwise</returns>
        [Pure]
        public bool BiCondition(bool a, bool b) =>
            a == b;
        
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<bool> EqualsAsync(bool x, bool y) =>
            Equals(x, y).AsTask();

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<int> GetHashCodeAsync(bool x) =>
            GetHashCode(x).AsTask();         

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<int> CompareAsync(bool x, bool y) =>
            Compare(x, y).AsTask();
    }
}
