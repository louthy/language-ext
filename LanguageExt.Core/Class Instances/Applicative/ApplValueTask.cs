#nullable enable

using System;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using LanguageExt.Common;

namespace LanguageExt.ClassInstances
{
    public readonly struct ApplValueTask<A, B> : 
        BiFunctorAsync<ValueTask<A>, ValueTask<B>, Error, A, Error, B>,
        ApplicativeAsync<ValueTask<Func<A, B>>, ValueTask<A>, ValueTask<B>, A, B>
    {
        public static readonly ApplValueTask<A, B> Inst = default;

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async ValueTask<B> Apply(ValueTask<Func<A, B>> fab, ValueTask<A> fa)
        {
            var (f, a) = await WaitAsync.All(fab, fa).ConfigureAwait(false);
            return f(a);
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async ValueTask<A> PureAsync(Task<A> x) =>
            await x.ConfigureAwait(false);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async ValueTask<B> Action(ValueTask<A> fa, ValueTask<B> fb)
        {
            await WaitAsync.All(fa, fb).ConfigureAwait(false);
            return await fb.ConfigureAwait(false);
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ValueTask<B> BiMapAsync(ValueTask<A> ma, Func<Error, ValueTask<Error>> fa, Func<A, ValueTask<B>> fb) => 
            default(FValueTask<A, B>).BiMapAsync(ma, fa, fb);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ValueTask<B> Map(ValueTask<A> ma, Func<A, B> f) => 
            default(FValueTask<A, B>).Map(ma, f);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ValueTask<B> MapAsync(ValueTask<A> ma, Func<A, ValueTask<B>> f) => 
            default(FValueTask<A, B>).MapAsync(ma, f);
    }
}
