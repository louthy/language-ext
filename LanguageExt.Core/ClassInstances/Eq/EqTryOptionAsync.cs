using System.Threading.Tasks;
using LanguageExt.TypeClasses;

namespace LanguageExt.ClassInstances
{
    public struct EqTryOptionAsync<EqA, A> : EqAsync<TryOptionAsync<A>> where EqA : struct, Eq<A>
    {
        public Task<bool> EqualsAsync(TryOptionAsync<A> x, TryOptionAsync<A> y) =>
            default(ApplTryOptionAsync<A, bool>)
                .ApplyOption(default(EqOption<A>).Equals, x, y)
                .IfNoneOrFail(false);

        public Task<int> GetHashCodeAsync(TryOptionAsync<A> x) =>
            default(HashableTryOptionAsync<EqA, A>).GetHashCodeAsync(x);
    }

    public struct EqTryOptionAsync<A> : EqAsync<TryOptionAsync<A>>
    {
        public Task<bool> EqualsAsync(TryOptionAsync<A> x, TryOptionAsync<A> y) =>
            default(EqTryOptionAsync<EqDefault<A>, A>).EqualsAsync(x, y);

        public Task<int> GetHashCodeAsync(TryOptionAsync<A> x) =>
            default(HashableTryOptionAsync<A>).GetHashCodeAsync(x);
    }
}
