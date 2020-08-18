using System.Data.SqlTypes;
using LanguageExt.TypeClasses;
using System.Threading.Tasks;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Either type ordering
    /// </summary>
    public struct OrdEitherAsync<L, A> : OrdAsync<EitherAsync<L, A>>
    {
        public Task<int> CompareAsync(EitherAsync<L, A> x, EitherAsync<L, A> y) =>
            default(OrdEitherAsync<OrdDefaultAsync<L>, OrdDefaultAsync<A>, L, A>).CompareAsync(x, y);

        public Task<bool> EqualsAsync(EitherAsync<L, A> x, EitherAsync<L, A> y) =>
            default(EqEitherAsync<EqDefault<L>, EqDefault<A>, L, A>).EqualsAsync(x, y);

        public Task<int> GetHashCodeAsync(EitherAsync<L, A> x) =>
            default(HashableEitherAsync<L, A>).GetHashCodeAsync(x);
    }

    /// <summary>
    /// Either type ordering
    /// </summary>
    public struct OrdEitherAsync<OrdL, OrdA, L, A> : OrdAsync<EitherAsync<L, A>> 
        where OrdL : struct, OrdAsync<L>
        where OrdA : struct, OrdAsync<A>
    {
        public async Task<int> CompareAsync(EitherAsync<L, A> x, EitherAsync<L, A> y)
        {
            var dx = await x.Data.ConfigureAwait(false);
            var dy = await y.Data.ConfigureAwait(false);
            
            return dx.State switch
            {
                EitherStatus.IsRight => dy.State switch
                {
                    EitherStatus.IsRight => await default(OrdA).CompareAsync(dx.Right, dy.Right).ConfigureAwait(false),
                    EitherStatus.IsLeft => 1,
                    _ => 1
                },
                EitherStatus.IsLeft => dy.State switch
                {
                    EitherStatus.IsLeft => await default(OrdL).CompareAsync(dx.Left, dy.Left).ConfigureAwait(false),
                    EitherStatus.IsRight => -1,
                    _ => 1
                },
                EitherStatus.IsBottom => dy.State == EitherStatus.IsBottom ? 0 : -1,
                _ => 0
            };
        }

        public Task<bool> EqualsAsync(EitherAsync<L, A> x, EitherAsync<L, A> y) =>
            default(EqEitherAsync<OrdL, OrdA, L, A>).EqualsAsync(x, y);

        public Task<int> GetHashCodeAsync(EitherAsync<L, A> x) =>
            default(HashableEitherAsync<OrdL, OrdA, L, A>).GetHashCodeAsync(x);
    }
}
