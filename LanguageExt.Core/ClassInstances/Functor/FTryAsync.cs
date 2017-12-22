using System;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;

namespace LanguageExt.ClassInstances
{
    public struct FTryAsync<A, B> : 
        FunctorAsync<TryAsync<A>, TryAsync<B>, A, B>,
        BiFunctorAsync<TryAsync<A>, TryAsync<B>, A, Unit, B>
    {
        public static readonly FTryAsync<A, B> Inst = default(FTryAsync<A, B>);

        [Pure]
        public TryAsync<B> BiMapAsync(TryAsync<A> ma, Func<A, B> fa, Func<Unit, B> fb) =>
            new TryAsync<B>(() => 
                default(MTryAsync<A>).MatchAsync(ma,
                    Some: a  => new Result<B>(fa(a)),
                    None: () => new Result<B>(fb(unit))));

        [Pure]
        public TryAsync<B> BiMapAsync(TryAsync<A> ma, Func<A, Task<B>> fa, Func<Unit, B> fb) =>
            new TryAsync<B>(async () =>
                await default(MTryAsync<A>).MatchAsync(ma,
                    Some: async a => new Result<B>(await fa(a)),
                    None: ()      => new Result<B>(fb(unit))));

        [Pure]
        public TryAsync<B> BiMapAsync(TryAsync<A> ma, Func<A, B> fa, Func<Unit, Task<B>> fb) =>
            new TryAsync<B>(async () =>
                await default(MTryAsync<A>).MatchAsync(ma,
                    Some: a        => new Result<B>(fa(a)),
                    None: async () => new Result<B>(await fb(unit))));

        [Pure]
        public TryAsync<B> BiMapAsync(TryAsync<A> ma, Func<A, Task<B>> fa, Func<Unit, Task<B>> fb) =>
            new TryAsync<B>(async () =>
                await default(MTryAsync<A>).MatchAsync(ma,
                    Some: async a  => new Result<B>(await fa(a)),
                    None: async () => new Result<B>(await fb(unit))));

        [Pure]
        public TryAsync<B> MapAsync(TryAsync<A> ma, Func<A, B> f) =>
            default(MTryAsync<A>).BindAsync<MTryAsync<B>, TryAsync<B>, B>(ma, a => Prelude.TryAsync(f(a)));

        [Pure]
        public TryAsync<B> MapAsync(TryAsync<A> ma, Func<A, Task<B>> f) =>
            default(MTryAsync<A>).BindAsync<MTryAsync<B>, TryAsync<B>, B>(ma, async a => Prelude.TryAsync(await f(a)));
    }
}
