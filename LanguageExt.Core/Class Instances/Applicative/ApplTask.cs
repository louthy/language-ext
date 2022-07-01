#nullable enable

using System;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using LanguageExt.Common;

namespace LanguageExt.ClassInstances
{
    public readonly struct ApplTask<A, B> : 
        BiFunctorAsync<Task<A>, Task<B>, Error, A, Error, B>,
        ApplicativeAsync<Task<Func<A, B>>, Task<A>, Task<B>, A, B>
    {
        public static readonly ApplTask<A, B> Inst = default;

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async Task<B> Apply(Task<Func<A, B>> fab, Task<A> fa)
        {
            var (f, a) = await WaitAsync.All(fab, fa).ConfigureAwait(false);
            return f(a);
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<A> PureAsync(Task<A> x) =>
            x;

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async Task<B> Action(Task<A> fa, Task<B> fb)
        {
            await WaitAsync.All(fa, fb).ConfigureAwait(false);
            return await fb.ConfigureAwait(false);
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<B> BiMapAsync(Task<A> ma, Func<Error, ValueTask<Error>> fa, Func<A, ValueTask<B>> fb) => 
            default(FTask<A, B>).BiMapAsync(ma, fa, fb);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<B> Map(Task<A> ma, Func<A, B> f) => 
            default(FTask<A, B>).Map(ma, f);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<B> MapAsync(Task<A> ma, Func<A, ValueTask<B>> f) => 
            default(FTask<A, B>).MapAsync(ma, f);
    }
}
