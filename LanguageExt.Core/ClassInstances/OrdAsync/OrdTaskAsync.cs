using LanguageExt.TypeClasses;
using System.Threading.Tasks;

namespace LanguageExt.ClassInstances
{
    public struct OrdTaskAsync<A> : OrdAsync<Task<A>>
    {
        public Task<int> CompareAsync(Task<A> x, Task<A> y) =>
            default(OrdTaskAsync<OrdDefaultAsync<A>, A>).CompareAsync(x, y);
 
        public Task<bool> EqualsAsync(Task<A> x, Task<A> y) =>
            default(EqTaskAsync<EqDefaultAsync<A>, A>).EqualsAsync(x, y);

        public Task<int> GetHashCodeAsync(Task<A> x) =>
            default(HashableTaskAsync<A>).GetHashCodeAsync(x);
    }

    public struct OrdTaskAsync<OrdA, A> : OrdAsync<Task<A>> where OrdA : struct, OrdAsync<A>
    {
        public async Task<int> CompareAsync(Task<A> x, Task<A> y)
        {
            var dx = await x;
            var dy = await y;
            return await default(OrdA).CompareAsync(dx, dy);
        }

        public Task<bool> EqualsAsync(Task<A> x, Task<A> y) =>
            default(EqTaskAsync<OrdA, A>).EqualsAsync(x, y);

        public Task<int> GetHashCodeAsync(Task<A> x) =>
            default(HashableTaskAsync<OrdA, A>).GetHashCodeAsync(x);
    }
}
