using System.Data.SqlTypes;
using LanguageExt.TypeClasses;
using System.Threading.Tasks;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Either type equality
    /// </summary>
    public struct EqEitherAsync<L, R> : EqAsync<EitherAsync<L, R>>
    {
        public Task<bool> EqualsAsync(EitherAsync<L, R> x, EitherAsync<L, R> y) =>
            default(EqEitherAsync<EqDefaultAsync<L>, EqDefaultAsync<R>, L, R>).EqualsAsync(x, y);

        public Task<int> GetHashCodeAsync(EitherAsync<L, R> x) =>
            default(HashableEitherAsync<L, R>).GetHashCodeAsync(x);
    }

    /// <summary>
    /// Either type equality
    /// </summary>
    public struct EqEitherAsync<EqL, EqR, L, R> : EqAsync<EitherAsync<L, R>> 
        where EqL : struct, EqAsync<L>
        where EqR : struct, EqAsync<R>
    {
        public async Task<bool> EqualsAsync(EitherAsync<L, R> x, EitherAsync<L, R> y)
        {
            var dx = await x.Data.ConfigureAwait(false);
            var dy = await x.Data.ConfigureAwait(false);
            return dx.State switch
            {
                EitherStatus.IsRight => dy.State switch
                {
                    EitherStatus.IsRight => await default(EqR).EqualsAsync(dx.Right, dy.Right).ConfigureAwait(false),
                    _ => false
                },
                EitherStatus.IsLeft => dy.State switch
                {
                    EitherStatus.IsLeft => await default(EqL).EqualsAsync(dx.Left, dy.Left).ConfigureAwait(false),
                    _ => false
                },
                EitherStatus.IsBottom => dy.State == EitherStatus.IsBottom,
                _ => false
            };
        }

        public Task<int> GetHashCodeAsync(EitherAsync<L, R> x) =>
            default(HashableEitherAsync<EqL, EqR, L, R>).GetHashCodeAsync(x);
    }
}
