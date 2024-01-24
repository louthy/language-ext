using System;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances;

public struct FOptional<OptionalA, OptionalB, OA, OB, A, B> :
    Functor<OA, OB, A, B>,
    BiFunctor<OA, OB, A, Unit, B>
    where OptionalA : Optional<OA, A>
    where OptionalB : Optional<OB, B>
{
    [Pure]
    public static OB BiMap(OA ma, Func<A, B> fa, Func<Unit, B> fb) =>
        OptionalA.Match(ma,
                        Some: a => OptionalB.MkOptional(fa(a)),
                        None: () => OptionalB.MkOptional(fb(unit)));

    [Pure]
    public static OB Map(OA ma, Func<A, B> f) =>
        OptionalA.Match(ma,
                        Some: a => OptionalB.Some(f(a)),
                        None: () => OptionalB.None);
}
