using System;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances
{
    public struct ApplTryOption<A, B> : 
        Functor<TryOption<A>, TryOption<B>, A, B>,
        BiFunctor<TryOption<A>, TryOption<B>, A, Unit, B>,
        Applicative<TryOption<Func<A, B>>, TryOption<A>, TryOption<B>, A, B>
    {
        public static readonly ApplTryOption<A, B> Inst = default(ApplTryOption<A, B>);

        [Pure]
        public TryOption<B> BiMap(TryOption<A> ma, Func<A, B> fa, Func<Unit, B> fb) =>
            FTryOption<A, B>.Inst.BiMap(ma, fa, fb);

        [Pure]
        public TryOption<B> Map(TryOption<A> ma, Func<A, B> f) =>
            FTryOption<A, B>.Inst.Map(ma, f);

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

    public struct ApplTryOption<A, B, C> :
        Applicative<TryOption<Func<A, Func<B, C>>>, TryOption<Func<B, C>>, TryOption<A>, TryOption<B>, TryOption<C>, A, B, C>
    {
        public static readonly ApplTryOption<A, B, C> Inst = default(ApplTryOption<A, B, C>);

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

    public struct ApplTryOption<A> :
        Functor<TryOption<A>, TryOption<A>, A, A>,
        BiFunctor<TryOption<A>, TryOption<A>, A, Unit, A>,
        Applicative<TryOption<Func<A, A>>, TryOption<A>, TryOption<A>, A, A>,
        Applicative<TryOption<Func<A, Func<A, A>>>, TryOption<Func<A, A>>, TryOption<A>, TryOption<A>, TryOption<A>, A, A, A>
    {
        public static readonly ApplTryOption<A> Inst = default(ApplTryOption<A>);

        [Pure]
        public TryOption<A> BiMap(TryOption<A> ma, Func<A, A> fa, Func<Unit, A> fb) =>
            FOptional<MTryOption<A>, MTryOption<A>, TryOption<A>, TryOption<A>, A, A>.Inst.BiMap(ma, fa, fb);

        [Pure]
        public TryOption<A> Map(TryOption<A> ma, Func<A, A> f) =>
            FOptional<MTryOption<A>, MTryOption<A>, TryOption<A>, TryOption<A>, A, A>.Inst.Map(ma, f);

        [Pure]
        public TryOption<A> Apply(TryOption<Func<A, A>> fab, TryOption<A> fa) =>
            from f in fab
            from a in fa
            select f(a);

        [Pure]
        public TryOption<A> Pure(A x) =>
            MTryOption<A>.Inst.Return(x);

        [Pure]
        public TryOption<A> Action(TryOption<A> fa, TryOption<A> fb) =>
            from a in fa
            from b in fb
            select b;

        [Pure]
        public TryOption<Func<A, A>> Apply(TryOption<Func<A, Func<A, A>>> fabc, TryOption<A> fa) =>
            from f in fabc
            from a in fa
            select f(a);

        [Pure]
        public TryOption<A> Apply(TryOption<Func<A, Func<A, A>>> fabc, TryOption<A> fa, TryOption<A> fb) =>
            from f in fabc
            from a in fa
            from b in fb
            select f(a)(b);
    }

}
