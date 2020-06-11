using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using LanguageExt.TypeClasses;
using System.Threading.Tasks;

namespace LanguageExt.ClassInstances
{
    public struct OrdTask<A> : Ord<Task<A>>
    {
        public int Compare(Task<A> x, Task<A> y) =>
            x.Id.CompareTo(y.Id);

        public bool Equals(Task<A> x, Task<A> y) =>
            default(EqTask<A>).Equals(x, y);

        public int GetHashCode(Task<A> x) =>
            default(HashableTask<A>).GetHashCode(x);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<bool> EqualsAsync(Task<A> x, Task<A> y) =>
            default(EqTask<A>).EqualsAsync(x, y);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<int> GetHashCodeAsync(Task<A> x) =>
            default(HashableTask<A>).GetHashCodeAsync(x);
        
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<int> CompareAsync(Task<A> x, Task<A> y) =>
            Compare(x, y).AsTask();    
    }
}
