using System;
using LanguageExt.Common;
using LanguageExt.Traits;
using LanguageExt.Traits.Domain;
using static LanguageExt.Prelude;

namespace LanguageExt.Core.Traits.Domain.RuleK.RuleM;

public interface RuleM<SELF, M, A> : Rule<SELF>
    where SELF : RuleM<SELF, M, A>, new()
    where M : Monad<M>
{
    public static abstract K<M, bool> Check(A v);

    public static virtual FinT<M, A> Validate(
        A v, 
        Func<SELF, A, K<M, Error>> Fail) =>
        from followsRule in SELF.Check(v)
        let mResult = followsRule 
            ? FinT.lift<M, A>(Pure(v)) 
            : FinT.lift(Fail(SELF.Instance, v))
                  .Bind(FinT.Fail<M, A>)
        from result in mResult 
        select result;

}
