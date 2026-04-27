using LanguageExt.Traits;
using LanguageExt.Traits.Domain;

namespace LanguageExt;

public static partial class Prelude
{
    public partial class ruleMFor<M, A>
        where M : Monad<M>
    {
        public sealed class All<R1, R2>
            : RuleM.All<R1, R2, M, A>, RuleM<All<R1, R2>, M, A>
            where R1 : RuleM<R1, M, A>, new()
            where R2 : RuleM<R2, M, A>, new();

        public sealed class Any<R1, R2>
            : RuleM.Any<R1, R2, M, A>, RuleM<Any<R1, R2>, M, A>
            where R1 : RuleM<R1, M, A>, new()
            where R2 : RuleM<R2, M, A>, new();

        public sealed class Not<R>
            : RuleM.Not<R, M, A>, RuleM<Not<R>, M, A>
            where R : RuleM<R, M, A>, new();

        public sealed class Lift<R>
            : RuleM.Lift<R, M, A>, RuleM<Lift<R>, M, A>
            where R : Rule<R, A>, new();
    }
}
