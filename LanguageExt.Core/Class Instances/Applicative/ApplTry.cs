using System;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances
{
    public struct ApplTry<A, B> : 
        Functor<Try<A>, Try<B>, A, B>,
        BiFunctor<Try<A>, Try<B>, A, Unit, B>,
        Applicative<Try<Func<A, B>>, Try<A>, Try<B>, A, B>
    {
        public static readonly ApplTry<A, B> Inst = default(ApplTry<A, B>);

        [Pure]
        public Try<B> BiMap(Try<A> ma, Func<A, B> fa, Func<Unit, B> fb) =>
            FTry<A, B>.Inst.BiMap(ma, fa, fb);

        [Pure]
        public Try<B> Map(Try<A> ma, Func<A, B> f) =>
            FTry<A, B>.Inst.Map(ma, f);

        [Pure]
        public Try<B> Apply(Try<Func<A, B>> fab, Try<A> fa) =>
            from f in fab
            from a in fa
            select f(a);

        [Pure]
        public Try<A> Pure(A x) =>
            MTry<A>.Inst.Return(x);

        [Pure]
        public Try<B> Action(Try<A> fa, Try<B> fb) =>
            from a in fa
            from b in fb
            select b;
    }

    public struct ApplTry<A, B, C> :
        Applicative<Try<Func<A, Func<B, C>>>, Try<Func<B, C>>, Try<A>, Try<B>, Try<C>, A, B, C>
    {
        public static readonly ApplTry<A, B, C> Inst = default(ApplTry<A, B, C>);

        [Pure]
        public Try<Func<B, C>> Apply(Try<Func<A, Func<B, C>>> fabc, Try<A> fa) =>
            from f in fabc
            from a in fa
            select f(a);

        [Pure]
        public Try<C> Apply(Try<Func<A, Func<B, C>>> fabc, Try<A> fa, Try<B> fb) =>
            from f in fabc
            from a in fa
            from b in fb
            select f(a)(b);

        [Pure]
        public Try<A> Pure(A x) =>
            MTry<A>.Inst.Return(x);
    }

    public struct ApplTry<A> : 
        Functor<Try<A>, Try<A>, A, A>,
        BiFunctor<Try<A>, Try<A>, A, Unit, A>,
        Applicative<Try<Func<A, A>>, Try<A>, Try<A>, A, A>,
        Applicative<Try<Func<A, Func<A, A>>>, Try<Func<A, A>>, Try<A>, Try<A>, Try<A>, A, A, A>
    {
        public static readonly ApplTry<A> Inst = default(ApplTry<A>);

        [Pure]
        public Try<A> BiMap(Try<A> ma, Func<A, A> fa, Func<Unit, A> fb) =>
            FOptional<MTry<A>, MTry<A>, Try<A>, Try<A>, A, A>.Inst.BiMap(ma, fa, fb);

        [Pure]
        public Try<A> Map(Try<A> ma, Func<A, A> f) =>
            FOptional<MTry<A>, MTry<A>, Try<A>, Try<A>, A, A>.Inst.Map(ma, f);

        [Pure]
        public Try<A> Apply(Try<Func<A, A>> fab, Try<A> fa) =>
            from f in fab
            from a in fa
            select f(a);

        [Pure]
        public Try<A> Pure(A x) =>
            MTry<A>.Inst.Return(x);

        [Pure]
        public Try<A> Action(Try<A> fa, Try<A> fb) =>
            from a in fa
            from b in fb
            select b;

        [Pure]
        public Try<Func<A, A>> Apply(Try<Func<A, Func<A, A>>> fabc, Try<A> fa) =>
            from f in fabc
            from a in fa
            select f(a);

        [Pure]
        public Try<A> Apply(Try<Func<A, Func<A, A>>> fabc, Try<A> fa, Try<A> fb) =>
            from f in fabc
            from a in fa
            from b in fb
            select f(a)(b);
    }


}
