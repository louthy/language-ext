using System;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances
{
    public struct FLst<A, B> :
        Functor<Lst<A>, Lst<B>, A, B>,
        Applicative<Lst<Func<A, B>>, Lst<A>, Lst<B>, A, B>
    {
        public static readonly FLst<A, B> Inst = default(FLst<A, B>);

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
            ma.Map(f);

        [Pure]
        public Lst<A> Pure(A x) =>
            List.create(x);
    }

    public struct FLst<A, B, C> :
        Applicative<Lst<Func<A, Func<B, C>>>, Lst<Func<B, C>>, Lst<A>, Lst<B>, Lst<C>, A, B, C>
    {
        public static readonly FLst<A, B, C> Inst = default(FLst<A, B, C>);

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

    public struct FLst<A> :
        Applicative<Lst<Func<A, A>>, Lst<A>, Lst<A>, A, A>,
        Applicative<Lst<Func<A, Func<A, A>>>, Lst<Func<A, A>>, Lst<A>, Lst<A>, Lst<A>, A, A, A>
    {
        public static readonly FLst<A> Inst = default(FLst<A>);

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
