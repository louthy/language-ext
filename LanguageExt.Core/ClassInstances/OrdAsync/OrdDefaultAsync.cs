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
    public struct OrdDefaultAsync<A> : OrdAsync<A>
    {
        public static readonly OrdDefaultAsync<A> Inst = default(OrdDefaultAsync<A>);

        static readonly Func<A, A, Task<int>> ord;

        static OrdDefaultAsync() =>
            ord = OrdAsyncClass<A>.CompareAsync;

        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        [Pure]
        public Task<int> CompareAsync(A x, A y) =>
            ord(x, y);

        [Pure]
        public Task<bool> EqualsAsync(A x, A y) =>
            default(EqDefaultAsync<A>).EqualsAsync(x, y);

        /// <summary>
        /// Get the hash-code of the provided value
        /// </summary>
        /// <returns>Hash code of x</returns>
        [Pure]
        public Task<int> GetHashCodeAsync(A x) =>
            default(HashableDefaultAsync<A>).GetHashCodeAsync(x);
    }
}
