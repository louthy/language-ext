using System;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;

namespace LanguageExt.ClassInstances
{
    public struct ApplTryOptionAsync<A, B> : 
        Functor<TryOptionAsync<A>, TryOptionAsync<B>, A, B>,
        BiFunctor<TryOptionAsync<A>, TryOptionAsync<B>, A, Unit, B>,
        Applicative<TryOptionAsync<Func<A, B>>, TryOptionAsync<A>, TryOptionAsync<B>, A, B>
    {
        public static readonly ApplTryOptionAsync<A, B> Inst = default(ApplTryOptionAsync<A, B>);

        [Pure]
        public TryOptionAsync<B> BiMap(TryOptionAsync<A> ma, Func<A, B> fa, Func<Unit, B> fb) =>
            FTryOptionAsync<A, B>.Inst.BiMap(ma, fa, fb);

        [Pure]
        public TryOptionAsync<B> Map(TryOptionAsync<A> ma, Func<A, B> f) =>
            FTryOptionAsync<A, B>.Inst.Map(ma, f);

        [Pure]
        public TryOptionAsync<B> Apply(TryOptionAsync<Func<A, B>> fab, TryOptionAsync<A> fa) =>
            async () =>
            {
                var f = fab.Try();
                var a = fa.Try();

                await Task.WhenAll(f, a);

                if (f.IsFaulted || f.Result.IsFaultedOrNone) return new OptionalResult<B>(f.Exception);
                if (a.IsFaulted || a.Result.IsFaultedOrNone) return new OptionalResult<B>(a.Exception);

                return new OptionalResult<B>(f.Result.Value.Value(a.Result.Value.Value));
            };

        [Pure]
        public TryOptionAsync<A> Pure(A x) =>
            MTryOptionAsync<A>.Inst.Return(x);

        [Pure]
        public TryOptionAsync<B> Action(TryOptionAsync<A> fa, TryOptionAsync<B> fb) =>
            async () =>
            {
                var a = fa.Try();
                var b = fb.Try();

                await Task.WhenAll(a, b);

                if (a.IsFaulted || a.Result.IsFaultedOrNone) return new OptionalResult<B>(a.Exception);
                if (b.IsFaulted || b.Result.IsFaultedOrNone) return new OptionalResult<B>(b.Exception);

                return new OptionalResult<B>(b.Result.Value);
            };
    }

    public struct ApplTryOptionAsync<A, B, C> :
        Applicative<TryOptionAsync<Func<A, Func<B, C>>>, TryOptionAsync<Func<B, C>>, TryOptionAsync<A>, TryOptionAsync<B>, TryOptionAsync<C>, A, B, C>
    {
        public static readonly ApplTryOptionAsync<A, B, C> Inst = default(ApplTryOptionAsync<A, B, C>);

        [Pure]
        public TryOptionAsync<Func<B, C>> Apply(TryOptionAsync<Func<A, Func<B, C>>> fabc, TryOptionAsync<A> fa) =>
            async () =>
            {
                var f = fabc.Try();
                var a = fa.Try();

                await Task.WhenAll(f, a);

                if (f.IsFaulted || f.Result.IsFaultedOrNone) return new OptionalResult<Func<B, C>>(f.Exception);
                if (a.IsFaulted || a.Result.IsFaultedOrNone) return new OptionalResult<Func<B, C>>(a.Exception);

                return new OptionalResult<Func<B, C>>(f.Result.Value.Value(a.Result.Value.Value));
            };

        [Pure]
        public TryOptionAsync<C> Apply(TryOptionAsync<Func<A, Func<B, C>>> fabc, TryOptionAsync<A> fa, TryOptionAsync<B> fb) =>
            async () =>
            {
                var f = fabc.Try();
                var a = fa.Try();
                var b = fb.Try();

                await Task.WhenAll(f, a, b);

                if (f.IsFaulted || f.Result.IsFaultedOrNone) return new OptionalResult<C>(f.Exception);
                if (a.IsFaulted || a.Result.IsFaultedOrNone) return new OptionalResult<C>(a.Exception);
                if (b.IsFaulted || b.Result.IsFaultedOrNone) return new OptionalResult<C>(b.Exception);

                return new OptionalResult<C>(f.Result.Value.Value(a.Result.Value.Value)(b.Result.Value.Value));
            };

        [Pure]
        public TryOptionAsync<A> Pure(A x) =>
            MTryOptionAsync<A>.Inst.Return(x);
    }


    public struct ApplTryOptionAsync<A> :
        Functor<TryOptionAsync<A>, TryOptionAsync<A>, A, A>,
        BiFunctor<TryOptionAsync<A>, TryOptionAsync<A>, A, Unit, A>,
        Applicative<TryOptionAsync<Func<A, A>>, TryOptionAsync<A>, TryOptionAsync<A>, A, A>,
        Applicative<TryOptionAsync<Func<A, Func<A, A>>>, TryOptionAsync<Func<A, A>>, TryOptionAsync<A>, TryOptionAsync<A>, TryOptionAsync<A>, A, A, A>
    {
        public static readonly ApplTryOptionAsync<A> Inst = default(ApplTryOptionAsync<A>);

        [Pure]
        public TryOptionAsync<A> BiMap(TryOptionAsync<A> ma, Func<A, A> fa, Func<Unit, A> fb) =>
            FOptional<MTryOptionAsync<A>, MTryOptionAsync<A>, TryOptionAsync<A>, TryOptionAsync<A>, A, A>.Inst.BiMap(ma, fa, fb);

        [Pure]
        public TryOptionAsync<A> Map(TryOptionAsync<A> ma, Func<A, A> f) =>
            FOptional<MTryOptionAsync<A>, MTryOptionAsync<A>, TryOptionAsync<A>, TryOptionAsync<A>, A, A>.Inst.Map(ma, f);

        [Pure]
        public TryOptionAsync<A> Apply(TryOptionAsync<Func<A, A>> fab, TryOptionAsync<A> fa) =>
            async () =>
            {
                var f = fab.Try();
                var a = fa.Try();

                await Task.WhenAll(f, a);

                if (f.IsFaulted || f.Result.IsFaultedOrNone) return new OptionalResult<A>(f.Exception);
                if (a.IsFaulted || a.Result.IsFaultedOrNone) return new OptionalResult<A>(a.Exception);

                return new OptionalResult<A>(f.Result.Value.Value(a.Result.Value.Value));
            };

        [Pure]
        public TryOptionAsync<A> Pure(A x) =>
            MTryOptionAsync<A>.Inst.Return(x);

        [Pure]
        public TryOptionAsync<A> Action(TryOptionAsync<A> fa, TryOptionAsync<A> fb) =>
            async () =>
            {
                var a = fa.Try();
                var b = fb.Try();

                await Task.WhenAll(a, b);

                if (a.IsFaulted || a.Result.IsFaultedOrNone) return new OptionalResult<A>(a.Exception);
                if (b.IsFaulted || b.Result.IsFaultedOrNone) return new OptionalResult<A>(b.Exception);

                return new OptionalResult<A>(b.Result.Value);
            };

        [Pure]
        public TryOptionAsync<Func<A, A>> Apply(TryOptionAsync<Func<A, Func<A, A>>> fabc, TryOptionAsync<A> fa) =>
            async () =>
            {
                var f = fabc.Try();
                var a = fa.Try();

                await Task.WhenAll(f, a);

                if (f.IsFaulted || f.Result.IsFaultedOrNone) return new OptionalResult<Func<A, A>>(f.Exception);
                if (a.IsFaulted || a.Result.IsFaultedOrNone) return new OptionalResult<Func<A, A>>(a.Exception);

                return new OptionalResult<Func<A, A>>(f.Result.Value.Value(a.Result.Value.Value));
            };

        [Pure]
        public TryOptionAsync<A> Apply(TryOptionAsync<Func<A, Func<A, A>>> fabc, TryOptionAsync<A> fa, TryOptionAsync<A> fb) =>
            async () =>
            {
                var f = fabc.Try();
                var a = fa.Try();
                var b = fb.Try();

                await Task.WhenAll(f, a, b);

                if (f.IsFaulted || f.Result.IsFaultedOrNone) return new OptionalResult<A>(f.Exception);
                if (a.IsFaulted || a.Result.IsFaultedOrNone) return new OptionalResult<A>(a.Exception);
                if (b.IsFaulted || b.Result.IsFaultedOrNone) return new OptionalResult<A>(b.Exception);

                return new OptionalResult<A>(f.Result.Value.Value(a.Result.Value.Value)(b.Result.Value.Value));
            };
    }
}
