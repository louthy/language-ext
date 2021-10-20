using LanguageExt.TypeClasses;
using System.Collections.Generic;
using static LanguageExt.Prelude;
using System;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Finds an appropriate Hashable from the loaded assemblies, if one can't be found then it
    /// falls back to the standard .NET Object.GetHashCode() method to provide a hash-code.
    /// </summary>
    public struct HashableDefault<A> : Hashable<A>
    {
        /// <summary>
        /// Get hash code of the value
        /// </summary>
        /// <param name="x">Value to get the hash code of</param>
        /// <returns>The hash code of x</returns>
        [Pure]
        public int GetHashCode(A x) =>
            HashableClass<A>.GetHashCode(x);

        [Pure]
        public Task<int> GetHashCodeAsync(A x) =>
            default(HashableDefaultAsync<A>).GetHashCodeAsync(x);
    }
}
