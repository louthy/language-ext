using LanguageExt.TypeClasses;
using System.Threading.Tasks;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Option type equality
    /// </summary>
    public struct EqOptionAsync<A> : EqAsync<OptionAsync<A>>
    {
        public Task<bool> Equals(OptionAsync<A> x, OptionAsync<A> y) =>
            default(EqOptionAsync<EqDefault<A>, A>).Equals(x, y);

        public Task<int> GetHashCode(OptionAsync<A> x) =>
            default(EqOptionAsync<EqDefault<A>, A>).GetHashCode(x);
    }

    /// <summary>
    /// Option type equality
    /// </summary>
    public struct EqOptionAsync<EqA, A> : EqAsync<OptionAsync<A>> where EqA : struct, Eq<A>
    {
        public Task<bool> Equals(OptionAsync<A> x, OptionAsync<A> y) =>
            default(EqOptionalAsync<MOptionAsync<A>, OptionAsync<A>, A>).Equals(x, y);

        public Task<int> GetHashCode(OptionAsync<A> x) =>
            default(EqOptionalAsync<MOptionAsync<A>, OptionAsync<A>, A>).GetHashCode(x);
    }
}
