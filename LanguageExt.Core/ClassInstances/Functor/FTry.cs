using System;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances
{
    public struct FTry<A, B> : 
        Functor<Try<A>, Try<B>, A, B>,
        BiFunctor<Try<A>, Try<B>, Unit, A, B>,
        Applicative<Try<Func<A, B>>, Try<A>, Try<B>, A, B>
    {
        public static readonly FTry<A, B> Inst = default(FTry<A, B>);

        [Pure]
        public Try<B> BiMap(Try<A> ma, Func<Unit, B> fa, Func<A, B> fb) =>
            FOptional<MTry<A>, MTry<B>, Try<A>, Try<B>, A, B>.Inst.BiMap(ma, fa, fb);

        [Pure]
        public Try<B> Map(Try<A> ma, Func<A, B> f) =>
            FOptional<MTry<A>, MTry<B>, Try<A>, Try<B>, A, B>.Inst.Map(ma, f);

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

    public struct FTry<A, B, C> :
        Applicative<Try<Func<A, Func<B, C>>>, Try<Func<B, C>>, Try<A>, Try<B>, Try<C>, A, B, C>
    {
        public static readonly FTry<A, B, C> Inst = default(FTry<A, B, C>);

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
}
