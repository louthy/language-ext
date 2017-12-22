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
        public OptionAsync<B> BiMapAsync(OptionAsync<A> ma, Func<A, B> fa, Func<Unit, B> fb) =>
            new OptionAsync<B>(OptionDataAsync.Lazy<B>(async () =>
                await ma.Match(
                    Some: x => fa(x),
                    None: () => fb(unit))));

        [Pure]
        public OptionAsync<B> BiMapAsync(OptionAsync<A> ma, Func<A, Task<B>> fa, Func<Unit, B> fb) =>
            new OptionAsync<B>(OptionDataAsync.Lazy<B>(async () =>
                await ma.Match(
                    Some: x => fa(x),
                    None: () => fb(unit))));

        [Pure]
        public OptionAsync<B> BiMapAsync(OptionAsync<A> ma, Func<A, B> fa, Func<Unit, Task<B>> fb) =>
            new OptionAsync<B>(OptionDataAsync.Lazy<B>(async () =>
                await ma.Match(
                    Some: x => fa(x),
                    None: () => fb(unit))));

        [Pure]
        public OptionAsync<B> BiMapAsync(OptionAsync<A> ma, Func<A, Task<B>> fa, Func<Unit, Task<B>> fb) =>
            new OptionAsync<B>(OptionDataAsync.Lazy<B>(async () =>
                await ma.Match(
                    Some: x => fa(x),
                    None: () => fb(unit))));

        [Pure]
        public OptionAsync<B> MapAsync(OptionAsync<A> ma, Func<A, B> f) =>
            default(MOptionAsync<A>).BindAsync<MOptionAsync<B>, OptionAsync<B>, B>(ma,
                a => default(MOptionAsync<B>).ReturnAsync(f(a).AsTask()));

        [Pure]
        public OptionAsync<B> MapAsync(OptionAsync<A> ma, Func<A, Task<B>> f) =>
            default(MOptionAsync<A>).BindAsync<MOptionAsync<B>, OptionAsync<B>, B>(ma,
                a => default(MOptionAsync<B>).ReturnAsync(f(a)));
    }
}
