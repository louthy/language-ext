using System;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;

namespace LanguageExt.ClassInstances
{
    public struct FValueTask<A, B> : 
        FunctorAsync<ValueTask<A>, ValueTask<B>, A, B>,
        BiFunctorAsync<ValueTask<A>, ValueTask<B>, A, Unit, B>
    {
        public static readonly FValueTask<A, B> Inst = default(FValueTask<A, B>);

        [Pure]
        public ValueTask<B> BiMapAsync(ValueTask<A> ma, Func<A, B> fa, Func<Unit, B> fb) =>
            default(MValueTask<A>).MatchAsync(ma, a => fa(a).AsTask(), () => fb(unit).AsTask()).ToValue();

        [Pure]
        public ValueTask<B> BiMapAsync(ValueTask<A> ma, Func<A, Task<B>> fa, Func<Unit, B> fb) =>
            default(MValueTask<A>).MatchAsync(ma, fa, () => fb(unit).AsTask()).ToValue();

        [Pure]
        public ValueTask<B> BiMapAsync(ValueTask<A> ma, Func<A, B> fa, Func<Unit, Task<B>> fb) =>
            default(MValueTask<A>).MatchAsync(ma, fa, () => fb(unit)).ToValue();

        [Pure]
        public ValueTask<B> BiMapAsync(ValueTask<A> ma, Func<A, Task<B>> fa, Func<Unit, Task<B>> fb) =>
            default(MValueTask<A>).MatchAsync(ma, fa, () => fb(unit)).ToValue();

        [Pure]
        public async ValueTask<B> Map(ValueTask<A> ma, Func<A, B> f) =>
            f(await ma.ConfigureAwait(false));

        [Pure]
        public async ValueTask<B> MapAsync(ValueTask<A> ma, Func<A, Task<B>> f) =>
            await f(await ma.ConfigureAwait(false)).ConfigureAwait(false);
    }
}
