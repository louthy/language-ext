using System.Threading.Tasks;
using LanguageExt.TypeClasses;

namespace LanguageExt.ClassInstances
{
    public struct EqTryAsync<EqA, A> : EqAsync<TryAsync<A>> where EqA : struct, EqAsync<A>
    {
        public async Task<bool> EqualsAsync(TryAsync<A> x, TryAsync<A> y)
        {
            var dx = await x.Try();
            var dy = await y.Try();
            if (dx.IsBottom && dy.IsBottom) return true;
            if (dx.IsFaulted && dy.IsFaulted) return true;
            if (dx.IsSuccess && dy.IsSuccess)
            {
                return await default(EqA).EqualsAsync(dx.Value, dy.Value);
            }
            else
            {
                return false;
            }
        }

        public Task<int> GetHashCodeAsync(TryAsync<A> x) =>
            default(HashableTryAsync<EqA, A>).GetHashCodeAsync(x);
    }

    public struct EqTryAsync<A> : EqAsync<TryAsync<A>>
    {
        public Task<bool> EqualsAsync(TryAsync<A> x, TryAsync<A> y) =>
            default(EqTryAsync<EqDefaultAsync<A>, A>).EqualsAsync(x, y);

        public Task<int> GetHashCodeAsync(TryAsync<A> x) =>
            default(HashableTryAsync<A>).GetHashCodeAsync(x);
    }
}
