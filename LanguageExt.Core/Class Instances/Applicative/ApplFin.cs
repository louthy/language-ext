using System;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using LanguageExt.Common;

namespace LanguageExt.ClassInstances
{
    public struct ApplFin<A, B> : 
        Functor<Fin<A>, Fin<B>, A, B>,
        BiFunctor<Fin<A>, Fin<B>, A, Error, B>,
        Applicative<Fin<Func<A, B>>, Fin<A>, Fin<B>, A, B>
    {
        public static readonly ApplFin<A, B> Inst = default(ApplFin<A, B>);

        [Pure]
        public Fin<B> BiMap(Fin<A> ma, Func<A, B> fa, Func<Error, B> fb) =>
            FFin<A, B>.Inst.BiMap(ma, fa, fb);

        [Pure]
        public Fin<B> Map(Fin<A> ma, Func<A, B> f) =>
            FFin<A, B>.Inst.Map(ma, f);

        [Pure]
        public Fin<B> Apply(Fin<Func<A, B>> fab, Fin<A> fa) =>
            from f in fab
            from a in fa
            select f(a);

        [Pure]
        public Fin<A> Pure(A x) =>
            MFin<A>.Inst.Return(x);

        [Pure]
        public Fin<B> Action(Fin<A> fa, Fin<B> fb) =>
            from a in fa
            from b in fb
            select b;
    }

    public struct ApplFin<A, B, C> :
        Applicative<Fin<Func<A, Func<B, C>>>, Fin<Func<B, C>>, Fin<A>, Fin<B>, Fin<C>, A, B, C>
    {
        public static readonly ApplFin<A, B, C> Inst = default(ApplFin<A, B, C>);

        [Pure]
        public Fin<Func<B, C>> Apply(Fin<Func<A, Func<B, C>>> fabc, Fin<A> fa) =>
            from f in fabc
            from a in fa
            select f(a);

        [Pure]
        public Fin<C> Apply(Fin<Func<A, Func<B, C>>> fabc, Fin<A> fa, Fin<B> fb) =>
            from f in fabc
            from a in fa
            from b in fb
            select f(a)(b);

        [Pure]
        public Fin<A> Pure(A x) =>
            MFin<A>.Inst.Return(x);
    }

    public struct ApplFin<A> : 
        Functor<Fin<A>, Fin<A>, A, A>,
        BiFunctor<Fin<A>, Fin<A>, A, Error, A>,
        Applicative<Fin<Func<A, A>>, Fin<A>, Fin<A>, A, A>,
        Applicative<Fin<Func<A, Func<A, A>>>, Fin<Func<A, A>>, Fin<A>, Fin<A>, Fin<A>, A, A, A>
    {
        public static readonly ApplFin<A> Inst = default(ApplFin<A>);

        [Pure]
        public Fin<A> BiMap(Fin<A> ma, Func<A, A> fa, Func<Error, A> fb) =>
            ma.BiMap(fa, fb);

        [Pure]
        public Fin<A> Map(Fin<A> ma, Func<A, A> f) =>
            ma.Map(f);

        [Pure]
        public Fin<A> Apply(Fin<Func<A, A>> fab, Fin<A> fa) =>
            from f in fab
            from a in fa
            select f(a);

        [Pure]
        public Fin<A> Pure(A x) =>
            MFin<A>.Inst.Return(x);

        [Pure]
        public Fin<A> Action(Fin<A> fa, Fin<A> fb) =>
            from a in fa
            from b in fb
            select b;

        [Pure]
        public Fin<Func<A, A>> Apply(Fin<Func<A, Func<A, A>>> fabc, Fin<A> fa) =>
            from f in fabc
            from a in fa
            select f(a);

        [Pure]
        public Fin<A> Apply(Fin<Func<A, Func<A, A>>> fabc, Fin<A> fa, Fin<A> fb) =>
            from f in fabc
            from a in fa
            from b in fb
            select f(a)(b);
    }
}
