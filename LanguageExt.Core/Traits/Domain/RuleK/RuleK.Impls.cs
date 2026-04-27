using System;
using System.Collections.Generic;
using System.Text;

namespace LanguageExt.Traits.Domain;

public static partial class RuleK
{
    public class All<R1, R2, F, A>
        : RuleK<All<R1, R2, F, A>, F, A>
        where R1 : RuleK<R1, F, A>, new()
        where R2 : RuleK<R2, F, A>, new()
        where F : Functor<F>
    {
        public R1 First => R1.Instance;

        public R2 Second => R2.Instance;

        public static bool Check(K<F, A> value) =>
            R1.Check(value) && R2.Check(value);
    }

    public class Any<R1, R2, F, A>
        : RuleK<Any<R1, R2, F, A>, F, A>
        where R1 : RuleK<R1, F, A>, new()
        where R2 : RuleK<R2, F, A>, new()
        where F : Functor<F>
    {
        public R1 First => R1.Instance;
        public R2 Second => R2.Instance;

        public static bool Check(K<F, A> value) =>
            R1.Check(value) || R2.Check(value);
    }

    public class Not<R, F, A> : RuleK<Not<R, F, A>, F, A>
        where R : RuleK<R, F, A>, new()
        where F : Functor<F>
    {
        public R NegatedRule => R.Instance;

        public static bool Check(K<F, A> value) =>
            !R.Check(value);
    }
}
