using LanguageExt.TypeClasses;
using System;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances
{
    public struct FEitherUnsafe<L, R, R2> :
        Functor<EitherUnsafe<L, R>, EitherUnsafe<L, R2>, R, R2>,
        BiFunctor<EitherUnsafe<L, R>, EitherUnsafe<L, R2>, L, R, R2>
    {
        public static readonly FEitherUnsafe<L, R, R2> Inst = default(FEitherUnsafe<L, R, R2>);

        [Pure]
        public EitherUnsafe<L, R2> BiMap(EitherUnsafe<L, R> ma, Func<L, R2> fa, Func<R, R2> fb) =>
            default(MEitherUnsafe<L, R>).MatchUnsafe(ma,
                Left: a => EitherUnsafe<L, R2>.Right(Check.NullReturn(fa(a))),
                Right: b => EitherUnsafe<L, R2>.Right(Check.NullReturn(fb(b))),
                Bottom: () => EitherUnsafe<L, R2>.Bottom);

        [Pure]
        public EitherUnsafe<L, R2> Map(EitherUnsafe<L, R> ma, Func<R, R2> f) =>
            default(MEitherUnsafe<L, R>).MatchUnsafe(ma,
                Left: EitherUnsafe<L, R2>.Left,
                Right: b => EitherUnsafe<L, R2>.Right(f(b)),
                Bottom: () => EitherUnsafe<L, R2>.Bottom);
    }

    public struct FEitherUnsafeBi<L, R, L2, R2> :
        BiFunctor<EitherUnsafe<L, R>, EitherUnsafe<L2, R2>, L, R, L2, R2>
    {
        public static readonly FEitherUnsafeBi<L, R, L2, R2> Inst = default(FEitherUnsafeBi<L, R, L2, R2>);

        [Pure]
        public EitherUnsafe<L2, R2> BiMap(EitherUnsafe<L, R> ma, Func<L, L2> fa, Func<R, R2> fb) =>
            default(MEitherUnsafe<L, R>).MatchUnsafe(ma,
                Left: a => EitherUnsafe<L2, R2>.Left(Check.NullReturn(fa(a))),
                Right: b => EitherUnsafe<L2, R2>.Right(Check.NullReturn(fb(b))),
                Bottom: () => EitherUnsafe<L2, R2>.Bottom);
    }
}
