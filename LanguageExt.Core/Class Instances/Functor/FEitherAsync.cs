#nullable enable

using LanguageExt.TypeClasses;
using System;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using LanguageExt.DataTypes.Serialisation;

namespace LanguageExt.ClassInstances
{
    public readonly struct FEitherAsync<L, A, B> :
        FunctorAsync<EitherAsync<L, A>, EitherAsync<L, B>, A, B>,
        BiFunctorAsync<EitherAsync<L, A>, EitherAsync<L, B>, L, A, L, B>
    {
        [Pure]
        public EitherAsync<L, B> BiMapAsync(EitherAsync<L, A> ma, Func<L, ValueTask<L>> fa, Func<A, ValueTask<B>> fb)
        {
            return new EitherAsync<L, B>(Go());
            async Task<EitherData<L, B>> Go()
            {
                var r = await ma.Data.ConfigureAwait(false);
                return r.State switch
                {
                    EitherStatus.IsRight => EitherData.Right<L, B>(await fb(r.Right).ConfigureAwait(false)),
                    EitherStatus.IsLeft  => EitherData.Left<L, B>(await fa(r.Left).ConfigureAwait(false)),
                    _ => EitherData<L, B>.Bottom
                };
            }
        }
        
        [Pure]
        public EitherAsync<L, B> MapAsync(EitherAsync<L, A> ma, Func<A, ValueTask<B>> f)
        {
            return new EitherAsync<L, B>(Go());
            async Task<EitherData<L, B>> Go()
            {
                var r = await ma.Data.ConfigureAwait(false);
                return r.State switch
                {
                    EitherStatus.IsRight => EitherData.Right<L, B>(await f(r.Right).ConfigureAwait(false)),
                    EitherStatus.IsLeft  => EitherData.Left<L, B>(r.Left),
                    _ => EitherData<L, B>.Bottom
                };
            }
        }

        [Pure]
        public EitherAsync<L, B> Map(EitherAsync<L, A> ma, Func<A, B> f) 
        {
            return new EitherAsync<L, B>(Go());
            async Task<EitherData<L, B>> Go()
            {
                var r = await ma.Data.ConfigureAwait(false);
                return r.State switch
                {
                    EitherStatus.IsRight => EitherData.Right<L, B>(f(r.Right)),
                    EitherStatus.IsLeft  => EitherData.Left<L, B>(r.Left),
                    _ => EitherData<L, B>.Bottom
                };
            }
        }
    }
}
