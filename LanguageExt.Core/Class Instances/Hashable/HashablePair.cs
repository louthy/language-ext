using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace LanguageExt;

public struct HashablePair<HashA, HashB, A, B> : Hashable<(A, B)>
    where HashA : Hashable<A>
    where HashB : Hashable<B>
{
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetHashCode((A, B) pair) =>
        (HashB.GetHashCode(pair.Item2) ^ 
         (HashA.GetHashCode(pair.Item1) ^ FNV32.OffsetBasis) * FNV32.Prime) * FNV32.Prime;
}
