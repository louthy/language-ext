using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using LanguageExt.Common;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Seq hashing
    /// </summary>
    public struct HashableSeq<HashA, A> : Hashable<Seq<A>>
        where HashA : struct, Hashable<A>
    {
        /// <summary>
        /// Get hash code of the value
        /// </summary>
        /// <param name="x">Value to get the hash code of</param>
        /// <returns>The hash code of x</returns>
        [Pure]
        public int GetHashCode(Seq<A> x) =>
            hash<HashA, A>(x);
         
        /// <summary>
        /// Get hash code of the value
        /// </summary>
        /// <param name="x">Value to get the hash code of</param>
        /// <returns>The hash code of x</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<int> GetHashCodeAsync(Seq<A> x) =>
            GetHashCode(x).AsTask();    
    }

    /// <summary>
    /// Seq hashing
    /// </summary>
    public struct HashableSeq<A> : Hashable<Seq<A>>
    {
        /// <summary>
        /// Get hash code of the value
        /// </summary>
        /// <param name="x">Value to get the hash code of</param>
        /// <returns>The hash code of x</returns>
        [Pure]
        public int GetHashCode(Seq<A> x) =>
            default(HashableSeq<HashableDefault<A>, A>).GetHashCode(x);
         
        /// <summary>
        /// Get hash code of the value
        /// </summary>
        /// <param name="x">Value to get the hash code of</param>
        /// <returns>The hash code of x</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<int> GetHashCodeAsync(Seq<A> x) =>
            GetHashCode(x).AsTask();    
    }
}
