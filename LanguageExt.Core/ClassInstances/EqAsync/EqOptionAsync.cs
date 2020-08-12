using LanguageExt.TypeClasses;
using System.Threading.Tasks;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Option type equality
    /// </summary>
    public struct EqOptionAsync<EqA, A> : EqAsync<OptionAsync<A>>
        where EqA : struct, EqAsync<A>
    {
        public async Task<bool> EqualsAsync(OptionAsync<A> x, OptionAsync<A> y)
        {
            var (sx, dx) = await x.Data.ConfigureAwait(false);
            var (sy, dy) = await y.Data.ConfigureAwait(false);
            if (!sx && !sy) return true;
            if (sx != sy) return false;
            return await default(EqA).EqualsAsync(dx, dy).ConfigureAwait(false);
        }

        public Task<int> GetHashCodeAsync(OptionAsync<A> x) =>
            default(HashableOptionAsync<A>).GetHashCodeAsync(x);
    }

    /// <summary>
    /// Option type equality
    /// </summary>
    public struct EqOptionAsync<A> : EqAsync<OptionAsync<A>>
    {
        public Task<bool> EqualsAsync(OptionAsync<A> x, OptionAsync<A> y) =>
            default(EqOptionAsync<EqDefaultAsync<A>, A>).EqualsAsync(x, y);

        public Task<int> GetHashCodeAsync(OptionAsync<A> x) =>
            default(HashableOptionAsync<HashableDefaultAsync<A>, A>).GetHashCodeAsync(x);
    }
}
