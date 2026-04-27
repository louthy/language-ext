using LanguageExt.Traits;
using LanguageExt.Traits.Domain;

namespace LanguageExt;

public static partial class Prelude
{
    public partial class ruleForK<M, A>
        where M : Monad<M>
    {
        public sealed class All<R1, R2>
            : RuleK.All<R1, R2, M, A>, RuleK<All<R1, R2>, M, A>
            where R1 : RuleK<R1, M, A>, new()
            where R2 : RuleK<R2, M, A>, new();

        public sealed class Any<R1, R2>
            : RuleK.Any<R1, R2, M, A>, RuleK<Any<R1, R2>, M, A>
            where R1 : RuleK<R1, M, A>, new()
            where R2 : RuleK<R2, M, A>, new();

        public sealed class Not<R>
            : RuleK.Not<R, M, A>, RuleK<Not<R>, M, A>
            where R : RuleK<R, M, A>, new();

        public sealed class Lift<R>
            : RuleK.Lift<R, M, A>, RuleK<Lift<R>, M, A>
            where R : Rule<R, A>, new();
    }
}
