using System;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;

namespace LanguageExt.ClassInstances
{
    public struct ApplTryAsync<A, B> : 
        Functor<TryAsync<A>, TryAsync<B>, A, B>,
        BiFunctor<TryAsync<A>, TryAsync<B>, A, Unit, B>,
        Applicative<TryAsync<Func<A, B>>, TryAsync<A>, TryAsync<B>, A, B>
    {
        public static readonly ApplTryAsync<A, B> Inst = default(ApplTryAsync<A, B>);

        [Pure]
        public TryAsync<B> BiMap(TryAsync<A> ma, Func<A, B> fa, Func<Unit, B> fb) =>
            FTryAsync<A, B>.Inst.BiMap(ma, fa, fb);

        [Pure]
        public TryAsync<B> Map(TryAsync<A> ma, Func<A, B> f) =>
            FTryAsync<A, B>.Inst.Map(ma, f);

        [Pure]
        public TryAsync<B> Apply(TryAsync<Func<A, B>> fab, TryAsync<A> fa) =>
            async () =>
            {
                var f = fab.Try();
                var a = fa.Try();

                await Task.WhenAll(f, a);

                if (f.IsFaulted || f.Result.IsFaulted) return new Result<B>(f.Exception);
                if (a.IsFaulted || a.Result.IsFaulted) return new Result<B>(a.Exception);

                return new Result<B>(f.Result.Value(a.Result.Value));
            };

        [Pure]
        public TryAsync<A> Pure(A x) =>
            MTryAsync<A>.Inst.Return(x);

        [Pure]
        public TryAsync<B> Action(TryAsync<A> fa, TryAsync<B> fb) =>
            async () =>
            {
                var a = fa.Try();
                var b = fb.Try();

                await Task.WhenAll(a, b);

                if (a.IsFaulted || a.Result.IsFaulted) return new Result<B>(a.Exception);
                if (b.IsFaulted || b.Result.IsFaulted) return new Result<B>(b.Exception);

                return new Result<B>(b.Result.Value);
            };
    }

    public struct ApplTryAsync<A, B, C> :
        Applicative<TryAsync<Func<A, Func<B, C>>>, TryAsync<Func<B, C>>, TryAsync<A>, TryAsync<B>, TryAsync<C>, A, B, C>
    {
        public static readonly ApplTryAsync<A, B, C> Inst = default(ApplTryAsync<A, B, C>);

        [Pure]
        public TryAsync<Func<B, C>> Apply(TryAsync<Func<A, Func<B, C>>> fabc, TryAsync<A> fa) =>
            async () =>
            {
                var f = fabc.Try();
                var a = fa.Try();

                await Task.WhenAll(f, a);

                if (f.IsFaulted || f.Result.IsFaulted) return new Result<Func<B, C>>(f.Exception);
                if (a.IsFaulted || a.Result.IsFaulted) return new Result<Func<B, C>>(a.Exception);

                return new Result<Func<B, C>>(f.Result.Value(a.Result.Value));
            };

        [Pure]
        public TryAsync<C> Apply(TryAsync<Func<A, Func<B, C>>> fabc, TryAsync<A> fa, TryAsync<B> fb) =>
            async () =>
            {
                var f = fabc.Try();
                var a = fa.Try();
                var b = fb.Try();

                await Task.WhenAll(f, a, b);

                if (f.IsFaulted || f.Result.IsFaulted) return new Result<C>(f.Exception);
                if (a.IsFaulted || a.Result.IsFaulted) return new Result<C>(a.Exception);
                if (b.IsFaulted || b.Result.IsFaulted) return new Result<C>(b.Exception);

                return new Result<C>(f.Result.Value(a.Result.Value)(b.Result.Value));
            };

        [Pure]
        public TryAsync<A> Pure(A x) =>
            MTryAsync<A>.Inst.Return(x);
    }


    public struct ApplTryAsync<A> :
        Functor<TryAsync<A>, TryAsync<A>, A, A>,
        BiFunctor<TryAsync<A>, TryAsync<A>, A, Unit, A>,
        Applicative<TryAsync<Func<A, A>>, TryAsync<A>, TryAsync<A>, A, A>,
        Applicative<TryAsync<Func<A, Func<A, A>>>, TryAsync<Func<A, A>>, TryAsync<A>, TryAsync<A>, TryAsync<A>, A, A, A>
    {
        public static readonly ApplTryAsync<A> Inst = default(ApplTryAsync<A>);

        [Pure]
        public TryAsync<A> BiMap(TryAsync<A> ma, Func<A, A> fa, Func<Unit, A> fb) => () =>
            ma.Match(
                Succ: a => new Result<A>(fa(a)),
                Fail: _ => new Result<A>(fb(unit)));

        [Pure]
        public TryAsync<A> Map(TryAsync<A> ma, Func<A, A> f) => () =>
            ma.Match(
                Succ: a => new Result<A>(f(a)),
                Fail: e => new Result<A>(e));

        [Pure]
        public TryAsync<A> Apply(TryAsync<Func<A, A>> fab, TryAsync<A> fa) =>
            async () =>
            {
                var f = fab.Try();
                var a = fa.Try();

                await Task.WhenAll(f, a);

                if (f.IsFaulted || f.Result.IsFaulted) return new Result<A>(f.Exception);
                if (a.IsFaulted || a.Result.IsFaulted) return new Result<A>(a.Exception);

                return new Result<A>(f.Result.Value(a.Result.Value));
            };

        [Pure]
        public TryAsync<A> Pure(A x) =>
            MTryAsync<A>.Inst.Return(x);

        [Pure]
        public TryAsync<A> Action(TryAsync<A> fa, TryAsync<A> fb) =>
            async () =>
            {
                var a = fa.Try();
                var b = fb.Try();

                await Task.WhenAll(a, b);

                if (a.IsFaulted || a.Result.IsFaulted) return new Result<A>(a.Exception);
                if (b.IsFaulted || b.Result.IsFaulted) return new Result<A>(b.Exception);

                return new Result<A>(b.Result.Value);
            };

        [Pure]
        public TryAsync<Func<A, A>> Apply(TryAsync<Func<A, Func<A, A>>> fabc, TryAsync<A> fa) =>
            async () =>
            {
                var f = fabc.Try();
                var a = fa.Try();

                await Task.WhenAll(f, a);

                if (f.IsFaulted || f.Result.IsFaulted) return new Result<Func<A, A>>(f.Exception);
                if (a.IsFaulted || a.Result.IsFaulted) return new Result<Func<A, A>>(a.Exception);

                return new Result<Func<A, A>>(f.Result.Value(a.Result.Value));
            };

        [Pure]
        public TryAsync<A> Apply(TryAsync<Func<A, Func<A, A>>> fabc, TryAsync<A> fa, TryAsync<A> fb) =>
            async () =>
            {
                var f = fabc.Try();
                var a = fa.Try();
                var b = fb.Try();

                await Task.WhenAll(f, a, b);

                if (f.IsFaulted || f.Result.IsFaulted) return new Result<A>(f.Exception);
                if (a.IsFaulted || a.Result.IsFaulted) return new Result<A>(a.Exception);
                if (b.IsFaulted || b.Result.IsFaulted) return new Result<A>(b.Exception);

                return new Result<A>(f.Result.Value(a.Result.Value)(b.Result.Value));
            };
    }
}
