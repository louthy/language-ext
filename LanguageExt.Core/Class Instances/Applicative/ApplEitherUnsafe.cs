using LanguageExt.TypeClasses;
using System;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances
{
    public struct ApplEitherUnsafe<L, R, R2> :
        Functor<EitherUnsafe<L, R>, EitherUnsafe<L, R2>, R, R2>,
        BiFunctor<EitherUnsafe<L, R>, EitherUnsafe<L, R2>, L, R, R2>,
        Applicative<EitherUnsafe<L, Func<R, R2>>, EitherUnsafe<L, R>, EitherUnsafe<L, R2>, R, R2>
    {
        public static readonly ApplEitherUnsafe<L, R, R2> Inst = default(ApplEitherUnsafe<L, R, R2>);

        [Pure]
        public EitherUnsafe<L, R2> BiMap(EitherUnsafe<L, R> ma, Func<L, R2> fa, Func<R, R2> fb) =>
            FEitherUnsafe<L, R, R2>.Inst.BiMap(ma, fa, fb);

        [Pure]
        public EitherUnsafe<L, R2> Map(EitherUnsafe<L, R> ma, Func<R, R2> f) =>
            FEitherUnsafe<L, R, R2>.Inst.Map(ma, f);

        [Pure]
        public EitherUnsafe<L, R2> Apply(EitherUnsafe<L, Func<R, R2>> fab, EitherUnsafe<L, R> fa) =>
            from f in fab
            from a in fa
            select f(a);

        [Pure]
        public EitherUnsafe<L, R2> Action(EitherUnsafe<L, R> fa, EitherUnsafe<L, R2> fb) =>
            from a in fa
            from b in fb
            select b;

        [Pure]
        public EitherUnsafe<L, R> Pure(R x) =>
            x;

    }

    public struct ApplEitherUnsafeBi<L, R, L2, R2> :
        BiFunctor<EitherUnsafe<L, R>, EitherUnsafe<L2, R2>, L, R, L2, R2>,
        Applicative<EitherUnsafe<L, Func<R, R2>>, EitherUnsafe<L, R>, EitherUnsafe<L, R2>, R, R2>
    {
        public static readonly ApplEitherUnsafeBi<L, R, L2, R2> Inst = default(ApplEitherUnsafeBi<L, R, L2, R2>);

        [Pure]
        public EitherUnsafe<L2, R2> BiMap(EitherUnsafe<L, R> ma, Func<L, L2> fa, Func<R, R2> fb) =>
            default(MEitherUnsafe<L, R>).MatchUnsafe(ma,
                Left: a => EitherUnsafe<L2, R2>.Left(Check.NullReturn(fa(a))),
                Right: b => EitherUnsafe<L2, R2>.Right(Check.NullReturn(fb(b))),
                Bottom: () => EitherUnsafe<L2, R2>.Bottom);

        [Pure]
        public EitherUnsafe<L, R2> Apply(EitherUnsafe<L, Func<R, R2>> fab, EitherUnsafe<L, R> fa) =>
            from f in fab
            from a in fa
            select f(a);

        [Pure]
        public EitherUnsafe<L, R2> Action(EitherUnsafe<L, R> fa, EitherUnsafe<L, R2> fb) =>
            from a in fa
            from b in fb
            select b;

        [Pure]
        public EitherUnsafe<L, R> Pure(R x) =>
            x;
    }

    public struct ApplEitherUnsafe<L, A, B, C> :
        Applicative<EitherUnsafe<L, Func<A, Func<B, C>>>, EitherUnsafe<L, Func<B, C>>, EitherUnsafe<L, A>, EitherUnsafe<L, B>, EitherUnsafe<L, C>, A, B, C>
    {
        public static readonly ApplEitherUnsafe<L, A, B, C> Inst = default(ApplEitherUnsafe<L, A, B, C>);

        [Pure]
        public EitherUnsafe<L, Func<B, C>> Apply(EitherUnsafe<L, Func<A, Func<B, C>>> fab, EitherUnsafe<L, A> fa) =>
            from f in fab
            from a in fa
            select f(a);

        [Pure]
        public EitherUnsafe<L, C> Apply(EitherUnsafe<L, Func<A, Func<B, C>>> fab, EitherUnsafe<L, A> fa, EitherUnsafe<L, B> fb) =>
            from f in fab
            from a in fa
            from b in fb
            select f(a)(b);

        [Pure]
        public EitherUnsafe<L, A> Pure(A x) =>
            x;
    }
}
