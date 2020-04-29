using System;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using LanguageExt.Common;

namespace LanguageExt.ClassInstances
{
    public struct ApplTryOptionAsync<A, B> : 
        FunctorAsync<TryOptionAsync<A>, TryOptionAsync<B>, A, B>,
        BiFunctorAsync<TryOptionAsync<A>, TryOptionAsync<B>, A, Unit, B>,
        ApplicativeAsync<TryOptionAsync<Func<A, B>>, TryOptionAsync<A>, TryOptionAsync<B>, A, B>
    {
        public static readonly ApplTryOptionAsync<A, B> Inst = default(ApplTryOptionAsync<A, B>);

        [Pure]
        public TryOptionAsync<B> BiMapAsync(TryOptionAsync<A> ma, Func<A, B> fa, Func<Unit, B> fb) =>
            default(FTryOptionAsync<A, B>).BiMapAsync(ma, fa, fb);

        [Pure]
        public TryOptionAsync<B> BiMapAsync(TryOptionAsync<A> ma, Func<A, Task<B>> fa, Func<Unit, B> fb) =>
            default(FTryOptionAsync<A, B>).BiMapAsync(ma, fa, fb);

        [Pure]
        public TryOptionAsync<B> BiMapAsync(TryOptionAsync<A> ma, Func<A, B> fa, Func<Unit, Task<B>> fb) =>
            default(FTryOptionAsync<A, B>).BiMapAsync(ma, fa, fb);

        [Pure]
        public TryOptionAsync<B> BiMapAsync(TryOptionAsync<A> ma, Func<A, Task<B>> fa, Func<Unit, Task<B>> fb) =>
            default(FTryOptionAsync<A, B>).BiMapAsync(ma, fa, fb);

        [Pure]
        public TryOptionAsync<B> Map(TryOptionAsync<A> ma, Func<A, B> f) =>
            default(FTryOptionAsync<A, B>).Map(ma, f);

        [Pure]
        public TryOptionAsync<B> MapAsync(TryOptionAsync<A> ma, Func<A, Task<B>> f) =>
            default(FTryOptionAsync<A, B>).MapAsync(ma, f);

        [Pure]
        public TryOptionAsync<B> Apply(TryOptionAsync<Func<A, B>> fab, TryOptionAsync<A> fa) =>
            async () =>
            {
                var f = fab.Try();
                var a = fa.Try();

                await Task.WhenAll(f, a);

                var rf = await f;
                if (rf.IsFaulted) return new OptionalResult<B>(rf.Exception);
                if (rf.IsFaultedOrNone) return OptionalResult<B>.None;

                var ra = await a;
                if (ra.IsFaulted) return new OptionalResult<B>(ra.Exception);
                if (ra.IsFaultedOrNone) return OptionalResult<B>.None;

                return new OptionalResult<B>(rf.Value.Value(ra.Value.Value));
            };

        [Pure]
        public TryOptionAsync<B> Apply(Func<A, B> fab, TryOptionAsync<A> fa) =>
            async () =>
            {
                var a = fa.Try();

                var ra = await a;
                if (ra.IsFaulted) return new OptionalResult<B>(ra.Exception);
                if (ra.IsFaultedOrNone) return OptionalResult<B>.None;

                return new OptionalResult<B>(fab(ra.Value.Value));
            };

        [Pure]
        public TryOptionAsync<B> Apply(TryOptionAsync<Func<A, A, B>> fab, TryOptionAsync<A> fa, TryOptionAsync<A> fb) =>
            async () =>
            {
                var f = fab.Try();
                var a = fa.Try();
                var b = fb.Try();

                await Task.WhenAll(f, a, b);

                var rf = await f;
                if (rf.IsFaulted) return new OptionalResult<B>(rf.Exception);
                if (rf.IsFaultedOrNone) return OptionalResult<B>.None;

                var ra = await a;
                if (ra.IsFaulted) return new OptionalResult<B>(ra.Exception);
                if (ra.IsFaultedOrNone) return OptionalResult<B>.None;

                var rb = await b;
                if (rb.IsFaulted) return new OptionalResult<B>(rb.Exception);
                if (rb.IsFaultedOrNone) return OptionalResult<B>.None;

                return new OptionalResult<B>(rf.Value.Value(ra.Value.Value, rb.Value.Value));
            };

        [Pure]
        public TryOptionAsync<B> Apply(Func<A, A, B> fab, TryOptionAsync<A> fa, TryOptionAsync<A> fb) =>
            async () =>
            {
                var a = fa.Try();
                var b = fb.Try();

                await Task.WhenAll(a, b);

                var ra = await a;
                if (ra.IsFaulted) return new OptionalResult<B>(ra.Exception);
                if (ra.IsFaultedOrNone) return OptionalResult<B>.None;

                var rb = await b;
                if (rb.IsFaulted) return new OptionalResult<B>(rb.Exception);
                if (rb.IsFaultedOrNone) return OptionalResult<B>.None;

                return new OptionalResult<B>(fab(ra.Value.Value, rb.Value.Value));
            };

        [Pure]
        public TryOptionAsync<B> ApplyOption(Func<Option<A>, Option<A>, B> fab, TryOptionAsync<A> fa, TryOptionAsync<A> fb) =>
            async () =>
            {
                var a = fa.Try();
                var b = fb.Try();

                await Task.WhenAll(a, b);

                var ra = await a;
                if (ra.IsFaulted) return new OptionalResult<B>(ra.Exception);

                var rb = await b;
                if (rb.IsFaulted) return new OptionalResult<B>(rb.Exception);

                return new OptionalResult<B>(fab(ra.Value, rb.Value));
            };

        [Pure]
        public TryOptionAsync<B> ApplyOption(TryOptionAsync<Func<Option<A>, Option<A>, B>> fab, TryOptionAsync<A> fa, TryOptionAsync<A> fb) =>
            async () =>
            {
                var f = fab.Try();
                var a = fa.Try();
                var b = fb.Try();

                await Task.WhenAll(f, a, b);

                var rf = await f;
                if (rf.IsFaulted) return new OptionalResult<B>(rf.Exception);
                if (rf.IsFaultedOrNone) return OptionalResult<B>.None;

                var ra = await a;
                if (ra.IsFaulted) return new OptionalResult<B>(ra.Exception);

                var rb = await b;
                if (rb.IsFaulted) return new OptionalResult<B>(rb.Exception);

                return new OptionalResult<B>(rf.Value.Value(ra.Value, rb.Value));
            };

        [Pure]
        public TryOptionAsync<A> PureAsync(Task<A> x) =>
            MTryOptionAsync<A>.Inst.ReturnAsync(x);

        [Pure]
        public TryOptionAsync<B> Action(TryOptionAsync<A> fa, TryOptionAsync<B> fb) =>
            async () =>
            {
                var a = fa.Try();
                var b = fb.Try();

                await Task.WhenAll(a, b);

                var ra = await a;
                if (ra.IsFaulted) return new OptionalResult<B>(ra.Exception);
                if (ra.IsFaultedOrNone) return OptionalResult<B>.None;

                var rb = await b;
                if (rb.IsFaulted) return new OptionalResult<B>(rb.Exception);
                if (rb.IsFaultedOrNone) return OptionalResult<B>.None;

                return new OptionalResult<B>(rb.Value);
            };
    }

    public struct ApplTryOptionAsync<A, B, C> :
        ApplicativeAsync<TryOptionAsync<Func<A, Func<B, C>>>, TryOptionAsync<Func<B, C>>, TryOptionAsync<A>, TryOptionAsync<B>, TryOptionAsync<C>, A, B, C>
    {
        public static readonly ApplTryOptionAsync<A, B, C> Inst = default(ApplTryOptionAsync<A, B, C>);

        [Pure]
        public TryOptionAsync<Func<B, C>> Apply(TryOptionAsync<Func<A, Func<B, C>>> fabc, TryOptionAsync<A> fa) =>
            async () =>
            {
                var f = fabc.Try();
                var a = fa.Try();

                await Task.WhenAll(f, a);

                var rf = await f;
                if (rf.IsFaulted) return new OptionalResult<Func<B, C>>(rf.Exception);
                if (rf.IsFaultedOrNone) return OptionalResult<Func<B, C>>.None;

                var ra = await a;
                if (ra.IsFaulted) return new OptionalResult<Func<B, C>>(ra.Exception);
                if (ra.IsFaultedOrNone) return OptionalResult<Func<B, C>>.None;

                return new OptionalResult<Func<B, C>>(rf.Value.Value(ra.Value.Value));
            };

        [Pure]
        public TryOptionAsync<C> Apply(TryOptionAsync<Func<A, Func<B, C>>> fabc, TryOptionAsync<A> fa, TryOptionAsync<B> fb) =>
            async () =>
            {
                var f = fabc.Try();
                var a = fa.Try();
                var b = fb.Try();

                await Task.WhenAll(f, a, b);

                var rf = await f;
                if (rf.IsFaulted) return new OptionalResult<C>(rf.Exception);
                if (rf.IsFaultedOrNone) return OptionalResult<C>.None;

                var ra = await a;
                if (ra.IsFaulted) return new OptionalResult<C>(ra.Exception);
                if (ra.IsFaultedOrNone) return OptionalResult<C>.None;

                var rb = await b;
                if (rb.IsFaulted) return new OptionalResult<C>(rb.Exception);
                if (rb.IsFaultedOrNone) return OptionalResult<C>.None;

                return new OptionalResult<C>(rf.Value.Value(ra.Value.Value)(rb.Value.Value));
            };

        [Pure]
        public TryOptionAsync<A> PureAsync(Task<A> x) =>
            MTryOptionAsync<A>.Inst.ReturnAsync(x);
    }


    public struct ApplTryOptionAsync<A> :
        FunctorAsync<TryOptionAsync<A>, TryOptionAsync<A>, A, A>,
        BiFunctorAsync<TryOptionAsync<A>, TryOptionAsync<A>, A, Unit, A>,
        ApplicativeAsync<TryOptionAsync<Func<A, A>>, TryOptionAsync<A>, TryOptionAsync<A>, A, A>,
        ApplicativeAsync<TryOptionAsync<Func<A, Func<A, A>>>, TryOptionAsync<Func<A, A>>, TryOptionAsync<A>, TryOptionAsync<A>, TryOptionAsync<A>, A, A, A>
    {
        public static readonly ApplTryOptionAsync<A> Inst = default(ApplTryOptionAsync<A>);

        [Pure]
        public TryOptionAsync<A> BiMapAsync(TryOptionAsync<A> ma, Func<A, A> fa, Func<Unit, A> fb) =>
            default(FTryOptionAsync<A, A>).BiMapAsync(ma, fa, fb);

        [Pure]
        public TryOptionAsync<A> BiMapAsync(TryOptionAsync<A> ma, Func<A, Task<A>> fa, Func<Unit, A> fb) =>
            default(FTryOptionAsync<A, A>).BiMapAsync(ma, fa, fb);

        [Pure]
        public TryOptionAsync<A> BiMapAsync(TryOptionAsync<A> ma, Func<A, A> fa, Func<Unit, Task<A>> fb) =>
            default(FTryOptionAsync<A, A>).BiMapAsync(ma, fa, fb);

        [Pure]
        public TryOptionAsync<A> BiMapAsync(TryOptionAsync<A> ma, Func<A, Task<A>> fa, Func<Unit, Task<A>> fb) =>
            default(FTryOptionAsync<A, A>).BiMapAsync(ma, fa, fb);

        [Pure]
        public TryOptionAsync<A> Map(TryOptionAsync<A> ma, Func<A, A> f) =>
            default(FTryOptionAsync<A, A>).Map(ma, f);

        [Pure]
        public TryOptionAsync<A> MapAsync(TryOptionAsync<A> ma, Func<A, Task<A>> f) =>
            default(FTryOptionAsync<A, A>).MapAsync(ma, f);

        [Pure]
        public TryOptionAsync<A> Apply(TryOptionAsync<Func<A, A>> fab, TryOptionAsync<A> fa) =>
            async () =>
            {
                var f = fab.Try();
                var a = fa.Try();

                await Task.WhenAll(f, a);

                var rf = await f;
                if (rf.IsFaulted) return new OptionalResult<A>(rf.Exception);
                if (rf.IsFaultedOrNone) return OptionalResult<A>.None;

                var ra = await a;
                if (ra.IsFaulted) return new OptionalResult<A>(ra.Exception);
                if (ra.IsFaultedOrNone) return OptionalResult<A>.None;

                return new OptionalResult<A>(rf.Value.Value(ra.Value.Value));
            };

        [Pure]
        public TryOptionAsync<A> PureAsync(Task<A> x) =>
            MTryOptionAsync<A>.Inst.ReturnAsync(x);

        [Pure]
        public TryOptionAsync<A> Action(TryOptionAsync<A> fa, TryOptionAsync<A> fb) =>
            async () =>
            {
                var a = fa.Try();
                var b = fb.Try();

                await Task.WhenAll(a, b);

                var ra = await a;
                if (ra.IsFaulted) return new OptionalResult<A>(ra.Exception);
                if (ra.IsFaultedOrNone) return OptionalResult<A>.None;

                var rb = await b;
                if (rb.IsFaulted) return new OptionalResult<A>(rb.Exception);
                if (rb.IsFaultedOrNone) return OptionalResult<A>.None;

                return new OptionalResult<A>(rb.Value);
            };

        [Pure]
        public TryOptionAsync<Func<A, A>> Apply(TryOptionAsync<Func<A, Func<A, A>>> fabc, TryOptionAsync<A> fa) =>
            async () =>
            {
                var f = fabc.Try();
                var a = fa.Try();

                await Task.WhenAll(f, a);

                var rf = await f;
                if (rf.IsFaulted) return new OptionalResult<Func<A, A>>(rf.Exception);
                if (rf.IsFaultedOrNone) return OptionalResult<Func<A, A>>.None;

                var ra = await a;
                if (ra.IsFaulted) return new OptionalResult<Func<A, A>>(ra.Exception);
                if (ra.IsFaultedOrNone) return OptionalResult<Func<A, A>>.None;

                return new OptionalResult<Func<A, A>>(rf.Value.Value(ra.Value.Value));
            };

        [Pure]
        public TryOptionAsync<A> Apply(TryOptionAsync<Func<A, Func<A, A>>> fabc, TryOptionAsync<A> fa, TryOptionAsync<A> fb) =>
            async () =>
            {
                var f = fabc.Try();
                var a = fa.Try();
                var b = fb.Try();

                await Task.WhenAll(f, a, b);

                var rf = await f;
                if (rf.IsFaulted) return new OptionalResult<A>(rf.Exception);
                if (rf.IsFaultedOrNone) return OptionalResult<A>.None;

                var ra = await a;
                if (ra.IsFaulted) return new OptionalResult<A>(ra.Exception);
                if (ra.IsFaultedOrNone) return OptionalResult<A>.None;

                var rb = await b;
                if (rb.IsFaulted) return new OptionalResult<A>(rb.Exception);
                if (rb.IsFaultedOrNone) return OptionalResult<A>.None;

                return new OptionalResult<A>(rf.Value.Value(ra.Value.Value)(rb.Value.Value));
            };
    }
}
