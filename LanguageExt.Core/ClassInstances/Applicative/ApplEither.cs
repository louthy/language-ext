using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using System;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances
{
    public struct ApplEither<L, R, R2> : 
        Functor<Either<L, R>, Either<L, R2>, R, R2>,
        BiFunctor<Either<L, R>, Either<L, R2>, L, R, R2>,
        Applicative<Either<L, Func<R, R2>>, Either<L, R>, Either<L, R2>, R, R2>
    {
        public static readonly ApplEither<L, R, R2> Inst = default(ApplEither<L, R, R2>);

        [Pure]
        public Either<L, R2> BiMap(Either<L, R> ma, Func<L, R2> fa, Func<R, R2> fb) =>
            FEither<L, R, R2>.Inst.BiMap(ma, fa, fb);

        [Pure]
        public Either<L, R2> Map(Either<L, R> ma, Func<R, R2> f) =>
            FEither<L, R, R2>.Inst.Map(ma, f);

        [Pure]
        public Either<L, R2> Apply(Either<L, Func<R, R2>> fab, Either<L, R> fa) =>
            from f in fab
            from a in fa
            select f(a);

        [Pure]
        public Either<L, R2> Action(Either<L, R> fa, Either<L, R2> fb) =>
            from a in fa
            from b in fb
            select b;

        [Pure]
        public Either<L, R> Pure(R x) =>
            Right<L, R>(x);
    }

    public struct ApplEitherBi<L, R, L2, R2> :
        BiFunctor<Either<L, R>, Either<L2, R2>, L, R, L2, R2>,
        Applicative<Either<L, Func<R, R2>>, Either<L, R>, Either<L, R2>, R, R2>
    {
        public static readonly ApplEitherBi<L, R, L2, R2> Inst = default(ApplEitherBi<L, R, L2, R2>);

        [Pure]
        public Either<L2, R2> BiMap(Either<L, R> ma, Func<L, L2> fa, Func<R, R2> fb) =>
            ma.Match(
                Right: b => Either<L2, R2>.Right(Check.NullReturn(fb(b))),
                Left:  a => Either<L2, R2>.Left(Check.NullReturn(fa(a))),
                Bottom: () => Either<L2, R2>.Bottom);

        [Pure]
        public Either<L, R2> Apply(Either<L, Func<R, R2>> fab, Either<L, R> fa) =>
            from f in fab
            from a in fa
            select f(a);

        [Pure]
        public Either<L, R2> Action(Either<L, R> fa, Either<L, R2> fb) =>
            from a in fa
            from b in fb
            select b;

        [Pure]
        public Either<L, R> Pure(R x) =>
            Right<L, R>(x);
    }

    public struct ApplEither<L, A, B, C> :
        Applicative<Either<L, Func<A, Func<B, C>>>, Either<L, Func<B, C>>, Either<L, A>, Either<L, B>, Either<L, C>, A, B, C>
    {
        public static readonly ApplEither<L, A, B, C> Inst = default(ApplEither<L, A, B, C>);

        [Pure]
        public Either<L, Func<B, C>> Apply(Either<L, Func<A, Func<B, C>>> fab, Either<L, A> fa) =>
            from f in fab
            from a in fa
            select f(a);

        [Pure]
        public Either<L, C> Apply(Either<L, Func<A, Func<B, C>>> fab, Either<L, A> fa, Either<L, B> fb) =>
            from f in fab
            from a in fa
            from b in fb
            select f(a)(b);

        [Pure]
        public Either<L, A> Pure(A x) =>
            Right<L, A>(x);
    }
}
