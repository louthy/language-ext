using System;
using System.Threading.Tasks;
using LanguageExt.TypeClasses;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Either type equality
    /// </summary>
    [Obsolete(Change.UseEffMonadInstead)]
    public struct EqEitherAsync<L, R>
    {
        public static Task<bool> EqualsAsync(EitherAsync<L, R> x, EitherAsync<L, R> y) =>
            EqEitherAsync<EqDefault<L>, EqDefault<R>, L, R>.EqualsAsync(x, y);

        public static Task<int> GetHashCodeAsync(EitherAsync<L, R> x) =>
            HashableEitherAsync<L, R>.GetHashCodeAsync(x);
    }

    /// <summary>
    /// Either type equality
    /// </summary>
    [Obsolete(Change.UseEffMonadInstead)]
    public struct EqEitherAsync<EqL, EqR, L, R> 
        where EqL : Eq<L>
        where EqR : Eq<R>
    {
        public static async Task<bool> EqualsAsync(EitherAsync<L, R> x, EitherAsync<L, R> y)
        {
            var dx = await x.Data.ConfigureAwait(false);
            var dy = await x.Data.ConfigureAwait(false);
            return dx.State switch
            {
                EitherStatus.IsRight => dy.State switch
                {
                    EitherStatus.IsRight => EqR.Equals(dx.RightValue, dy.RightValue),
                    _                    => false
                },
                EitherStatus.IsLeft => dy.State switch
                {
                    EitherStatus.IsLeft => EqL.Equals(dx.LeftValue, dy.LeftValue),
                    _                   => false
                },
                EitherStatus.IsBottom => dy.State == EitherStatus.IsBottom,
                _ => false
            };
        }

        public static Task<int> GetHashCodeAsync(EitherAsync<L, R> x) =>
            HashableEitherAsync<EqL, EqR, L, R>.GetHashCodeAsync(x);
    }
}
