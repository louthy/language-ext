using System;
using System.Linq;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances
{
    public struct ApplSeq<A, B> :
        Functor<Seq<A>, Seq<B>, A, B>,
        Applicative<Seq<Func<A, B>>, Seq<A>, Seq<B>, A, B>
    {
        public static readonly ApplSeq<A, B> Inst = default(ApplSeq<A, B>);

        [Pure]
        public Seq<B> Action(Seq<A> fa, Seq<B> fb) =>
            from a in fa
            from b in fb
            select b;

        [Pure]
        public Seq<B> Apply(Seq<Func<A, B>> fab, Seq<A> fa) =>
            from f in fab
            from a in fa
            select f(a);

        [Pure]
        public Seq<B> Map(Seq<A> ma, Func<A, B> f) =>
            FSeq<A, B>.Inst.Map(ma, f);

        [Pure]
        public Seq<A> Pure(A x) =>
            Seq.create(x);
    }

    public struct ApplSeq<A, B, C> :
        Applicative<Seq<Func<A, Func<B, C>>>, Seq<Func<B, C>>, Seq<A>, Seq<B>, Seq<C>, A, B, C>
    {
        public static readonly ApplSeq<A, B, C> Inst = default(ApplSeq<A, B, C>);

        [Pure]
        public Seq<Func<B, C>> Apply(Seq<Func<A, Func<B, C>>> fabc, Seq<A> fa) =>
            from f in fabc
            from a in fa
            select f(a);

        [Pure]
        public Seq<C> Apply(Seq<Func<A, Func<B, C>>> fabc, Seq<A> fa, Seq<B> fb) =>
            from f in fabc
            from a in fa
            from b in fb
            select f(a)(b);

        [Pure]
        public Seq<A> Pure(A x) =>
            Seq.create(x);
    }

    public struct ApplSeq<A> :
        Applicative<Seq<Func<A, A>>, Seq<A>, Seq<A>, A, A>,
        Applicative<Seq<Func<A, Func<A, A>>>, Seq<Func<A, A>>, Seq<A>, Seq<A>, Seq<A>, A, A, A>
    {
        public static readonly ApplSeq<A> Inst = default(ApplSeq<A>);

        [Pure]
        public Seq<A> Action(Seq<A> fa, Seq<A> fb) =>
            from a in fa
            from b in fb
            select b;

        [Pure]
        public Seq<A> Apply(Seq<Func<A, A>> fab, Seq<A> fa) =>
            from f in fab
            from a in fa
            select f(a);

        [Pure]
        public Seq<Func<A, A>> Apply(Seq<Func<A, Func<A, A>>> fabc, Seq<A> fa) =>
            from f in fabc
            from a in fa
            select f(a);

        [Pure]
        public Seq<A> Apply(Seq<Func<A, Func<A, A>>> fabc, Seq<A> fa, Seq<A> fb) =>
            from f in fabc
            from a in fa
            from b in fb
            select f(a)(b);

        [Pure]
        public Seq<A> Pure(A x) =>
            Seq.create(x);
    }
}
