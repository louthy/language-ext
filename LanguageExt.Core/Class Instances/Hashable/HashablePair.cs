using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace LanguageExt
{
    public struct HashablePair<HashA, HashB, A, B> : Hashable<(A, B)>
        where HashA : struct, Hashable<A>
        where HashB : struct, Hashable<B>
    {
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetHashCode((A, B) pair) =>
            (default(HashB).GetHashCode(pair.Item2) ^ 
                ((default(HashA).GetHashCode(pair.Item1) ^ FNV32.OffsetBasis) * FNV32.Prime)) * FNV32.Prime;

        /// <summary>
        /// Get hash code of the value
        /// </summary>
        /// <param name="x">Value to get the hash code of</param>
        /// <returns>The hash code of x</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<int> GetHashCodeAsync((A, B) x) =>
            GetHashCode(x).AsTask();
    }
}
