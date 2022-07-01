#nullable enable

using System;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using LanguageExt.Common;

namespace LanguageExt.ClassInstances
{
    public readonly struct ApplTryAsync<A, B> : 
        BiFunctorAsync<TryAsync<A>, TryAsync<B>, Error, A, Error, B>,
        ApplicativeAsync<TryAsync<Func<A, B>>, TryAsync<A>, TryAsync<B>, A, B>
    {
        public static readonly ApplTryAsync<A, B> Inst = default;

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TryAsync<B> Apply(TryAsync<Func<A, B>> fab, TryAsync<A> fa) =>
            async () =>
            {
                var (f, a) = await WaitAsync.All(fab.Try(), fa.Try()).ConfigureAwait(false);
                
                if (f.IsFaulted) return new Result<B>(f.Exception);
                if (a.IsFaulted) return new Result<B>(a.Exception);
                
                return new Result<B>(f.Value(a.Value));
            };

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TryAsync<A> PureAsync(Task<A> x) =>
            MTryAsync<A>.Inst.ReturnAsync(x);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TryAsync<B> Action(TryAsync<A> fa, TryAsync<B> fb) =>
            async () =>
            {
                var (a, b) = await WaitAsync.All(fa.Try(), fb.Try()).ConfigureAwait(false);
                if (a.IsFaulted) return new Result<B>(a.Exception);
                return b;
            };

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TryAsync<B> BiMapAsync(TryAsync<A> ma, Func<Error, ValueTask<Error>> fa, Func<A, ValueTask<B>> fb) => 
            default(FTryAsync<A, B>).BiMapAsync(ma, fa, fb);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TryAsync<B> Map(TryAsync<A> ma, Func<A, B> f) => 
            default(FTryAsync<A, B>).Map(ma, f);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TryAsync<B> MapAsync(TryAsync<A> ma, Func<A, ValueTask<B>> f) => 
            default(FTryAsync<A, B>).MapAsync(ma, f);
    }
}
