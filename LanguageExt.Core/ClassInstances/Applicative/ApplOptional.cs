using System;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances
{
    public struct ApplOptional<OptionalA, OptionalB, OA, OB, A, B> :
        Functor<OA, OB, A, B>,
        BiFunctor<OA, OB, A, Unit, B>
        where OptionalA : struct, Optional<OA, A>
        where OptionalB : struct, Optional<OB, B>
    {
        public static readonly ApplOptional<OptionalA, OptionalB, OA, OB, A, B> Inst = default(ApplOptional<OptionalA, OptionalB, OA, OB, A, B>);

        [Pure]
        public OB BiMap(OA ma, Func<A, B> fa, Func<Unit, B> fb) =>
            FOptional<OptionalA, OptionalB, OA, OB, A, B>.Inst.BiMap(ma, fa, fb);

        [Pure]
        public OB Map(OA ma, Func<A, B> f) =>
            FOptional<OptionalA, OptionalB, OA, OB, A, B>.Inst.Map(ma, f);
    }

    public struct ApplOptional<OptionFAB, OptionFA, OptionFB, FAB, FA, FB, A, B> : 
        Applicative<FAB, FA, FB, A, B>
            where OptionFAB : struct, Optional<FAB, Func<A, B>>
            where OptionFA : struct, Optional<FA, A>
            where OptionFB : struct, Optional<FB, B>
    {
        public static readonly ApplOptional<OptionFAB, OptionFA, OptionFB, FAB, FA, FB, A, B> Inst = default(ApplOptional<OptionFAB, OptionFA, OptionFB, FAB, FA, FB, A, B>);

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
    }

    public struct ApplOptional<OptionFABC, OptionFBC, OptionFA, OptionFB, OptionFC, FABC, FBC, FA, FB, FC, A, B, C> : 
        Applicative<FABC, FBC, FA, FB, FC, A, B, C>
            where OptionFABC : struct, Optional<FABC, Func<A, Func<B, C>>>
            where OptionFBC  : struct, Optional<FBC, Func<B, C>>
            where OptionFA   : struct, Optional<FA, A>
            where OptionFB   : struct, Optional<FB, B>
            where OptionFC   : struct, Optional<FC, C>
    {
        public static readonly ApplOptional<OptionFABC, OptionFBC, OptionFA, OptionFB, OptionFC, FABC, FBC, FA, FB, FC, A, B, C> Inst = default(ApplOptional<OptionFABC, OptionFBC, OptionFA, OptionFB, OptionFC, FABC, FBC, FA, FB, FC, A, B, C>);

        [Pure]
        public FBC Apply(FABC fabc, FA fa) =>
            default(OptionFABC).Match(fabc,
                Some: f =>
                    default(OptionFA).Match(fa,
                        Some: a => default(OptionFBC).Optional(f(a)),
                        None: () => default(OptionFBC).None),
                None: () => default(OptionFBC).None);

        [Pure]
        public FC Apply(FABC fabc, FA fa, FB fb) =>
            default(OptionFABC).Match(fabc,
                Some: f =>
                    default(OptionFA).Match(fa,
                        Some: a =>
                            default(OptionFB).Match(fb,
                                Some: b => default(OptionFC).Optional(f(a)(b)),
                                None: () => default(OptionFC).None),
                        None: () => default(OptionFC).None),
                None: () => default(OptionFC).None);

        [Pure]
        public FA Pure(A x) =>
            default(OptionFA).Optional(x);
    }
}
