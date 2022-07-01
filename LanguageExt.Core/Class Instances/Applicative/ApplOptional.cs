/*
#nullable enable

using System;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances
{
    public readonly struct ApplOptional<OptionalA, OptionalB, OA, OB, A, B> :
        Functor<OA, OB, A, B>,
        BiFunctor<OA, OB, A, Unit, B>
        where OptionalA : struct, Optional<OA, A>
        where OptionalB : struct, Optional<OB, B>
    {
        public static readonly ApplOptional<OptionalA, OptionalB, OA, OB, A, B> Inst = default;

        [Pure]
        public OB BiMap(OA ma, Func<A, B> fa, Func<Unit, B> fb) =>
            FOptional<OptionalA, OptionalB, OA, OB, A, B>.Inst.BiMap(ma, fa, fb);

        [Pure]
        public OB Map(OA ma, Func<A, B> f) =>
            FOptional<OptionalA, OptionalB, OA, OB, A, B>.Inst.Map(ma, f);
    }

    public readonly struct ApplOptional<OptionFAB, OptionFA, OptionFB, FAB, FA, FB, A, B> : 
        Applicative<FAB, FA, FB, A, B>
            where OptionFAB : struct, Optional<FAB, Func<A, B>>
            where OptionFA : struct, Optional<FA, A>
            where OptionFB : struct, Optional<FB, B>
    {
        public static readonly ApplOptional<OptionFAB, OptionFA, OptionFB, FAB, FA, FB, A, B> Inst = default;

        [Pure]
        public FB Action(FA fa, FB fb)
        {
            ignore(default(OptionFA).IsSome(fa));
            return fb;
        }

        [Pure]
        public FB Apply(FAB fab, FA fa) =>
            default(OptionFAB).Match(fab,
                Some: f =>
                    default(OptionFA).Match(fa,
                        Some: a  => default(OptionFB).Optional(f(a)),
                        None: () => default(OptionFB).None),
                None: () => default(OptionFB).None);

        [Pure]
        public FA Pure(A x) =>
            default(OptionFA).Optional(x);

        public FB Map(FA ma, Func<A, B> f) => 
            default(FOptional<OptionFA, OptionFB, FA, FB, A, B>).Map(ma, f);
    }
}
*/
