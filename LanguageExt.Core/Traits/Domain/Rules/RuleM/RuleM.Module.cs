namespace LanguageExt.Traits.Domain;

public static partial class RuleM<M>
    where M : Monad<M>
{
    public static partial class For<A>
    {
        public class Id<R> : RuleM<Id<R>, M, A>
            where R : RuleM<R, M, A>, new()
        {
            public R Inner => R.Instance;

            public static K<M, bool> Check(A value) =>
                R.Check(value);
        }

        public class All<R1, R2>
            : RuleM<All<R1, R2>, M, A>
            where R1 : RuleM<R1, M, A>, new()
            where R2 : RuleM<R2, M, A>, new()
        {
            public R1 First => R1.Instance;

            public R2 Second => R2.Instance;

            public static K<M, bool> Check(A value) =>
                from r1Val in R1.Check(value)
                from r2Val in R2.Check(value)
                select r1Val && r2Val;
        }

        public class Any<R1, R2>
            : RuleM<Any<R1, R2>, M, A>
            where R1 : RuleM<R1, M, A>, new()
            where R2 : RuleM<R2, M, A>, new()
        {
            public R1 First => R1.Instance;
            public R2 Second => R2.Instance;

            public static K<M, bool> Check(A value) =>
                from r1Val in R1.Check(value)
                from r2Val in R2.Check(value)
                select r1Val || r2Val;
        }

        public class Not<R> : RuleM<Not<R>, M, A>
            where R : RuleM<R, M, A>, new()
        {
            public R NegatedRule => R.Instance;

            public static K<M, bool> Check(A value) =>
                from rVal in R.Check(value)
                select !rVal;
        }

        public class Lift<R> : RuleM<Lift<R>, M, A>
            where R : Rule<R, A>, new()
        {
            public R Lifted => R.Instance;

            public static K<M, bool> Check(A value) =>
                M.Pure(R.Check(value));
        }
    }
}
