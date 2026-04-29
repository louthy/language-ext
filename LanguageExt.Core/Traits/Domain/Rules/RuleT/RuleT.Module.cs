using System.Collections.Generic;

namespace LanguageExt.Traits.Domain;

public static partial class RuleT<T, M>
    where T : MonadT<T, M>
    where M : Monad<M>
{
    public static partial class For<A>
    {
        public class Id<R> : RuleT<Id<R>, T, M, A>
            where R : RuleT<R, T, M, A>, new()
        {
            public R Inner => R.Instance;

            public static K<T, bool> Check(K<M, A> value) =>
                R.Check(value);
        }

        public class All<R1, R2>
            : RuleT<All<R1, R2>, T, M, A>
            where R1 : RuleT<R1, T, M, A>, new()
            where R2 : RuleT<R2, T, M, A>, new()
        {
            public R1 First => R1.Instance;

            public R2 Second => R2.Instance;

            public void Deconstruct(out R1 first, out R2 second) =>
                (first, second) = (First, Second);

            public static K<T, bool> Check(K<M, A> value) =>
                from v in T.Lift(value)
                from r1Val in R1.Check(M.Pure(v))
                from r2Val in R2.Check(M.Pure(v))
                select r1Val && r2Val;

        }

        public class Any<R1, R2>
            : RuleT<Any<R1, R2>, T, M, A>
            where R1 : RuleT<R1, T, M, A>, new()
            where R2 : RuleT<R2, T, M, A>, new()
        {
            public R1 First => R1.Instance;

            public R2 Second => R2.Instance;

            public void Deconstruct(out R1 first, out R2 second) =>
                (first, second) = (First, Second);

            public static K<T, bool> Check(K<M, A> value) =>
                from v in T.Lift(value)
                from r1Val in R1.Check(M.Pure(v))
                from r2Val in R2.Check(M.Pure(v))
                select r1Val || r2Val;
        }

        public class Not<R> : RuleT<Not<R>, T, M, A>
            where R : RuleT<R, T, M, A>, new()
        {
            public R NegatedRule => R.Instance;

            public void Deconstruct(out R rule) =>
                rule = NegatedRule;

            public static K<T, bool> Check(K<M, A> value) =>
                from rVal in R.Check(value)
                select !rVal;
        }

        public class LiftT<R> : RuleT<LiftT<R>, T, M, A>
            where R : RuleK<R, M, A>, new()
        {
            public R Lifted => R.Instance;

            public void Deconstruct(out R rule) =>
                rule = Lifted;

            public static K<T, bool> Check(K<M, A> value) =>
                T.Pure(R.Check(value));
        }
    }
}
