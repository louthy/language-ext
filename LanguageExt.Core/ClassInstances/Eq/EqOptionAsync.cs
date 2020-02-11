using LanguageExt.TypeClasses;
using System.Threading.Tasks;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Option type equality
    /// </summary>
    public struct EqOptionAsync<A> : EqAsync<OptionAsync<A>>
    {
        public Task<bool> EqualsAsync(OptionAsync<A> x, OptionAsync<A> y) =>
            default(EqOptionAsync<EqDefault<A>, A>).EqualsAsync(x, y);

        public Task<int> GetHashCodeAsync(OptionAsync<A> x) =>
            default(HashableOptionAsync<A>).GetHashCodeAsync(x);
    }

    /// <summary>
    /// Option type equality
    /// </summary>
    public struct EqOptionAsync<EqA, A> : EqAsync<OptionAsync<A>> where EqA : struct, Eq<A>
    {
        public Task<bool> EqualsAsync(OptionAsync<A> x, OptionAsync<A> y) =>
            default(EqOptionalAsync<MOptionAsync<A>, OptionAsync<A>, A>).EqualsAsync(x, y);

        public Task<int> GetHashCodeAsync(OptionAsync<A> x) =>
            default(HashableOptionAsync<EqA, A>).GetHashCodeAsync(x);
    }
}
