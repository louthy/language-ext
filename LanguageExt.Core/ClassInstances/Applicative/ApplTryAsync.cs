using System;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using LanguageExt.Common;

namespace LanguageExt.ClassInstances
{
    public struct ApplTryAsync<A, B> : 
        FunctorAsync<TryAsync<A>, TryAsync<B>, A, B>,
        BiFunctorAsync<TryAsync<A>, TryAsync<B>, A, Unit, B>,
        ApplicativeAsync<TryAsync<Func<A, B>>, TryAsync<A>, TryAsync<B>, A, B>
    {
        public static readonly ApplTryAsync<A, B> Inst = default(ApplTryAsync<A, B>);

        [Pure]
        public TryAsync<B> BiMapAsync(TryAsync<A> ma, Func<A, B> fa, Func<Unit, B> fb) =>
            default(FTryAsync<A, B>).BiMapAsync(ma, fa, fb);

        [Pure]
        public TryAsync<B> BiMapAsync(TryAsync<A> ma, Func<A, B> fa, Func<Unit, Task<B>> fb) =>
            default(FTryAsync<A, B>).BiMapAsync(ma, fa, fb);

        [Pure]
        public TryAsync<B> BiMapAsync(TryAsync<A> ma, Func<A, Task<B>> fa, Func<Unit, B> fb) =>
            default(FTryAsync<A, B>).BiMapAsync(ma, fa, fb);

        [Pure]
        public TryAsync<B> BiMapAsync(TryAsync<A> ma, Func<A, Task<B>> fa, Func<Unit, Task<B>> fb) =>
            default(FTryAsync<A, B>).BiMapAsync(ma, fa, fb);

        [Pure]
        public TryAsync<B> Map(TryAsync<A> ma, Func<A, B> f) =>
            default(FTryAsync<A, B>).Map(ma, f);

        [Pure]
        public TryAsync<B> MapAsync(TryAsync<A> ma, Func<A, Task<B>> f) =>
            default(FTryAsync<A, B>).MapAsync(ma, f);

        [Pure]
        public TryAsync<B> Apply(TryAsync<Func<A, B>> fab, TryAsync<A> fa) =>
            async () =>
            {
                var f = fab.Try();
                var a = fa.Try();

                await Task.WhenAll(f, a).ConfigureAwait(false);

                if (f.IsFaulted) return new Result<B>(f.Exception);
                if (a.IsFaulted) return new Result<B>(a.Exception);

                var rf = await f.ConfigureAwait(false);
                if (rf.IsFaulted) return new Result<B>(rf.Exception);

                var ra = await a.ConfigureAwait(false);
                if (ra.IsFaulted) return new Result<B>(ra.Exception);

                return new Result<B>(rf.Value(ra.Value));
            };

        [Pure]
        public TryAsync<A> PureAsync(Task<A> x) =>
            MTryAsync<A>.Inst.ReturnAsync(x);

        [Pure]
        public TryAsync<B> Action(TryAsync<A> fa, TryAsync<B> fb) =>
            async () =>
            {
                var a = fa.Try();
                var b = fb.Try();

                await Task.WhenAll(a, b).ConfigureAwait(false);

                if (a.IsFaulted) return new Result<B>(a.Exception);
                if (b.IsFaulted) return new Result<B>(b.Exception);

                var ra = await a.ConfigureAwait(false);
                if (ra.IsFaulted) return new Result<B>(ra.Exception);

                var rb = await b.ConfigureAwait(false);
                if (rb.IsFaulted) return new Result<B>(rb.Exception);

                return new Result<B>(rb.Value);
            };
    }

    public struct ApplTryAsync<A, B, C> :
        ApplicativeAsync<TryAsync<Func<A, Func<B, C>>>, TryAsync<Func<B, C>>, TryAsync<A>, TryAsync<B>, TryAsync<C>, A, B, C>
    {
        public static readonly ApplTryAsync<A, B, C> Inst = default(ApplTryAsync<A, B, C>);

        [Pure]
        public TryAsync<Func<B, C>> Apply(TryAsync<Func<A, Func<B, C>>> fabc, TryAsync<A> fa) =>
            async () =>
            {
                var f = fabc.Try();
                var a = fa.Try();

                await Task.WhenAll(f, a).ConfigureAwait(false);

                if (f.IsFaulted) return new Result<Func<B, C>>(f.Exception);
                if (a.IsFaulted) return new Result<Func<B, C>>(a.Exception);

                var rf = await f.ConfigureAwait(false);
                if (rf.IsFaulted) return new Result<Func<B, C>>(rf.Exception);

                var ra = await a.ConfigureAwait(false);
                if (ra.IsFaulted) return new Result<Func<B, C>>(ra.Exception);

                return new Result<Func<B, C>>(rf.Value(ra.Value));
            };

        [Pure]
        public TryAsync<C> Apply(TryAsync<Func<A, Func<B, C>>> fabc, TryAsync<A> fa, TryAsync<B> fb) =>
            async () =>
            {
                var f = fabc.Try();
                var a = fa.Try();
                var b = fb.Try();

                await Task.WhenAll(f, a, b).ConfigureAwait(false);

                if (f.IsFaulted) return new Result<C>(f.Exception);
                if (a.IsFaulted) return new Result<C>(a.Exception);
                if (b.IsFaulted) return new Result<C>(b.Exception);

                var rf = await f.ConfigureAwait(false);
                if (rf.IsFaulted) return new Result<C>(rf.Exception);

                var ra = await a.ConfigureAwait(false);
                if (ra.IsFaulted) return new Result<C>(ra.Exception);

                var rb = await b.ConfigureAwait(false);
                if (rb.IsFaulted) return new Result<C>(rb.Exception);

                return new Result<C>(rf.Value(ra.Value)(rb.Value));
            };

        [Pure]
        public TryAsync<A> PureAsync(Task<A> x) =>
            MTryAsync<A>.Inst.ReturnAsync(x);
    }


    public struct ApplTryAsync<A> :
        FunctorAsync<TryAsync<A>, TryAsync<A>, A, A>,
        BiFunctorAsync<TryAsync<A>, TryAsync<A>, A, Unit, A>,
        ApplicativeAsync<TryAsync<Func<A, A>>, TryAsync<A>, TryAsync<A>, A, A>,
        ApplicativeAsync<TryAsync<Func<A, Func<A, A>>>, TryAsync<Func<A, A>>, TryAsync<A>, TryAsync<A>, TryAsync<A>, A, A, A>
    {
        public static readonly ApplTryAsync<A> Inst = default(ApplTryAsync<A>);

        [Pure]
        public TryAsync<A> BiMapAsync(TryAsync<A> ma, Func<A, A> fa, Func<Unit, A> fb) => () =>
            ma.Match(
                Succ: a => new Result<A>(fa(a)),
                Fail: _ => new Result<A>(fb(unit)));

        [Pure]
        public TryAsync<A> BiMapAsync(TryAsync<A> ma, Func<A, Task<A>> fa, Func<Unit, A> fb) => () =>
            ma.Match(
                Succ: async a => new Result<A>(await fa(a).ConfigureAwait(false)),
                Fail: _ => new Result<A>(fb(unit)));

        [Pure]
        public TryAsync<A> BiMapAsync(TryAsync<A> ma, Func<A, A> fa, Func<Unit, Task<A>> fb) => () =>
            ma.Match(
                Succ: a => new Result<A>(fa(a)),
                Fail: async _ => new Result<A>(await fb(unit).ConfigureAwait(false)));

        [Pure]
        public TryAsync<A> BiMapAsync(TryAsync<A> ma, Func<A, Task<A>> fa, Func<Unit, Task<A>> fb) => () =>
            ma.Match(
                Succ: async a => new Result<A>(await fa(a).ConfigureAwait(false)),
                Fail: async _ => new Result<A>(await fb(unit).ConfigureAwait(false)));

        [Pure]
        public TryAsync<A> Map(TryAsync<A> ma, Func<A, A> f) => () =>
            ma.Match(
                Succ: a => new Result<A>(f(a)),
                Fail: e => new Result<A>(e));

        [Pure]
        public TryAsync<A> MapAsync(TryAsync<A> ma, Func<A, Task<A>> f) => () =>
            ma.Match(
                Succ: async a => new Result<A>(await f(a).ConfigureAwait(false)),
                Fail: e => new Result<A>(e));

        [Pure]
        public TryAsync<A> Apply(TryAsync<Func<A, A>> fab, TryAsync<A> fa) =>
            async () =>
            {
                var f = fab.Try();
                var a = fa.Try();

                await Task.WhenAll(f, a).ConfigureAwait(false);

                if (f.IsFaulted) return new Result<A>(f.Exception);
                if (a.IsFaulted) return new Result<A>(a.Exception);

                var rf = await f.ConfigureAwait(false);
                if (rf.IsFaulted) return new Result<A>(rf.Exception);

                var ra = await a.ConfigureAwait(false);
                if (ra.IsFaulted) return new Result<A>(ra.Exception);

                return new Result<A>(rf.Value(ra.Value));
            };

        [Pure]
        public TryAsync<A> PureAsync(Task<A> x) =>
            MTryAsync<A>.Inst.ReturnAsync(x);

        [Pure]
        public TryAsync<A> Action(TryAsync<A> fa, TryAsync<A> fb) =>
            async () =>
            {
                var a = fa.Try();
                var b = fb.Try();

                await Task.WhenAll(a, b).ConfigureAwait(false);

                if (a.IsFaulted) return new Result<A>(a.Exception);
                if (b.IsFaulted) return new Result<A>(b.Exception);

                var ra = await a.ConfigureAwait(false);
                if (ra.IsFaulted) return new Result<A>(ra.Exception);

                var rb = await b.ConfigureAwait(false);
                if (rb.IsFaulted) return new Result<A>(rb.Exception);

                return new Result<A>(rb.Value);
            };

        [Pure]
        public TryAsync<Func<A, A>> Apply(TryAsync<Func<A, Func<A, A>>> fabc, TryAsync<A> fa) =>
            async () =>
            {
                var f = fabc.Try();
                var a = fa.Try();

                await Task.WhenAll(f, a).ConfigureAwait(false);

                if (f.IsFaulted) return new Result<Func<A, A>>(f.Exception);
                if (a.IsFaulted) return new Result<Func<A, A>>(a.Exception);

                var rf = await f.ConfigureAwait(false);
                if (rf.IsFaulted) return new Result<Func<A, A>>(rf.Exception);

                var ra = await a.ConfigureAwait(false);
                if (ra.IsFaulted) return new Result<Func<A, A>>(ra.Exception);

                return new Result<Func<A, A>>(rf.Value(ra.Value));
            };

        [Pure]
        public TryAsync<A> Apply(TryAsync<Func<A, Func<A, A>>> fabc, TryAsync<A> fa, TryAsync<A> fb) =>
            async () =>
            {
                var f = fabc.Try();
                var a = fa.Try();
                var b = fb.Try();

                await Task.WhenAll(f, a, b).ConfigureAwait(false);

                if (f.IsFaulted) return new Result<A>(f.Exception);
                if (a.IsFaulted) return new Result<A>(a.Exception);
                if (b.IsFaulted) return new Result<A>(b.Exception);

                var rf = await f.ConfigureAwait(false);
                if (rf.IsFaulted) return new Result<A>(rf.Exception);

                var ra = await a.ConfigureAwait(false);
                if (ra.IsFaulted) return new Result<A>(ra.Exception);

                var rb = await b.ConfigureAwait(false);
                if (rb.IsFaulted) return new Result<A>(rb.Exception);

                return new Result<A>(rf.Value(ra.Value)(rb.Value));
            };
    }
}
