using System.Threading.Tasks;
using LanguageExt.TypeClasses;

namespace LanguageExt.ClassInstances
{
    public struct OrdTryAsync<OrdA, A> : OrdAsync<TryAsync<A>> where OrdA : struct, OrdAsync<A>
    {
        public async Task<int> CompareAsync(TryAsync<A> x, TryAsync<A> y)
        {
            var dx = await x.Try();
            var dy = await y.Try();
            if (dx.IsBottom && dy.IsBottom) return 0;
            if (dx.IsFaulted && dy.IsFaulted) return 0;
            if (dx.IsSuccess && dy.IsSuccess)
            {
                return await default(OrdA).CompareAsync(dx.Value, dy.Value);
            }

            if (dx.IsBottom && !dy.IsBottom) return -1;
            if (!dx.IsBottom && dy.IsBottom) return 1;

            if (dx.IsFaulted && !dy.IsFaulted) return -1;
            if (!dx.IsFaulted && dy.IsFaulted) return 1;

            if (dx.IsSuccess && !dy.IsSuccess) return -1;
            if (!dx.IsSuccess && dy.IsSuccess) return 1;
            return 0;
        }

        public Task<bool> EqualsAsync(TryAsync<A> x, TryAsync<A> y) =>
            default(EqTryAsync<OrdA, A>).EqualsAsync(x, y);

        public Task<int> GetHashCodeAsync(TryAsync<A> x) =>
            default(HashableTryAsync<OrdA, A>).GetHashCodeAsync(x);
    }

    public struct OrdTryAsync<A> : OrdAsync<TryAsync<A>>
    {
        public Task<int> CompareAsync(TryAsync<A> x, TryAsync<A> y) =>
            default(OrdTryAsync<OrdDefaultAsync<A>, A>).CompareAsync(x, y);
 
        public Task<bool> EqualsAsync(TryAsync<A> x, TryAsync<A> y) =>
            default(EqTryAsync<EqDefault<A>, A>).EqualsAsync(x, y);

        public Task<int> GetHashCodeAsync(TryAsync<A> x) =>
            default(HashableTryAsync<A>).GetHashCodeAsync(x);
    }
}
