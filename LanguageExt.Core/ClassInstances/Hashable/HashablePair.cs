using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

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
    }
}
