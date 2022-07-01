#nullable enable

using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using LanguageExt.Common;
using LanguageExt.TypeClasses;
using System.Threading.Tasks;

namespace LanguageExt.ClassInstances
{
    public readonly struct FTryAsync<A, B> : 
        FunctorAsync<TryAsync<A>, TryAsync<B>, A, B>,
        BiFunctorAsync<TryAsync<A>, TryAsync<B>, Error, A, Error, B>
    {
        public static readonly FTryAsync<A, B> Inst = default;

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TryAsync<B> Map(TryAsync<A> ma, Func<A, B> f)
        {
            return Go;
            
            async Task<Result<B>> Go()
            {
                var r = await ma.Try().ConfigureAwait(false);
                return r switch
                {
                    {IsSuccess: true} x => new Result<B>(f(x.Value)),
                    {IsFaulted: true} x => new Result<B>(x.Exception),
                    _ => Result<B>.Bottom
                };
            }
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TryAsync<B> MapAsync(TryAsync<A> ma, Func<A, ValueTask<B>> f) 
        {
            return Go;
            
            async Task<Result<B>> Go()
            {
                var r = await ma.Try().ConfigureAwait(false);
                return r switch
                {
                    {IsSuccess: true} x => new Result<B>(await f(x.Value).ConfigureAwait(false)),
                    {IsFaulted: true} x => new Result<B>(x.Exception),
                    _ => Result<B>.Bottom
                };
            }
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TryAsync<B> BiMapAsync(TryAsync<A> ma, Func<Error, ValueTask<Error>> fa, Func<A, ValueTask<B>> fb) 
        {
            return Go;
            
            async Task<Result<B>> Go()
            {
                var r = await ma.Try().ConfigureAwait(false);
                return r switch
                {
                    {IsSuccess: true} x => new Result<B>(await fb(x.Value).ConfigureAwait(false)),
                    {IsFaulted: true} x => new Result<B>(await fa(x.Exception).ConfigureAwait(false)),
                    _ => Result<B>.Bottom
                };
            }
        }
    }
}
