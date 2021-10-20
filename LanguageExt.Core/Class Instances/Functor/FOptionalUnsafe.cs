using System;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances
{
    public struct FOptionalUnsafe<OptionalA, OptionalB, OA, OB, A, B> : 
        Functor<OA, OB, A, B>,
        BiFunctor<OA, OB, A, Unit, B>
        where OptionalA : struct, OptionalUnsafe<OA, A>
        where OptionalB : struct, OptionalUnsafe<OB, B>
    {
        public static readonly FOptionalUnsafe<OptionalA, OptionalB, OA, OB, A, B> Inst = default(FOptionalUnsafe<OptionalA, OptionalB, OA, OB, A, B>);

        [Pure]
        public OB BiMap(OA ma, Func<A, B> fa, Func<Unit, B> fb) =>
            default(OptionalA).MatchUnsafe(ma,
                Some: a  => default(OptionalB).Optional(fa(a)),
                None: () => default(OptionalB).Optional(fb(unit)));

        [Pure]
        public OB Map(OA ma, Func<A, B> f) =>
            default(OptionalA).MatchUnsafe(ma,
                Some: a  => default(OptionalB).Some(f(a)),
                None: () => default(OptionalB).None);
    }
}
