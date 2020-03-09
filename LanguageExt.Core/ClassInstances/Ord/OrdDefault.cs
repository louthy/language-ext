using LanguageExt.TypeClasses;
using System.Collections.Generic;
using static LanguageExt.Prelude;
using System;
using System.Diagnostics.Contracts;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Uses the standard .NET  Comparer<A>.Default.Compare(a,b) method to
    /// provide equality testing.
    /// </summary>
    public struct OrdDefault<A> : Ord<A>
    {
        public static readonly OrdDefault<A> Inst = default(OrdDefault<A>);

        static readonly Func<A, A, int> ord;

        static OrdDefault() =>
            ord = OrdClass<A>.Compare;

        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        [Pure]
        public int Compare(A x, A y) =>
            ord(x, y);

        [Pure]
        public bool Equals(A x, A y) =>
            default(EqDefault<A>).Equals(x, y);

        /// <summary>
        /// Get the hash-code of the provided value
        /// </summary>
        /// <returns>Hash code of x</returns>
        [Pure]
        public int GetHashCode(A x) =>
            default(EqDefault<A>).GetHashCode(x);
   
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<bool> EqualsAsync(A x, A y) =>
            Equals(x, y).AsTask();

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<int> GetHashCodeAsync(A x) =>
            GetHashCode(x).AsTask();
           
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<int> CompareAsync(A x, A y) =>
            Compare(x, y).AsTask();   
    }
}
