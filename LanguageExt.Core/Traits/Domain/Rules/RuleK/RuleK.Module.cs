using System.Collections.Generic;

namespace LanguageExt.Traits.Domain;

public static partial class Rule
{
    public static partial class ForK<F, A>
    {
        public class Id<R> : RuleK<Id<R>, F, A>
            where R : RuleK<R, F, A>, new()
        {
            public R Inner => R.Instance;

            public void Deconstruct(out R rule) =>
                rule = Inner;

            public static bool Check(K<F, A> value) =>
                R.Check(value);

        }
        public class All<R1, R2>
            : RuleK<All<R1, R2>, F, A>
            where R1 : RuleK<R1, F, A>, new()
            where R2 : RuleK<R2, F, A>, new()
        {
            public R1 First => R1.Instance;

            public R2 Second => R2.Instance;

            public void Deconstruct(out R1 first, out R2 second) =>
                (first, second) = (First, Second);

            public static bool Check(K<F, A> value) =>
                R1.Check(value) && R2.Check(value);
        }

        public class Any<R1, R2>
            : RuleK<Any<R1, R2>, F, A>
            where R1 : RuleK<R1, F, A>, new()
            where R2 : RuleK<R2, F, A>, new()
        {
            public R1 First => R1.Instance;

            public R2 Second => R2.Instance;

            public void Deconstruct(out R1 first, out R2 second) =>
                (first, second) = (First, Second);

            public static bool Check(K<F, A> value) =>
                R1.Check(value) || R2.Check(value);
        }

        public class Not<R> : RuleK<Not<R>, F, A>
            where R : RuleK<R, F, A>, new()
        {
            public R NegatedRule => R.Instance;

            public void Deconstruct(out R rule) =>
                rule = NegatedRule;

            public static bool Check(K<F, A> value) =>
                !R.Check(value);
        }
    }

}
