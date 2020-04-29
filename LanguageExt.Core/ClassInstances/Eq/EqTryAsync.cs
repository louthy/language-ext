using System.Threading.Tasks;
using LanguageExt.TypeClasses;

namespace LanguageExt.ClassInstances
{
    public struct EqTryAsync<EqA, A> : EqAsync<TryAsync<A>> where EqA : struct, Eq<A>
    {
        public Task<bool> EqualsAsync(TryAsync<A> x, TryAsync<A> y) =>
            (from a in x
             from b in y
             select default(EqA).Equals(a, b)).IfFail(false);

        public Task<int> GetHashCodeAsync(TryAsync<A> x) =>
            default(HashableTryAsync<EqA, A>).GetHashCodeAsync(x);
    }

    public struct EqTryAsync<A> : EqAsync<TryAsync<A>>
    {
        public Task<bool> EqualsAsync(TryAsync<A> x, TryAsync<A> y) =>
            default(EqTryAsync<EqDefault<A>, A>).EqualsAsync(x, y);

        public Task<int> GetHashCodeAsync(TryAsync<A> x) =>
            default(HashableTryAsync<A>).GetHashCodeAsync(x);
    }
}
