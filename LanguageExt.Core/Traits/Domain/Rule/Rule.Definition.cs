
using System;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt.Traits.Domain;

public interface Rule<SELF> 
    where SELF : Rule<SELF>, new()
{
    public static virtual SELF Instance { get; } = new();
}

public interface Rule<SELF, A> : Rule<SELF>
    where SELF : Rule<SELF, A>, new()
{
    public static abstract bool Check(A value);

    public static virtual RuleM.Lift<SELF, M, A> ToRuleK<M>() 
        where M : Monad<M> => 
        new();

    public static virtual Fin<A> Validate(A value, Func<SELF, A, Error> Fail) =>
        SELF.Check(value) ? value : Fail(SELF.Instance, value);

}

