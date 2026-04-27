
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

    public static virtual RuleK.Lift<SELF, M, A> ToRuleK<M>() 
        where M : Monad<M> => 
        new();

    public static virtual Fin<A> Validate(A value, Func<SELF, A, Error> Fail) =>
        SELF.Check(value) ? value : Fail(SELF.Instance, value);

}

public interface ComposedRule<R1, R2, A>
    where R1 : Rule<R1, A>, new()
    where R2 : Rule<R2, A>, new();
