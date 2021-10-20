using System.Diagnostics.Contracts;
using LanguageExt.TypeClasses;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using LanguageExt.Common;

namespace LanguageExt.ClassInstances
{
    public struct HashableResult<A> : Hashable<Result<A>>
    {
        /// <summary>
        /// Get hash code of the value
        /// </summary>
        /// <param name="x">Value to get the hash code of</param>
        /// <returns>The hash code of x</returns>
        public int GetHashCode(Result<A> x) =>
            x.IsBottom ? -2
          : x.IsFaulted ? -1
          : x.Value?.GetHashCode() ?? 0;
         
        /// <summary>
        /// Get hash code of the value
        /// </summary>
        /// <param name="x">Value to get the hash code of</param>
        /// <returns>The hash code of x</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<int> GetHashCodeAsync(Result<A> x) =>
            GetHashCode(x).AsTask();    
    }

    public struct HashableOptionalResult<A> : Hashable<OptionalResult<A>>
    {
        /// <summary>
        /// Get hash code of the value
        /// </summary>
        /// <param name="x">Value to get the hash code of</param>
        /// <returns>The hash code of x</returns>
        public int GetHashCode(OptionalResult<A> x) =>
            x.IsBottom ? -2
          : x.IsFaulted ? -1
          : default(HashableOption<A>).GetHashCode(x.Value);
         
        /// <summary>
        /// Get hash code of the value
        /// </summary>
        /// <param name="x">Value to get the hash code of</param>
        /// <returns>The hash code of x</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<int> GetHashCodeAsync(OptionalResult<A> x) =>
            GetHashCode(x).AsTask();    
    }
}
