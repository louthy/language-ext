using System;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances
{
    public struct FOptional<OptionalA, OptionalB, OA, OB, A, B> : 
        Functor<OA, OB, A, B>,
        BiFunctor<OA, OB, Unit, A, B>
        where OptionalA : struct, Optional<OA, A>
        where OptionalB : struct, Optional<OB, B>
    {
        public static readonly FOptional<OptionalA, OptionalB, OA, OB, A, B> Inst = default(FOptional<OptionalA, OptionalB, OA, OB, A, B>);

        [Pure]
        public OB BiMap(OA ma, Func<Unit, B> fa, Func<A, B> fb) =>
            default(OptionalA).Match(ma,
                Some: a  => default(OptionalB).Optional(fb(a)),
                None: () => default(OptionalB).Optional(fa(unit)));

        [Pure]
        public OB Map(OA ma, Func<A, B> f) =>
            default(OptionalA).Match(ma,
                Some: a  => default(OptionalB).Some(f(a)),
                None: () => default(OptionalB).None);
    }
}
