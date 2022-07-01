#nullable enable

using System;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using LanguageExt.Common;

namespace LanguageExt.ClassInstances
{
    public readonly struct FValueTask<A, B> : 
        FunctorAsync<ValueTask<A>, ValueTask<B>, A, B>,
        BiFunctorAsync<ValueTask<A>, ValueTask<B>, Error, A, Error, B>
    {
        public static readonly FTask<A, B> Inst = default;

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async ValueTask<B> Map(ValueTask<A> ma, Func<A, B> f) =>
            f(await ma.ConfigureAwait(false));

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async ValueTask<B> MapAsync(ValueTask<A> ma, Func<A, ValueTask<B>> f) =>
            await f(await ma.ConfigureAwait(false)).ConfigureAwait(false);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async ValueTask<B> BiMapAsync(ValueTask<A> ma, Func<Error, ValueTask<Error>> fa, Func<A, ValueTask<B>> fb)
        {
            try
            {
                return await fb(await ma.ConfigureAwait(false)).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                var nx = await fa(e).ConfigureAwait(false);
                return nx.Throw<B>();
            }
        }
    }
}
