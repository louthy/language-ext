using LanguageExt.TypeClasses;
using System.Threading.Tasks;

namespace LanguageExt.ClassInstances
{
    public struct OrdValueTaskAsync<A> : OrdAsync<ValueTask<A>>
    {
        public Task<int> CompareAsync(ValueTask<A> x, ValueTask<A> y) =>
            default(OrdValueTaskAsync<OrdDefaultAsync<A>, A>).CompareAsync(x, y);
 
        public Task<bool> EqualsAsync(ValueTask<A> x, ValueTask<A> y) =>
            default(EqValueTaskAsync<EqDefaultAsync<A>, A>).EqualsAsync(x, y);

        public Task<int> GetHashCodeAsync(ValueTask<A> x) =>
            default(HashableValueTaskAsync<A>).GetHashCodeAsync(x);
    }

    public struct OrdValueTaskAsync<OrdA, A> : OrdAsync<ValueTask<A>> where OrdA : struct, OrdAsync<A>
    {
        public async Task<int> CompareAsync(ValueTask<A> x, ValueTask<A> y)
        {
            var dx = await x.ConfigureAwait(false);
            var dy = await y.ConfigureAwait(false);
            return await default(OrdA).CompareAsync(dx, dy).ConfigureAwait(false);
        }

        public Task<bool> EqualsAsync(ValueTask<A> x, ValueTask<A> y) =>
            default(EqValueTaskAsync<OrdA, A>).EqualsAsync(x, y);

        public Task<int> GetHashCodeAsync(ValueTask<A> x) =>
            default(HashableValueTaskAsync<OrdA, A>).GetHashCodeAsync(x);
    }
}
