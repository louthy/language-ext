using System;
using LanguageExt.Common;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;

namespace LanguageExt.ClassInstances
{
    public struct FTryOptionAsync<A, B> : 
        FunctorAsync<TryOptionAsync<A>, TryOptionAsync<B>, A, B>,
        BiFunctorAsync<TryOptionAsync<A>, TryOptionAsync<B>, A, Unit, B>
    {
        public static readonly FTryOptionAsync<A, B> Inst = default(FTryOptionAsync<A, B>);

        [Pure]
        public TryOptionAsync<B> BiMapAsync(TryOptionAsync<A> ma, Func<A, B> fa, Func<Unit, B> fb) =>
            new TryOptionAsync<B>(() =>
                default(MTryOptionAsync<A>).Match(ma,
                    Succ: a => new OptionalResult<B>(fa(a)),
                    Fail: () => new OptionalResult<B>(fb(unit))));

        [Pure]
        public TryOptionAsync<B> BiMapAsync(TryOptionAsync<A> ma, Func<A, Task<B>> fa, Func<Unit, B> fb) =>
            new TryOptionAsync<B>(async () =>
                await default(MTryOptionAsync<A>).MatchAsync(ma,
                    SuccAsync: async a => new OptionalResult<B>(await fa(a).ConfigureAwait(false)),
                    Fail: () => new OptionalResult<B>(fb(unit))).ConfigureAwait(false));

        [Pure]
        public TryOptionAsync<B> BiMapAsync(TryOptionAsync<A> ma, Func<A, B> fa, Func<Unit, Task<B>> fb) =>
            new TryOptionAsync<B>(async () =>
                await default(MTryOptionAsync<A>).MatchAsync(ma,
                    Succ: a => new OptionalResult<B>(fa(a)),
                    FailAsync: async () => new OptionalResult<B>(await fb(unit).ConfigureAwait(false))).ConfigureAwait(false));

        [Pure]
        public TryOptionAsync<B> BiMapAsync(TryOptionAsync<A> ma, Func<A, Task<B>> fa, Func<Unit, Task<B>> fb) =>
            new TryOptionAsync<B>(async () =>
                await default(MTryOptionAsync<A>).MatchAsync(ma,
                    SuccAsync: async a => new OptionalResult<B>(await fa(a).ConfigureAwait(false)),
                    FailAsync: async () => new OptionalResult<B>(await fb(unit).ConfigureAwait(false))).ConfigureAwait(false));

        [Pure]
        public TryOptionAsync<B> Map(TryOptionAsync<A> ma, Func<A, B> f) =>
            default(MTryOptionAsync<A>).Bind<MTryOptionAsync<B>, TryOptionAsync<B>, B>(ma, a => Prelude.TryOptionAsync(f(a)));

        [Pure]
        public TryOptionAsync<B> MapAsync(TryOptionAsync<A> ma, Func<A, Task<B>> f) =>
            default(MTryOptionAsync<A>).BindAsync<MTryOptionAsync<B>, TryOptionAsync<B>, B>(ma, async a => Prelude.TryOptionAsync(await f(a)));
    }
}
