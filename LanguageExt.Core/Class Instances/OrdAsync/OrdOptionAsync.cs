using LanguageExt.TypeClasses;
using System.Threading.Tasks;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Option type equality
    /// </summary>
    public struct OrdOptionAsync<A> : OrdAsync<OptionAsync<A>>
    {
        public Task<int> CompareAsync(OptionAsync<A> x, OptionAsync<A> y) =>
            default(OrdOptionAsync<OrdDefaultAsync<A>, A>).CompareAsync(x, y);

        public Task<bool> EqualsAsync(OptionAsync<A> x, OptionAsync<A> y) =>
            default(EqOptionAsync<EqDefaultAsync<A>, A>).EqualsAsync(x, y);

        public Task<int> GetHashCodeAsync(OptionAsync<A> x) =>
            default(HashableOptionAsync<A>).GetHashCodeAsync(x);
    }

    /// <summary>
    /// Option type equality
    /// </summary>
    public struct OrdOptionAsync<OrdA, A> : OrdAsync<OptionAsync<A>> where OrdA : struct, OrdAsync<A>
    {
        public async Task<int> CompareAsync(OptionAsync<A> x, OptionAsync<A> y)
        {
            var (sx, dx) = await x.Data.ConfigureAwait(false);
            var (sy, dy) = await y.Data.ConfigureAwait(false);
            if (!sx && !sy) return 0;
            if (sx && !sy) return 1;
            if (!sx && sy) return -1;
            return await default(OrdA).CompareAsync(dx, dy).ConfigureAwait(false);
        }

        public Task<bool> EqualsAsync(OptionAsync<A> x, OptionAsync<A> y) =>
            default(EqOptionAsync<OrdA, A>).EqualsAsync(x, y);

        public Task<int> GetHashCodeAsync(OptionAsync<A> x) =>
            default(HashableOptionAsync<OrdA, A>).GetHashCodeAsync(x);
    }
}
