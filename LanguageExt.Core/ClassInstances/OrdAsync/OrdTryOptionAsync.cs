using System.Threading.Tasks;
using LanguageExt.TypeClasses;

namespace LanguageExt.ClassInstances
{
    public struct OrdTryOptionAsync<OrdA, A> : OrdAsync<TryOptionAsync<A>> where OrdA : struct, OrdAsync<A>
    {
        public async Task<int> CompareAsync(TryOptionAsync<A> x, TryOptionAsync<A> y)
        {
            var dx = await x.Try().ConfigureAwait(false);
            var dy = await y.Try().ConfigureAwait(false);
            if (dx.IsBottom && dy.IsBottom) return 0;
            if (dx.IsFaulted && dy.IsFaulted) return 0;
            if (dx.IsNone && dy.IsNone) return 0;
            if (dx.IsSome && dy.IsSome)
            {
                return await default(OrdA).CompareAsync(dx.Value.Value, dy.Value.Value).ConfigureAwait(false);
            }

            if (dx.IsBottom && !dy.IsBottom) return -1;
            if (!dx.IsBottom && dy.IsBottom) return 1;

            if (dx.IsFaulted && !dy.IsFaulted) return -1;
            if (!dx.IsFaulted && dy.IsFaulted) return 1;

            if (dx.IsNone && !dy.IsNone) return -1;
            if (!dx.IsNone && dy.IsNone) return 1;

            if (dx.IsSome && !dy.IsSome) return -1;
            if (!dx.IsSome && dy.IsSome) return 1;
            return 0;
        }

        public Task<bool> EqualsAsync(TryOptionAsync<A> x, TryOptionAsync<A> y) =>
            default(EqTryOptionAsync<OrdA, A>).EqualsAsync(x, y);

        public Task<int> GetHashCodeAsync(TryOptionAsync<A> x) =>
            default(HashableTryOptionAsync<OrdA, A>).GetHashCodeAsync(x);
    }

    public struct OrdTryOptionAsync<A> : OrdAsync<TryOptionAsync<A>>
    {
        public Task<int> CompareAsync(TryOptionAsync<A> x, TryOptionAsync<A> y) =>
            default(OrdTryOptionAsync<OrdDefaultAsync<A>, A>).CompareAsync(x, y);

        public Task<bool> EqualsAsync(TryOptionAsync<A> x, TryOptionAsync<A> y) =>
            default(EqTryOptionAsync<EqDefaultAsync<A>, A>).EqualsAsync(x, y);

        public Task<int> GetHashCodeAsync(TryOptionAsync<A> x) =>
            default(HashableTryOptionAsync<A>).GetHashCodeAsync(x);
    }
}
