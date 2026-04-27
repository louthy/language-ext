using System;
using System.Collections.Generic;
using System.Text;

namespace LanguageExt.Traits.Domain;

public static partial class RuleK
{
    public class All<R1, R2, M, A>
        : RuleK<All<R1, R2, M, A>, M, A>
        where R1 : RuleK<R1, M, A>, new()
        where R2 : RuleK<R2, M, A>, new()
        where M : Monad<M>
    {
        public R1 First => R1.Instance;

        public R2 Second => R2.Instance;

        public static K<M, bool> Check(A value) =>
            from r1Val in R1.Check(value)
            from r2Val in R2.Check(value)
            select r1Val && r2Val;
}

    public class Any<R1, R2, M, A>
        : RuleK<Any<R1, R2, M, A>, M, A>
        where R1 : RuleK<R1, M, A>, new()
        where R2 : RuleK<R2, M, A>, new()
        where M : Monad<M>
    {
        public R1 First => R1.Instance;
        public R2 Second => R2.Instance;

        public static K<M, bool> Check(A value) =>
            from r1Val in R1.Check(value)
            from r2Val in R2.Check(value)
            select r1Val || r2Val;
    }

    public class Not<R, M, A> : RuleK<Not<R, M, A>, M, A>
        where R : RuleK<R, M, A>, new()
        where M : Monad<M>
    {
        public R NegatedRule => R.Instance;

        public static K<M, bool> Check(A value) =>
            from rVal in R.Check(value)
            select !rVal;
    }


    public class Lift<R, M, A> : RuleK<Lift<R, M, A>, M, A>
        where R : Rule<R, A>, new()
        where M : Monad<M>
    {
        public R Lifted => R.Instance;

        public static K<M, bool> Check(A value) =>
            M.Pure(R.Check(value));
    }
}
