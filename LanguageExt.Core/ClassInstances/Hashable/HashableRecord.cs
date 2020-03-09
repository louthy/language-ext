using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using LanguageExt.TypeClasses;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Hash for all record types
    /// </summary>
    /// <typeparam name="A">Record type</typeparam>
    public struct HashableRecord<A> : Hashable<A> where A : Record<A>
    {
        /// <summary>
        /// Get hash code of the value
        /// </summary>
        /// <param name="x">Value to get the hash code of</param>
        /// <returns>The hash code of x</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetHashCode(A x) =>
            RecordType<A>.Hash(x);
         
        /// <summary>
        /// Get hash code of the value
        /// </summary>
        /// <param name="x">Value to get the hash code of</param>
        /// <returns>The hash code of x</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<int> GetHashCodeAsync(A x) =>
            GetHashCode(x).AsTask();
    }
}
