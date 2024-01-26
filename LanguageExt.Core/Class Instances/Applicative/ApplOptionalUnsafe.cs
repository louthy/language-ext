using System;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances;

public struct ApplOptionalUnsafe<OptionalA, OptionalB, OA, OB, A, B> :
    Functor<OA, OB, A?, B?>,
    BiFunctor<OA, OB, A?, Unit, B?>
    where OptionalA : OptionalUnsafe<OA, A>
    where OptionalB : OptionalUnsafe<OB, B>
{
    [Pure]
    public static OB BiMap(OA ma, Func<A?, B?> fa, Func<Unit, B?> fb) =>
        FOptionalUnsafe<OptionalA, OptionalB, OA, OB, A, B>.BiMap(ma, fa, fb);

    [Pure]
    public static OB Map(OA ma, Func<A?, B?> f) =>
        FOptionalUnsafe<OptionalA, OptionalB, OA, OB, A, B>.Map(ma, f);
}

public struct ApplOptionalUnsafe<OptionFAB, OptionFA, OptionFB, FAB, FA, FB, A, B> :
    Applicative<FAB, FA, FB, A?, B?>
    where OptionFAB : OptionalUnsafe<FAB, Func<A?, B?>>
    where OptionFA : OptionalUnsafe<FA, A>
    where OptionFB : OptionalUnsafe<FB, B>
{
    [Pure]
    public static FB Action(FA fa, FB fb)
    {
        ignore(OptionFA.IsSome(fa));
        return fb;
    }

    [Pure]
    public static FB Apply(FAB fab, FA fa) =>
        OptionFAB.MatchUnsafe(fab,
                              Some: f =>
                                  OptionFA.MatchUnsafe(fa,
                                                       Some: a => f is null ? OptionFB.None : OptionFB.Some(f(a)),
                                                       None: () => OptionFB.None),
                              None: () => OptionFB.None) ?? throw new ValueIsNullException();

    [Pure]
    public static FA Pure(A? x) =>
        OptionFA.Some(x);
}
