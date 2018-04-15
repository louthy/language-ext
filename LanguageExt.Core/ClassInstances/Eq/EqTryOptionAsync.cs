using System.Threading.Tasks;
using LanguageExt.TypeClasses;

namespace LanguageExt.ClassInstances
{
    public struct EqTryOptionAsync<EqA, A> : EqAsync<TryOptionAsync<A>> where EqA : struct, Eq<A>
    {
        public Task<bool> Equals(TryOptionAsync<A> x, TryOptionAsync<A> y) =>
            default(ApplTryOptionAsync<A, bool>)
                .ApplyOption(default(EqOption<A>).Equals, x, y)
                .IfNoneOrFail(false);

        public Task<int> GetHashCode(TryOptionAsync<A> x) =>
            x.Map(default(EqA).GetHashCode)
             .IfNoneOrFail(0);
    }

    public struct EqTryOptionAsync<A> : EqAsync<TryOptionAsync<A>>
    {
        public Task<bool> Equals(TryOptionAsync<A> x, TryOptionAsync<A> y) =>
            default(EqTryOptionAsync<EqDefault<A>, A>).Equals(x, y);

        public Task<int> GetHashCode(TryOptionAsync<A> x) =>
            default(EqTryOptionAsync<EqDefault<A>, A>).GetHashCode(x);
    }
}
