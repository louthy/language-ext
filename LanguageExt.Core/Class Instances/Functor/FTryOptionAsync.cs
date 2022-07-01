#nullable enable

using System;
using LanguageExt.Common;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace LanguageExt.ClassInstances
{
    public readonly struct FTryOptionAsync<A, B> : 
        FunctorAsync<TryOptionAsync<A>, TryOptionAsync<B>, A, B>,
        BiFunctorAsync<TryOptionAsync<A>, TryOptionAsync<B>, Error, A, Error, B>
    {
        public static readonly FTryOptionAsync<A, B> Inst = default;
        
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TryOptionAsync<B> Map(TryOptionAsync<A> ma, Func<A, B> f) =>
            ma.Map(f);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TryOptionAsync<B> MapAsync(TryOptionAsync<A> ma, Func<A, ValueTask<B>> f) => 
            ma.MapAsync(async x => await f(x).ConfigureAwait(false));

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TryOptionAsync<B> BiMapAsync(TryOptionAsync<A> ma, Func<Error, ValueTask<Error>> fa, Func<A, ValueTask<B>> fb) => 
            ma.BiMapAsync(fb, fa);
    }
}
