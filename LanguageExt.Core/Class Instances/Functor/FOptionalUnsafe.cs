using System;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances;

public struct FOptionalUnsafe<OptionalA, OptionalB, OA, OB, A, B> : 
    Functor<OA, OB, A?, B?>,
    BiFunctor<OA, OB, A?, Unit, B?>
    where OptionalA : OptionalUnsafe<OA, A>
    where OptionalB : OptionalUnsafe<OB, B>
{
    [Pure]
    public static OB BiMap(OA ma, Func<A?, B?> fa, Func<Unit, B?> fb) =>
        OptionalA.MatchUnsafe(ma,
                              Some: a => OptionalB.Some(fa(a)),
                              None: () => OptionalB.Some(fb(unit))) ?? throw new ValueIsNullException();

    [Pure]
    public static OB Map(OA ma, Func<A?, B?> f) =>
        OptionalA.MatchUnsafe(ma,
                              Some: a => OptionalB.Some(f(a)),
                              None: () => OptionalB.None) ?? throw new ValueIsNullException();
}
