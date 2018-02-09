using LanguageExt.TypeClasses;
using System;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances
{
    public struct FEither<L, R, R2> : 
        Functor<Either<L, R>, Either<L, R2>, R, R2>,
        BiFunctor<Either<L, R>, Either<L, R2>, L, R, R2>
    {
        public static readonly FEither<L, R, R2> Inst = default(FEither<L, R, R2>);

        [Pure]
        public Either<L, R2> BiMap(Either<L, R> ma, Func<L, R2> fa, Func<R, R2> fb) =>
            ma.Match(
                Left: a => Either<L, R2>.Right(Check.NullReturn(fa(a))),
                Right: b => Either<L, R2>.Right(Check.NullReturn(fb(b))),
                Bottom: () => Either<L, R2>.Bottom);

        [Pure]
        public Either<L, R2> Map(Either<L, R> ma, Func<R, R2> f) =>
             ma.Match(
                Left: Either<L, R2>.Left,
                Right: b => Either<L, R2>.Right(f(b)),
                Bottom: () => Either<L, R2>.Bottom);
    }

    public struct FEitherBi<L, R, L2, R2> :
        BiFunctor<Either<L, R>, Either<L2, R2>, L, R, L2, R2>
    {
        public static readonly FEitherBi<L, R, L2, R2> Inst = default(FEitherBi<L, R, L2, R2>);

        [Pure]
        public Either<L2, R2> BiMap(Either<L, R> ma, Func<L, L2> fa, Func<R, R2> fb) =>
             ma.Match(
                Left: a => Either<L2, R2>.Left(Check.NullReturn(fa(a))),
                Right: b => Either<L2, R2>.Right(Check.NullReturn(fb(b))),
                Bottom: () => Either<L2, R2>.Bottom);
    }
}
