using LanguageExt.TypeClasses;
using System;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances
{
    public struct FEither<L, R, Res> : 
        Functor<Either<L, R>, Either<L, Res>, R, Res>,
        BiFunctor<Either<L, R>, Either<L, Res>, L, R, Res>
    {
        public static readonly FEither<L, R, Res> Inst = default(FEither<L, R, Res>);

        [Pure]
        public Either<L, Res> BiMap(Either<L, R> ma, Func<L, Res> fa, Func<R, Res> fb) =>
            default(MEither<L, R>).Match(ma,
                Choice1: a => Either<L, Res>.Right(Check.NullReturn(fa(a))),
                Choice2: b => Either<L, Res>.Right(Check.NullReturn(fb(b))),
                Bottom: () => Either<L, Res>.Bottom);

        [Pure]
        public Either<L, Res> Map(Either<L, R> ma, Func<R, Res> f) =>
            default(MEither<L, R>).Match(ma,
                Choice1: Either<L, Res>.Left,
                Choice2: b => Either<L, Res>.Right(f(b)),
                Bottom: () => Either<L, Res>.Bottom);
    }

    public struct FEither<L, R, L2, R2> :
        BiFunctor<Either<L, R>, Either<L2, R2>, L, R, L2, R2>
    {
        public static readonly FEither<L, R, L2, R2> Inst = default(FEither<L, R, L2, R2>);

        [Pure]
        public Either<L2, R2> BiMap(Either<L, R> ma, Func<L, L2> fa, Func<R, R2> fb) =>
            default(MEither<L, R>).Match(ma,
                Choice1: a => Either<L2, R2>.Left(Check.NullReturn(fa(a))),
                Choice2: b => Either<L2, R2>.Right(Check.NullReturn(fb(b))),
                Bottom: () => Either<L2, R2>.Bottom);
    }
}
