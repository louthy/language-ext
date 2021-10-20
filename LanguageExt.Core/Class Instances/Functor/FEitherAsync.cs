using LanguageExt.TypeClasses;
using System;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;

namespace LanguageExt.ClassInstances
{
    public struct FEitherAsync<L, R, R2> :
        FunctorAsync<EitherAsync<L, R>, EitherAsync<L, R2>, R, R2>,
        BiFunctorAsync<EitherAsync<L, R>, EitherAsync<L, R2>, L, R, R2>
    {
        [Pure]
        public EitherAsync<L, R2> BiMapAsync(EitherAsync<L, R> ma, Func<L, R2> fa, Func<R, R2> fb) =>
            ma.BiMap<R2>(fb, fa);

        [Pure]
        public EitherAsync<L, R2> BiMapAsync(EitherAsync<L, R> ma, Func<L, Task<R2>> fa, Func<R, R2> fb) =>
            ma.BiMapAsync<R2>(fb, fa);

        [Pure]
        public EitherAsync<L, R2> BiMapAsync(EitherAsync<L, R> ma, Func<L, R2> fa, Func<R, Task<R2>> fb) =>
            ma.BiMapAsync<R2>(fb, fa);

        [Pure]
        public EitherAsync<L, R2> BiMapAsync(EitherAsync<L, R> ma, Func<L, Task<R2>> fa, Func<R, Task<R2>> fb) =>
            ma.BiMapAsync<R2>(fb, fa);

        [Pure]
        public EitherAsync<L, R2> Map(EitherAsync<L, R> ma, Func<R, R2> f) =>
            ma.Map(f);

        [Pure]
        public EitherAsync<L, R2> MapAsync(EitherAsync<L, R> ma, Func<R, Task<R2>> f) =>
            ma.MapAsync(f);
    }
}
