using System;
using LanguageExt.Common;

namespace LanguageExt.Traits.Domain;

public interface RuleK<SELF, F, A> : Rule<SELF>
    where SELF : RuleK<SELF, F, A>, new()
{
    public static abstract bool Check(K<F, A> value);
    
    public static virtual Fin<K<F, A>> Validate(K<F, A> value, Func<SELF, K<F, A>, Error> Fail) =>
        SELF.Check(value) ? Prelude.Pure(value) : Fail(SELF.Instance, value);
}

