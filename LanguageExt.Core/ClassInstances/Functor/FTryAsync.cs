using System;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances
{
    public struct FTryAsync<A, B> : 
        Functor<TryAsync<A>, TryAsync<B>, A, B>,
        BiFunctor<TryAsync<A>, TryAsync<B>, Unit, A, B>,
        Applicative<TryAsync<Func<A, B>>, TryAsync<A>, TryAsync<B>, A, B>
    {
        public static readonly FTryAsync<A, B> Inst = default(FTryAsync<A, B>);

        [Pure]
        public TryAsync<B> BiMap(TryAsync<A> ma, Func<Unit, B> fa, Func<A, B> fb) =>
            FOptional<MTryAsync<A>, MTryAsync<B>, TryAsync<A>, TryAsync<B>, A, B>.Inst.BiMap(ma, fa, fb);

        [Pure]
        public TryAsync<B> Map(TryAsync<A> ma, Func<A, B> f) =>
            FOptional<MTryAsync<A>, MTryAsync<B>, TryAsync<A>, TryAsync<B>, A, B>.Inst.Map(ma, f);

        [Pure]
        public TryAsync<B> Apply(TryAsync<Func<A, B>> fab, TryAsync<A> fa) =>
            from f in fab
            from a in fa
            select f(a);

        [Pure]
        public TryAsync<A> Pure(A x) =>
            MTryAsync<A>.Inst.Return(x);

        [Pure]
        public TryAsync<B> Action(TryAsync<A> fa, TryAsync<B> fb) =>
            from a in fa
            from b in fb
            select b;
    }

    public struct FTryAsync<A, B, C> :
        Applicative<TryAsync<Func<A, Func<B, C>>>, TryAsync<Func<B, C>>, TryAsync<A>, TryAsync<B>, TryAsync<C>, A, B, C>
    {
        public static readonly FTryAsync<A, B, C> Inst = default(FTryAsync<A, B, C>);

        [Pure]
        public TryAsync<Func<B, C>> Apply(TryAsync<Func<A, Func<B, C>>> fabc, TryAsync<A> fa) =>
            from f in fabc
            from a in fa
            select f(a);

        [Pure]
        public TryAsync<C> Apply(TryAsync<Func<A, Func<B, C>>> fabc, TryAsync<A> fa, TryAsync<B> fb) =>
            from f in fabc
            from a in fa
            from b in fb
            select f(a)(b);

        [Pure]
        public TryAsync<A> Pure(A x) =>
            MTryAsync<A>.Inst.Return(x);
    }
}
