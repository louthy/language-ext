using LanguageExt.Traits;
using LanguageExt.Traits.Domain;

namespace LanguageExt;

public static partial class Prelude
{
    public partial class ruleFor<F, A>
        where F : Functor<F>
    {
        public sealed class All<R1, R2>
            : RuleK.All<R1, R2, F, A>, RuleK<All<R1, R2>, F, A>
            where R1 : RuleK<R1, F, A>, new()
            where R2 : RuleK<R2, F, A>, new();

        public sealed class Any<R1, R2>
            : RuleK.Any<R1, R2, F, A>, RuleK<Any<R1, R2>, F, A>
            where R1 : RuleK<R1, F, A>, new()
            where R2 : RuleK<R2, F, A>, new();

        public sealed class Not<R>
            : RuleK.Not<R, F, A>, RuleK<Not<R>, F, A>
            where R : RuleK<R, F, A>, new();
    }
}
