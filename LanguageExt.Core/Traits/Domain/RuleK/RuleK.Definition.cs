namespace LanguageExt.Traits.Domain;

public interface RuleK<SELF, F, A> : Rule<SELF, K<F, A>>
    where SELF : RuleK<SELF, F, A>, new()
    where F : Functor<F>;

