using System;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances;

public struct ApplOptional<OptionalA, OptionalB, OA, OB, A, B> :
    Functor<OA, OB, A, B>,
    BiFunctor<OA, OB, A, Unit, B>
    where OptionalA : Optional<OA, A>
    where OptionalB : Optional<OB, B>
{
    [Pure]
    public static OB BiMap(OA ma, Func<A, B> fa, Func<Unit, B> fb) =>
        FOptional<OptionalA, OptionalB, OA, OB, A, B>.BiMap(ma, fa, fb);

    [Pure]
    public static OB Map(OA ma, Func<A, B> f) =>
        FOptional<OptionalA, OptionalB, OA, OB, A, B>.Map(ma, f);
}

public struct ApplOptional<OptionFAB, OptionFA, OptionFB, FAB, FA, FB, A, B> :
    Applicative<FAB, FA, FB, A, B>
    where OptionFAB : Optional<FAB, Func<A, B>>
    where OptionFA : Optional<FA, A>
    where OptionFB : Optional<FB, B>
{
    [Pure]
    public static FB Action(FA fa, FB fb)
    {
        ignore(OptionFA.IsSome(fa));
        return fb;
    }

    [Pure]
    public static FB Apply(FAB fab, FA fa) =>
        OptionFAB.Match(fab,
                        Some: f =>
                            OptionFA.Match(fa,
                                           Some: a => OptionFB.MkOptional(f(a)),
                                           None: () => OptionFB.None),
                        None: () => OptionFB.None);

    [Pure]
    public static FA Pure(A x) =>
        OptionFA.MkOptional(x);
}
