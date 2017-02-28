using System;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances
{
    public struct FTryOption<A, B> : 
        Functor<TryOption<A>, TryOption<B>, A, B>,
        BiFunctor<TryOption<A>, TryOption<B>, Unit, A, B>,
        Applicative<TryOption<Func<A, B>>, TryOption<A>, TryOption<B>, A, B>
    {
        public static readonly FTryOption<A, B> Inst = default(FTryOption<A, B>);

        [Pure]
        public TryOption<B> BiMap(TryOption<A> ma, Func<Unit, B> fa, Func<A, B> fb) =>
            FOptional<MTryOption<A>, MTryOption<B>, TryOption<A>, TryOption<B>, A, B>.Inst.BiMap(ma, fa, fb);

        [Pure]
        public TryOption<B> Map(TryOption<A> ma, Func<A, B> f) =>
            FOptional<MTryOption<A>, MTryOption<B>, TryOption<A>, TryOption<B>, A, B>.Inst.Map(ma, f);

        [Pure]
        public TryOption<B> Apply(TryOption<Func<A, B>> fab, TryOption<A> fa) =>
            from f in fab
            from a in fa
            select f(a);

        [Pure]
        public TryOption<A> Pure(A x) =>
            MTryOption<A>.Inst.Return(x);

        [Pure]
        public TryOption<B> Action(TryOption<A> fa, TryOption<B> fb) =>
            from a in fa
            from b in fb
            select b;
    }

    public struct FTryOption<A, B, C> :
        Applicative<TryOption<Func<A, Func<B, C>>>, TryOption<Func<B, C>>, TryOption<A>, TryOption<B>, TryOption<C>, A, B, C>
    {
        public static readonly FTryOption<A, B, C> Inst = default(FTryOption<A, B, C>);

        [Pure]
        public TryOption<Func<B, C>> Apply(TryOption<Func<A, Func<B, C>>> fabc, TryOption<A> fa) =>
            from f in fabc
            from a in fa
            select f(a);

        [Pure]
        public TryOption<C> Apply(TryOption<Func<A, Func<B, C>>> fabc, TryOption<A> fa, TryOption<B> fb) =>
            from f in fabc
            from a in fa
            from b in fb
            select f(a)(b);

        [Pure]
        public TryOption<A> Pure(A x) =>
            MTryOption<A>.Inst.Return(x);
    }
}
