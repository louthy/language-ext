using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using System;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances
{
    public struct FValidation<MonoidFail, FAIL, SUCCESS, SUCCESS2> : 
        Functor<Validation<MonoidFail, FAIL, SUCCESS>, Validation<MonoidFail, FAIL, SUCCESS2>, SUCCESS, SUCCESS2>,
        BiFunctor<Validation<MonoidFail, FAIL, SUCCESS>, Validation<MonoidFail, FAIL, SUCCESS2>, FAIL, SUCCESS, SUCCESS2>
        where MonoidFail : struct, Monoid<FAIL>, Eq<FAIL>
    {
        public static readonly FValidation<MonoidFail, FAIL, SUCCESS, SUCCESS2> Inst = default(FValidation<MonoidFail, FAIL, SUCCESS, SUCCESS2>);

        [Pure]
        public Validation<MonoidFail, FAIL, SUCCESS2> BiMap(Validation<MonoidFail, FAIL, SUCCESS> ma, Func<FAIL, SUCCESS2> fa, Func<SUCCESS, SUCCESS2> fb) =>
            ma.Match(
                Fail:    a => Validation<MonoidFail, FAIL, SUCCESS2>.Success(Check.NullReturn(fa(a))),
                Succ:    b => Validation<MonoidFail, FAIL, SUCCESS2>.Success(Check.NullReturn(fb(b))));

        [Pure]
        public Validation<MonoidFail, FAIL, SUCCESS2> Map(Validation<MonoidFail, FAIL, SUCCESS> ma, Func<SUCCESS, SUCCESS2> f) =>
             ma.Match(
                Fail: Validation<MonoidFail, FAIL, SUCCESS2>.Fail,
                Succ: b => Validation<MonoidFail, FAIL, SUCCESS2>.Success(f(b)));
    }
}
