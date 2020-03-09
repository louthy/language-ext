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
            x.Id == y.Id;

        public int GetHashCode(Task<A> x) =>
            x.Id.GetHashCode();

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<bool> EqualsAsync(Task<A> x, Task<A> y) =>
            from a in x
            from b in y
            from r in default(EqDefaultAsync<A>).EqualsAsync(a, b)
            select r;

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<int> GetHashCodeAsync(Task<A> x) =>
            from a in x
            from r in default(HashableDefaultAsync<A>).GetHashCodeAsync(a)
            select r;
    }
}
