using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using LanguageExt.TypeClasses;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Always returns true for equality checks and 0 for ordering
    /// </summary>
    public struct OrdTrue<A> : Ord<A>
    {
        public int Compare(A x, A y) =>
            0;

        public bool Equals(A x, A y) =>
            true;

        public int GetHashCode(A x) =>
            default(OrdDefault<A>).GetHashCode(x);
        
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<bool> EqualsAsync(A x, A y) =>
            Equals(x, y).AsTask();

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<int> GetHashCodeAsync(A x) =>
            GetHashCode(x).AsTask();         
          
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<int> CompareAsync(A x, A y) =>
            Compare(x, y).AsTask();     
    }
}
