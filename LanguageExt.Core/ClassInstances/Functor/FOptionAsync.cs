using System;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;

namespace LanguageExt.ClassInstances
{
    public struct FOptionAsync<A, B> : 
        FunctorAsync<OptionAsync<A>, OptionAsync<B>, A, B>,
        BiFunctorAsync<OptionAsync<A>, OptionAsync<B>, A, Unit, B>
    {
        public static readonly FOptionAsync<A, B> Inst = default(FOptionAsync<A, B>);

        [Pure]
        public OptionAsync<B> BiMapAsync(OptionAsync<A> ma, Func<A, B> fa, Func<Unit, B> fb)
        {
            async Task<(bool IsSome, B Value)> Do(OptionAsync<A> mma, Func<A, B> ffa, Func<Unit, B> ffb) =>
                await mma.Match(
                    Some: x  => (true, ffa(x)),
                    None: () => (true, ffb(unit))).ConfigureAwait(false);

            return new OptionAsync<B>(Do(ma, fa, fb));
        }

        [Pure]
        public OptionAsync<B> BiMapAsync(OptionAsync<A> ma, Func<A, Task<B>> fa, Func<Unit, B> fb)
        {
            async Task<(bool IsSome, B Value)> Do(OptionAsync<A> mma, Func<A, Task<B>> ffa, Func<Unit, B> ffb) =>
                await mma.MatchAsync(
                    Some: async x => (true, await ffa(x)),
                    None: ()      => (true, ffb(unit))).ConfigureAwait(false);

            return new OptionAsync<B>(Do(ma, fa, fb));
        }

        [Pure]
        public OptionAsync<B> BiMapAsync(OptionAsync<A> ma, Func<A, B> fa, Func<Unit, Task<B>> fb)
        {
            async Task<(bool IsSome, B Value)> Do(OptionAsync<A> mma, Func<A, B> ffa, Func<Unit, Task<B>> ffb) =>
                await mma.MatchAsync(
                    Some: x        => (true, ffa(x)),
                    None: async () => (true, await ffb(unit))).ConfigureAwait(false);

            return new OptionAsync<B>(Do(ma, fa, fb));
        }

        [Pure]
        public OptionAsync<B> BiMapAsync(OptionAsync<A> ma, Func<A, Task<B>> fa, Func<Unit, Task<B>> fb)
        {
            async Task<(bool IsSome, B Value)> Do(OptionAsync<A> mma, Func<A, Task<B>> ffa, Func<Unit, Task<B>> ffb) =>
                await mma.MatchAsync(
                    Some: async x  => (true, await ffa(x)),
                    None: async () => (true, await ffb(unit))).ConfigureAwait(false);

            return new OptionAsync<B>(Do(ma, fa, fb));
        }

        [Pure]
        public OptionAsync<B> Map(OptionAsync<A> ma, Func<A, B> f) =>
            default(MOptionAsync<A>).Bind<MOptionAsync<B>, OptionAsync<B>, B>(ma,
                a => default(MOptionAsync<B>).ReturnAsync(f(a).AsTask()));

        [Pure]
        public OptionAsync<B> MapAsync(OptionAsync<A> ma, Func<A, Task<B>> f) =>
            default(MOptionAsync<A>).Bind<MOptionAsync<B>, OptionAsync<B>, B>(ma,
                a => default(MOptionAsync<B>).ReturnAsync(f(a)));
    }
}
