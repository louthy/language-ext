#nullable enable

using System;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using LanguageExt.Common;

namespace LanguageExt.ClassInstances
{
    public readonly struct ApplTryOptionAsync<A, B> : 
        BiFunctorAsync<TryOptionAsync<A>, TryOptionAsync<B>, Error, A, Error, B>,
        ApplicativeAsync<TryOptionAsync<Func<A, B>>, TryOptionAsync<A>, TryOptionAsync<B>, A, B>
    {
        public static readonly ApplTryOptionAsync<A, B> Inst = default;

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TryOptionAsync<B> BiMapAsync(TryOptionAsync<A> ma, Func<Error, ValueTask<Error>> fa, Func<A, ValueTask<B>> fb) =>
            default(FTryOptionAsync<A, B>).BiMapAsync(ma, fa, fb);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TryOptionAsync<B> Map(TryOptionAsync<A> ma, Func<A, B> f) => 
            default(FTryOptionAsync<A, B>).Map(ma, f);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TryOptionAsync<B> MapAsync(TryOptionAsync<A> ma, Func<A, ValueTask<B>> f) => 
            default(FTryOptionAsync<A, B>).MapAsync(ma, f);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TryOptionAsync<A> PureAsync(Task<A> x) =>
            Prelude.TryOptionAsync(x);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TryOptionAsync<B> Apply(TryOptionAsync<Func<A, B>> fab, TryOptionAsync<A> fa)
        {
            return Go;
            async Task<OptionalResult<B>> Go()
            {
                var (f, a) = await WaitAsync.All(fab.Try(), fa.Try());
                if(f.IsNone) return OptionalResult<B>.None;
                if(f.IsFaulted) return new OptionalResult<B>(f.Exception);
                if(a.IsNone) return OptionalResult<B>.None;
                if(a.IsFaulted) return new OptionalResult<B>(a.Exception);
                #nullable disable
                return f.Value.Value(a.Value.Value);
                #nullable enable
            }
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TryOptionAsync<B> Action(TryOptionAsync<A> fa, TryOptionAsync<B> fb)
        {
            return Go;
            async Task<OptionalResult<B>> Go()
            {
                var (a, b) = await WaitAsync.All(fa.Try(), fb.Try());
                if(a.IsNone) return OptionalResult<B>.None;
                if(a.IsFaulted) return new OptionalResult<B>(a.Exception);
                return b;
            }
        }
    }
}
