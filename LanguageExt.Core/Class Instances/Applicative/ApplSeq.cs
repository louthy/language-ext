using System;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances
{
    public struct ApplSeq<A, B> :
        Functor<Seq<A>, Seq<B>, A, B>,
        Applicative<Seq<Func<A, B>>, Seq<A>, Seq<B>, A, B>
    {
        [Pure]
        public static Seq<B> Action(Seq<A> fa, Seq<B> fb) =>
            from a in fa
            from b in fb
            select b;

        [Pure]
        public static Seq<B> Apply(Seq<Func<A, B>> fab, Seq<A> fa) =>
            from f in fab
            from a in fa
            select f(a);

        [Pure]
        public static Seq<B> Map(Seq<A> ma, Func<A, B> f) =>
            FSeq<A, B>.Map(ma, f);

        [Pure]
        public static Seq<A> Pure(A x) =>
            Seq.create(x);
    }

    public struct ApplSeq<A, B, C> :
        Applicative<Seq<Func<A, Func<B, C>>>, Seq<Func<B, C>>, Seq<A>, Seq<B>, Seq<C>, A, B>
    {
        [Pure]
        public static Seq<Func<B, C>> Apply(Seq<Func<A, Func<B, C>>> fabc, Seq<A> fa) =>
            from f in fabc
            from a in fa
            select f(a);

        [Pure]
        public static Seq<C> Apply(Seq<Func<A, Func<B, C>>> fabc, Seq<A> fa, Seq<B> fb) =>
            from f in fabc
            from a in fa
            from b in fb
            select f(a)(b);

        [Pure]
        public static Seq<A> Pure(A x) =>
            Seq.create(x);
    }

    public struct ApplSeq<A> :
        Applicative<Seq<Func<A, A>>, Seq<A>, Seq<A>, A, A>,
        Applicative<Seq<Func<A, Func<A, A>>>, Seq<Func<A, A>>, Seq<A>, Seq<A>, Seq<A>, A, A>
    {
        [Pure]
        public static Seq<A> Action(Seq<A> fa, Seq<A> fb) =>
            from a in fa
            from b in fb
            select b;

        [Pure]
        public static Seq<A> Apply(Seq<Func<A, A>> fab, Seq<A> fa) =>
            from f in fab
            from a in fa
            select f(a);

        [Pure]
        public static Seq<Func<A, A>> Apply(Seq<Func<A, Func<A, A>>> fabc, Seq<A> fa) =>
            from f in fabc
            from a in fa
            select f(a);

        [Pure]
        public static Seq<A> Apply(Seq<Func<A, Func<A, A>>> fabc, Seq<A> fa, Seq<A> fb) =>
            from f in fabc
            from a in fa
            from b in fb
            select f(a)(b);

        [Pure]
        public static Seq<A> Pure(A x) =>
            Seq.create(x);
    }
}
