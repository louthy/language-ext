namespace LanguageExt.Traits.Domain;

public static partial class Rule
{
    public static partial class For<A>
    {
        public class Id<R> : Rule<Id<R>, A>
            where R : Rule<R, A>, new()
        {
            public R InnerRule => R.Instance;

            public void Deconstruct(out R rule) =>
                rule = R.Instance;
            public static bool Check(A value) =>
                R.Check(value);
        }

        public class All<R1, R2>
            : Rule<All<R1, R2>, A>
            where R1 : Rule<R1, A>, new()
            where R2 : Rule<R2, A>, new()
        {
            public R1 First => R1.Instance;

            public R2 Second => R2.Instance;

            public void Deconstruct(out R1 rule1, out R2 rule2) =>
                (rule1, rule2) = (R1.Instance, R2.Instance);

            public static bool Check(A value) =>
                R1.Check(value) && R2.Check(value);
        }

        public class Any<R1, R2> : Rule<Any<R1, R2>, A>
            where R1 : Rule<R1, A>, new()
            where R2 : Rule<R2, A>, new()
        {
            public R1 First => R1.Instance;

            public R2 Second => R2.Instance;

            public void Deconstruct(out R1 rule1, out R2 rule2) =>
                (rule1, rule2) = (R1.Instance, R2.Instance);

            public static bool Check(A value) =>
                R1.Check(value) || R2.Check(value);
        }

        public class Not<R> : Rule<Not<R>, A>
            where R : Rule<R, A>, new()
        {
            public R NegatedRule => R.Instance;

            public void Deconstruct(out R rule) =>
                rule = R.Instance;

            public static bool Check(A value) =>
                !R.Check(value);
        }
    }
}
