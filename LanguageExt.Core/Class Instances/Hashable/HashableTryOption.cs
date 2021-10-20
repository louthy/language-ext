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
    /// Hash of any type in the TryOption type-class
    /// </summary>
    public struct HashableTryOption<HashA, A> : Hashable<TryOption<A>>
        where HashA : struct, Hashable<A>
    {
        /// <summary>
        /// Get hash code of the value
        /// </summary>
        /// <param name="x">Value to get the hash code of</param>
        /// <returns>The hash code of x</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetHashCode(TryOption<A> x)
        {
            var res = x.Try();
            return res.IsFaulted || res.Value.IsNone ? 0 : default(HashA).GetHashCode(res.Value.Value);
        }
         
        /// <summary>
        /// Get hash code of the value
        /// </summary>
        /// <param name="x">Value to get the hash code of</param>
        /// <returns>The hash code of x</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<int> GetHashCodeAsync(TryOption<A> x) =>
            GetHashCode(x).AsTask();        
    }

    /// <summary>
    /// Hash of any type in the TryOption type-class
    /// </summary>
    public struct HashableTryOption<A> : Hashable<TryOption<A>>
    {
        
        /// <summary>
        /// Get hash code of the value
        /// </summary>
        /// <param name="x">Value to get the hash code of</param>
        /// <returns>The hash code of x</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetHashCode(TryOption<A> x) =>
            default(HashableTryOption<HashableDefault<A>, A>).GetHashCode(x);
        
        /// <summary>
        /// Get hash code of the value
        /// </summary>
        /// <param name="x">Value to get the hash code of</param>
        /// <returns>The hash code of x</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<int> GetHashCodeAsync(TryOption<A> x) =>
            GetHashCode(x).AsTask();        
    }
}
