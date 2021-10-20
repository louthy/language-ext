using System;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances
{
    public struct ApplLst<A, B> :
        Functor<Lst<A>, Lst<B>, A, B>,
        Applicative<Lst<Func<A, B>>, Lst<A>, Lst<B>, A, B>
    {
        public static readonly ApplLst<A, B> Inst = default(ApplLst<A, B>);

        [Pure]
        public Lst<B> Action(Lst<A> fa, Lst<B> fb) =>
            from a in fa
            from b in fb
            select b;

        [Pure]
        public Lst<B> Apply(Lst<Func<A, B>> fab, Lst<A> fa) =>
            from f in fab
            from a in fa
            select f(a);


        [Pure]
        public Lst<B> Map(Lst<A> ma, Func<A, B> f) =>
            FLst<A, B>.Inst.Map(ma, f);

        [Pure]
        public Lst<A> Pure(A x) =>
            List.create(x);
    }

    public struct ApplLst<A, B, C> :
        Applicative<Lst<Func<A, Func<B, C>>>, Lst<Func<B, C>>, Lst<A>, Lst<B>, Lst<C>, A, B, C>
    {
        public static readonly ApplLst<A, B, C> Inst = default(ApplLst<A, B, C>);

        [Pure]
        public Lst<Func<B, C>> Apply(Lst<Func<A, Func<B, C>>> fabc, Lst<A> fa) =>
            from f in fabc
            from a in fa
            select f(a);

        [Pure]
        public Lst<C> Apply(Lst<Func<A, Func<B, C>>> fabc, Lst<A> fa, Lst<B> fb) =>
            from f in fabc
            from a in fa
            from b in fb
            select f(a)(b);

        [Pure]
        public Lst<A> Pure(A x) =>
            List.create(x);
    }

    public struct ApplLst<A> :
        Applicative<Lst<Func<A, A>>, Lst<A>, Lst<A>, A, A>,
        Applicative<Lst<Func<A, Func<A, A>>>, Lst<Func<A, A>>, Lst<A>, Lst<A>, Lst<A>, A, A, A>
    {
        public static readonly ApplLst<A> Inst = default(ApplLst<A>);

        [Pure]
        public Lst<A> Action(Lst<A> fa, Lst<A> fb) =>
            from a in fa
            from b in fb
            select b;

        [Pure]
        public Lst<A> Apply(Lst<Func<A, A>> fab, Lst<A> fa) =>
            from f in fab
            from a in fa
            select f(a);

        [Pure]
        public Lst<Func<A, A>> Apply(Lst<Func<A, Func<A, A>>> fabc, Lst<A> fa) =>
            from f in fabc
            from a in fa
            select f(a);

        [Pure]
        public Lst<A> Apply(Lst<Func<A, Func<A, A>>> fabc, Lst<A> fa, Lst<A> fb) =>
            from f in fabc
            from a in fa
            from b in fb
            select f(a)(b);

        [Pure]
        public Lst<A> Pure(A x) =>
            List.create(x);
    }
}
