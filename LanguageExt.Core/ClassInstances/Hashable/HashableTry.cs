using System;
using LanguageExt;
using LanguageExt.TypeClasses;
using static LanguageExt.TypeClass;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Hash of any values bound by the Try monad
    /// </summary>
    public struct HashableTry<HashA, A> : Hashable<Try<A>>
        where HashA : struct, Hashable<A>
    {
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetHashCode(Try<A> x)
        {
            var res = x.Try();
            return res.IsFaulted 
                ? 0 
                : default(HashA).GetHashCode(res.Value);
        }
         
        /// <summary>
        /// Get hash code of the value
        /// </summary>
        /// <param name="x">Value to get the hash code of</param>
        /// <returns>The hash code of x</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<int> GetHashCodeAsync(Try<A> x) =>
            GetHashCode(x).AsTask();        
    }

    /// <summary>
    /// Hash of any values bound by the Try monad
    /// </summary>
    public struct HashableTry<A> : Hashable<Try<A>>
    {
        [Pure]
        public int GetHashCode(Try<A> x) =>
            default(HashableTry<HashableDefault<A>, A>).GetHashCode(x);
         
        /// <summary>
        /// Get hash code of the value
        /// </summary>
        /// <param name="x">Value to get the hash code of</param>
        /// <returns>The hash code of x</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<int> GetHashCodeAsync(Try<A> x) =>
            GetHashCode(x).AsTask();        
    }
}
