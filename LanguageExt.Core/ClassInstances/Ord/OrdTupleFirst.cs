using LanguageExt.TypeClasses;
using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Ord instance for a pair tuple.  It orders using the first
    /// item in the tuple only and the provided OrdA.
    /// </summary>
    public struct OrdTupleFirst<OrdA, A, B> : Ord<ValueTuple<A, B>> where OrdA : struct, Ord<A>
    {
        public int Compare((A, B) x, (A, B) y) =>
            default(OrdA).Compare(x.Item1, y.Item1);

        public bool Equals((A, B) x, (A, B) y) =>
            default(OrdA).Equals(x.Item1, y.Item1);

        public int GetHashCode((A, B) x) =>
            x.GetHashCode();
        
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<bool> EqualsAsync((A, B) x, (A, B) y) =>
            Equals(x, y).AsTask();

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<int> GetHashCodeAsync((A, B) x) =>
            GetHashCode(x).AsTask();         
         
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<int> CompareAsync((A, B) x, (A, B) y) =>
            Compare(x, y).AsTask();     
    }
}
