using System;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances
{
    public struct ApplOptionalUnsafe<OptionalA, OptionalB, OA, OB, A, B> :
        Functor<OA, OB, A, B>,
        BiFunctor<OA, OB, A, Unit, B>
        where OptionalA : struct, OptionalUnsafe<OA, A>
        where OptionalB : struct, OptionalUnsafe<OB, B>
    {
        public static readonly ApplOptionalUnsafe<OptionalA, OptionalB, OA, OB, A, B> Inst = default(ApplOptionalUnsafe<OptionalA, OptionalB, OA, OB, A, B>);

        [Pure]
        public OB BiMap(OA ma, Func<A, B> fa, Func<Unit, B> fb) =>
            FOptionalUnsafe<OptionalA, OptionalB, OA, OB, A, B>.Inst.BiMap(ma, fa, fb);

        [Pure]
        public OB Map(OA ma, Func<A, B> f) =>
            FOptionalUnsafe<OptionalA, OptionalB, OA, OB, A, B>.Inst.Map(ma, f);
    }

    public struct ApplOptionalUnsafe<OptionFAB, OptionFA, OptionFB, FAB, FA, FB, A, B> : 
        Applicative<FAB, FA, FB, A, B>
            where OptionFAB : struct, OptionalUnsafe<FAB, Func<A, B>>
            where OptionFA : struct, OptionalUnsafe<FA, A>
            where OptionFB : struct, OptionalUnsafe<FB, B>
    {
        public static readonly ApplOptionalUnsafe<OptionFAB, OptionFA, OptionFB, FAB, FA, FB, A, B> Inst = default(ApplOptionalUnsafe<OptionFAB, OptionFA, OptionFB, FAB, FA, FB, A, B>);

        [Pure]
        public FB Action(FA fa, FB fb)
        {
            ignore(default(OptionFA).IsSome(fa));
            return fb;
        }

        [Pure]
        public FB Apply(FAB fab, FA fa) =>
            default(OptionFAB).MatchUnsafe(fab,
                Some: f =>
                    default(OptionFA).MatchUnsafe(fa,
                        Some: a  => default(OptionFB).Optional(f(a)),
                        None: () => default(OptionFB).None),
                None: () => default(OptionFB).None);

        [Pure]
        public FA Pure(A x) =>
            default(OptionFA).Optional(x);
    }

    public struct ApplOptionalUnsafe<OptionFABC, OptionFBC, OptionFA, OptionFB, OptionFC, FABC, FBC, FA, FB, FC, A, B, C> : 
        Applicative<FABC, FBC, FA, FB, FC, A, B, C>
            where OptionFABC : struct, OptionalUnsafe<FABC, Func<A, Func<B, C>>>
            where OptionFBC  : struct, OptionalUnsafe<FBC, Func<B, C>>
            where OptionFA   : struct, OptionalUnsafe<FA, A>
            where OptionFB   : struct, OptionalUnsafe<FB, B>
            where OptionFC   : struct, OptionalUnsafe<FC, C>
    {
        public static readonly ApplOptionalUnsafe<OptionFABC, OptionFBC, OptionFA, OptionFB, OptionFC, FABC, FBC, FA, FB, FC, A, B, C> Inst = default(ApplOptionalUnsafe<OptionFABC, OptionFBC, OptionFA, OptionFB, OptionFC, FABC, FBC, FA, FB, FC, A, B, C>);

        [Pure]
        public FBC Apply(FABC fabc, FA fa) =>
            default(OptionFABC).MatchUnsafe(fabc,
                Some: f =>
                    default(OptionFA).MatchUnsafe(fa,
                        Some: a => default(OptionFBC).Optional(f(a)),
                        None: () => default(OptionFBC).None),
                None: () => default(OptionFBC).None);

        [Pure]
        public FC Apply(FABC fabc, FA fa, FB fb) =>
            default(OptionFABC).MatchUnsafe(fabc,
                Some: f =>
                    default(OptionFA).MatchUnsafe(fa,
                        Some: a =>
                            default(OptionFB).MatchUnsafe(fb,
                                Some: b => default(OptionFC).Optional(f(a)(b)),
                                None: () => default(OptionFC).None),
                        None: () => default(OptionFC).None),
                None: () => default(OptionFC).None);

        [Pure]
        public FA Pure(A x) =>
            default(OptionFA).Optional(x);
    }
}
