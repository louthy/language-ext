using System;
using System.Linq;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances
{
    public struct ApplArr<A, B> : 
        Functor<Arr<A>, Arr<B>, A, B>,
        Applicative<Arr<Func<A, B>>, Arr<A>, Arr<B>, A, B>
    {
        public static readonly ApplArr<A, B> Inst = default(ApplArr<A, B>);

        [Pure]
        public Arr<B> Action(Arr<A> fa, Arr<B> fb) =>
            (from a in fa
             from b in fb
             select b).ToArr();

        [Pure]
        public Arr<B> Apply(Arr<Func<A, B>> fab, Arr<A> fa) =>
            (from f in fab
             from a in fa
             select f(a)).ToArr();

        [Pure]
        public Arr<B> Map(Arr<A> ma, Func<A, B> f) =>
            FArr<A, B>.Inst.Map(ma, f);

        [Pure]
        public Arr<A> Pure(A x) =>
            Array(x);
    }

    public struct ApplArr<A, B, C> :
        Applicative<Arr<Func<A, Func<B, C>>>, Arr<Func<B, C>>, Arr<A>, Arr<B>, Arr<C>, A, B, C>
    {
        public static readonly ApplArr<A, B, C> Inst = default(ApplArr<A, B, C>);

        [Pure]
        public Arr<Func<B, C>> Apply(Arr<Func<A, Func<B, C>>> fabc, Arr<A> fa) =>
            (from f in fabc
             from a in fa
             select f(a)).ToArr();

        [Pure]
        public Arr<C> Apply(Arr<Func<A, Func<B, C>>> fabc, Arr<A> fa, Arr<B> fb) =>
            (from f in fabc
             from a in fa
             from b in fb
             select f(a)(b)).ToArr();

        [Pure]
        public Arr<A> Pure(A x) =>
            Array(x);
    }

    public struct ApplArr<A> :
        Applicative<Arr<Func<A, A>>, Arr<A>, Arr<A>, A, A>,
        Applicative<Arr<Func<A, Func<A, A>>>, Arr<Func<A, A>>, Arr<A>, Arr<A>, Arr<A>, A, A, A>
    {
        public static readonly ApplArr<A> Inst = default(ApplArr<A>);

        [Pure]
        public Arr<A> Action(Arr<A> fa, Arr<A> fb) =>
            (from a in fa
             from b in fb
             select b).ToArr();

        [Pure]
        public Arr<A> Apply(Arr<Func<A, A>> fab, Arr<A> fa) =>
            (from f in fab
             from a in fa
             select f(a)).ToArr();

        [Pure]
        public Arr<Func<A, A>> Apply(Arr<Func<A, Func<A, A>>> fabc, Arr<A> fa) =>
            (from f in fabc
             from a in fa
             select f(a)).ToArr();

        [Pure]
        public Arr<A> Apply(Arr<Func<A, Func<A, A>>> fabc, Arr<A> fa, Arr<A> fb) =>
            (from f in fabc
             from a in fa
             from b in fb
             select f(a)(b)).ToArr();

        [Pure]
        public Arr<A> Pure(A x) =>
            Array(x);
    }
}
