using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using System;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances
{
    public struct FValidation<FAIL, SUCCESS, SUCCESS2> : 
        Functor<Validation<FAIL, SUCCESS>, Validation<FAIL, SUCCESS2>, SUCCESS, SUCCESS2>,
        BiFunctor<Validation<FAIL, SUCCESS>, Validation<FAIL, SUCCESS2>, Seq<FAIL>, SUCCESS, SUCCESS2>
    {
        public static readonly FValidation<FAIL, SUCCESS, SUCCESS2> Inst = default(FValidation<FAIL, SUCCESS, SUCCESS2>);

        [Pure]
        public Validation<FAIL, SUCCESS2> BiMap(Validation<FAIL, SUCCESS> ma, Func<Seq<FAIL>, SUCCESS2> fa, Func<SUCCESS, SUCCESS2> fb) =>
            ma.Match(
                Fail:    a => Validation<FAIL, SUCCESS2>.Success(Check.NullReturn(fa(a))),
                Succ:    b => Validation<FAIL, SUCCESS2>.Success(Check.NullReturn(fb(b))));

        [Pure]
        public Validation<FAIL, SUCCESS2> Map(Validation<FAIL, SUCCESS> ma, Func<SUCCESS, SUCCESS2> f) =>
             ma.Match(
                Fail: Validation<FAIL, SUCCESS2>.Fail,
                Succ: b => Validation<FAIL, SUCCESS2>.Success(f(b)));
    }

    public struct FValidationBi<FAIL, SUCCESS, FAIL2, SUCCESS2> :
        BiFunctor<Validation<FAIL, SUCCESS>, Validation<FAIL2, SUCCESS2>, FAIL, SUCCESS, FAIL2, SUCCESS2>
    {
        public static readonly FValidationBi<FAIL, SUCCESS, FAIL2, SUCCESS2> Inst = default(FValidationBi<FAIL, SUCCESS, FAIL2, SUCCESS2>);

        [Pure]
        public Validation<FAIL2, SUCCESS2> BiMap(Validation<FAIL, SUCCESS> ma, Func<FAIL, FAIL2> fa, Func<SUCCESS, SUCCESS2> fb) =>
            ma.Match(
                Fail: a => Validation<FAIL2, SUCCESS2>.Fail(a.Map(fa)),
                Succ: b => Validation<FAIL2, SUCCESS2>.Success(fb(b)));
    }
}
