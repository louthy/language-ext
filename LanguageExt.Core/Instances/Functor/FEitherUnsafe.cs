using LanguageExt.TypeClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageExt.Instances
{
    public struct FEitherUnsafe<L, R, Res> :
        Functor<EitherUnsafe<L, R>, EitherUnsafe<L, Res>, R, Res>,
        BiFunctor<EitherUnsafe<L, R>, EitherUnsafe<L, Res>, L, R, Res>
    {
        public static readonly FEitherUnsafe<L, R, Res> Inst = default(FEitherUnsafe<L, R, Res>);

        public EitherUnsafe<L, Res> BiMap(EitherUnsafe<L, R> ma, Func<L, Res> fa, Func<R, Res> fb) =>
            default(MEitherUnsafe<L, R>).Match(ma,
                Choice1: a => EitherUnsafe<L, Res>.Right(fa(a)),
                Choice2: b => EitherUnsafe<L, Res>.Right(fb(b)),
                Bottom: () => EitherUnsafe<L, Res>.Bottom);

        public EitherUnsafe<L, Res> Map(EitherUnsafe<L, R> ma, Func<R, Res> f) =>
            default(MEitherUnsafe<L, R>).Match(ma,
                Choice1: EitherUnsafe<L, Res>.Left,
                Choice2: b => EitherUnsafe<L, Res>.Right(f(b)),
                Bottom: () => EitherUnsafe<L, Res>.Bottom);
    }

    public struct FEitherUnsafe<L, R, L2, R2> :
        BiFunctor<EitherUnsafe<L, R>, EitherUnsafe<L2, R2>, L, R, L2, R2>
    {
        public static readonly FEitherUnsafe<L, R, L2, R2> Inst = default(FEitherUnsafe<L, R, L2, R2>);

        public EitherUnsafe<L2, R2> BiMap(EitherUnsafe<L, R> ma, Func<L, L2> fa, Func<R, R2> fb) =>
            default(MEitherUnsafe<L, R>).Match(ma,
                Choice1: a => EitherUnsafe<L2, R2>.Left(fa(a)),
                Choice2: b => EitherUnsafe<L2, R2>.Right(fb(b)),
                Bottom: () => EitherUnsafe<L2, R2>.Bottom);
    }
}
