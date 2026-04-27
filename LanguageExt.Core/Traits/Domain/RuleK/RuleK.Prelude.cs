using LanguageExt.Traits;
using LanguageExt.Traits.Domain;

namespace LanguageExt;

public static partial class Prelude
{
    public partial class ruleForK2<M, A>
        where M : Monad<M>
    {
        public sealed class All<R1, R2>
            : RuleK2.All<R1, R2, M, A>, RuleK2<All<R1, R2>, M, A>
            where R1 : RuleK2<R1, M, A>, new()
            where R2 : RuleK2<R2, M, A>, new();

        public sealed class Any<R1, R2>
            : RuleK2.Any<R1, R2, M, A>, RuleK2<Any<R1, R2>, M, A>
            where R1 : RuleK2<R1, M, A>, new()
            where R2 : RuleK2<R2, M, A>, new();

        public sealed class Not<R>
            : RuleK2.Not<R, M, A>, RuleK2<Not<R>, M, A>
            where R : RuleK2<R, M, A>, new();

        public sealed class Lift<R>
            : RuleK2.Lift<R, M, A>, RuleK2<Lift<R>, M, A>
            where R : Rule<R, A>, new();
    }
}
