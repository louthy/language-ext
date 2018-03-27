using System.Threading.Tasks;
using LanguageExt.TypeClasses;

namespace LanguageExt.ClassInstances
{
    public struct EqTryAsync<EqA, A> : EqAsync<TryAsync<A>> where EqA : struct, Eq<A>
    {
        public Task<bool> Equals(TryAsync<A> x, TryAsync<A> y) =>
            (from a in x
             from b in y
             select default(EqA).Equals(a, b)).IfFail(false);

        public Task<int> GetHashCode(TryAsync<A> x) =>
            x.Map(default(EqA).GetHashCode).IfFail(0);
    }

    public struct EqTryAsync<A> : EqAsync<TryAsync<A>>
    {
        public Task<bool> Equals(TryAsync<A> x, TryAsync<A> y) =>
            default(EqTryAsync<EqDefault<A>, A>).Equals(x, y);

        public Task<int> GetHashCode(TryAsync<A> x) =>
            default(EqTryAsync<EqDefault<A>, A>).GetHashCode(x);
    }
}
