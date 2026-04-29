using System;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt.Traits.Domain;

public interface RuleM<SELF, M, A>
    where SELF : RuleM<SELF, M, A>, new()
    where M : Monad<M>
{
    public static virtual SELF Instance { get; } = new();

    public static abstract K<M, bool> Check(A v);

    public static virtual FinT<M, A> ValidateM(
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
