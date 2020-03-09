using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using LanguageExt.TypeClasses;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Ordering class instance for all record types
    /// </summary>
    /// <typeparam name="A">Record type</typeparam>
    public struct OrdRecord<A> : Ord<A> where A : Record<A>
    {
        public int Compare(A x, A y) =>
            RecordType<A>.Compare(x, y);

        public bool Equals(A x, A y) =>
            RecordType<A>.EqualityTyped(x, y);

        public int GetHashCode(A x) =>
            RecordType<A>.Hash(x);

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
