using LanguageExt.TypeClasses;
using System;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances;

public struct FValidation<MonoidFail, FAIL, SUCCESS, SUCCESS2> : 
    Functor<Validation<MonoidFail, FAIL, SUCCESS>, Validation<MonoidFail, FAIL, SUCCESS2>, SUCCESS, SUCCESS2>,
    BiFunctor<Validation<MonoidFail, FAIL, SUCCESS>, Validation<MonoidFail, FAIL, SUCCESS2>, FAIL, SUCCESS, SUCCESS2>
    where MonoidFail : Monoid<FAIL>, Eq<FAIL>
{
    [Pure]
    public static Validation<MonoidFail, FAIL, SUCCESS2> BiMap(Validation<MonoidFail, FAIL, SUCCESS> ma, Func<FAIL, SUCCESS2> fa, Func<SUCCESS, SUCCESS2> fb) =>
        ma.Match(
            Fail:    a => Validation<MonoidFail, FAIL, SUCCESS2>.Success(Check.NullReturn(fa(a))),
            Succ:    b => Validation<MonoidFail, FAIL, SUCCESS2>.Success(Check.NullReturn(fb(b))));

    [Pure]
    public static Validation<MonoidFail, FAIL, SUCCESS2> Map(Validation<MonoidFail, FAIL, SUCCESS> ma, Func<SUCCESS, SUCCESS2> f) =>
        ma.Match(
            Fail: Validation<MonoidFail, FAIL, SUCCESS2>.Fail,
            Succ: b => Validation<MonoidFail, FAIL, SUCCESS2>.Success(f(b)));
}

public struct FValidationBi<MonoidFail, FAIL, SUCCESS, MonoidFail2, FAIL2, SUCCESS2> :
    BiFunctor<Validation<MonoidFail, FAIL, SUCCESS>, Validation<MonoidFail2, FAIL2, SUCCESS2>, FAIL, SUCCESS, FAIL2, SUCCESS2>
    where MonoidFail : Monoid<FAIL>, Eq<FAIL>
    where MonoidFail2 : Monoid<FAIL2>, Eq<FAIL2>
{
    [Pure]
    public static Validation<MonoidFail2, FAIL2, SUCCESS2> BiMap(Validation<MonoidFail, FAIL, SUCCESS> ma, Func<FAIL, FAIL2> fa, Func<SUCCESS, SUCCESS2> fb) =>
        ma.Match(
            Fail: a => Validation<MonoidFail2, FAIL2, SUCCESS2>.Fail(fa(a)),
            Succ: b => Validation<MonoidFail2, FAIL2, SUCCESS2>.Success(fb(b)));
}
