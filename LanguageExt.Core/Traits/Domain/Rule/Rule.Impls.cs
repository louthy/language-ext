namespace LanguageExt.Traits.Domain;

public static partial class Rule
{
    public class All<R1, R2, A>
        : Rule<All<R1, R2, A>, A>
        where R1 : Rule<R1, A>, new()
        where R2 : Rule<R2, A>, new()
    {
        public R1 First => R1.Instance;

        public R2 Second => R2.Instance;

        public static bool Check(A value) =>
            R1.Check(value) && R2.Check(value);
    }

    public class Any<R1, R2, A>
        : Rule<Any<R1, R2, A>, A>
        where R1 : Rule<R1, A>, new()
        where R2 : Rule<R2, A>, new()
    {
        public R1 First => R1.Instance;
        public R2 Second => R2.Instance;

        public static bool Check(A value) =>
            R1.Check(value) || R2.Check(value);
    }

    public class Not<R, A> : Rule<Not<R, A>, A>
        where R : Rule<R, A>, new()
    {
        public R NegatedRule => R.Instance;
        public static bool Check(A value) =>
            !R.Check(value);
    }

}
