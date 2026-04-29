using System;
using LanguageExt.Common;

namespace LanguageExt.Traits.Domain;


public interface RuleT<SELF, T, M, A>
    where SELF : RuleT<SELF, T, M, A>, new()
    where T : MonadT<T, M>
    where M : Monad<M>
{
    public static virtual SELF Instance { get; } = new();

    static abstract K<T, bool> Check(K<M, A> ma);

    public static virtual K<T, bool> Check(A a) =>
        SELF.Check(M.Pure(a));

    public static virtual FinT<T, A> ValidateT(K<M, A> ma, Func<SELF, A, K<T, Error>> Fail) =>
        from value in T.Lift(ma)
        from followsRule in SELF.Check(value)
        let mResult = followsRule
            ? FinT.lift(T.Pure(value)) 
            : FinT.lift(Fail(SELF.Instance, value))
                  .Bind(FinT.Fail<T, A>)
        from result in mResult
        select result;
}
