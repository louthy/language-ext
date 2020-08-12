using System.Threading.Tasks;
using LanguageExt.TypeClasses;

namespace LanguageExt.ClassInstances
{
    public struct EqTryOptionAsync<EqA, A> : EqAsync<TryOptionAsync<A>> where EqA : struct, EqAsync<A>
    {
        public async Task<bool> EqualsAsync(TryOptionAsync<A> x, TryOptionAsync<A> y)
        {
            var dx = await x.Try().ConfigureAwait(false);
            var dy = await y.Try().ConfigureAwait(false);
            if (dx.IsBottom && dy.IsBottom) return true;
            if (dx.IsFaulted && dy.IsFaulted) return true;
            if (dx.IsNone && dy.IsNone) return true;
            if (dx.IsSome && dy.IsSome)
            {
                return await default(EqA).EqualsAsync(dx.Value.Value, dy.Value.Value).ConfigureAwait(false);
            }
            else
            {
                return false;
            }
        }
        public Task<int> GetHashCodeAsync(TryOptionAsync<A> x) =>
            default(HashableTryOptionAsync<EqA, A>).GetHashCodeAsync(x);
    }

    public struct EqTryOptionAsync<A> : EqAsync<TryOptionAsync<A>>
    {
        public Task<bool> EqualsAsync(TryOptionAsync<A> x, TryOptionAsync<A> y) =>
            default(EqTryOptionAsync<EqDefaultAsync<A>, A>).EqualsAsync(x, y);

        public Task<int> GetHashCodeAsync(TryOptionAsync<A> x) =>
            default(HashableTryOptionAsync<A>).GetHashCodeAsync(x);
    }
}
