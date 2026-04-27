using System;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt.Traits;

public interface RuleK<SELF, M, A> : Rule<SELF>
    where SELF : RuleK<SELF, M, A>, new()
    where M : Monad<M>
{
    public static abstract K<M, bool> Check(A value);

    public static virtual FinT<M, Unit> Validate(
        A value, 
        Func<SELF, A, K<M, Error>> Fail) =>
        from followsRule in SELF.Check(value)
        let mResult = followsRule 
            ? FinT.lift<M, Unit>(Pure(unit)) : LiftError(value, Fail)
        from result in mResult 
        select result;

    private static FinT<M, Unit> LiftError(A value, Func<SELF, A, K<M, Error>> Fail) =>
        from error in FinT.lift(Fail(SELF.Instance, value))
        from _1 in FinT.Fail<M, Unit>(error)
        select unit;
}
