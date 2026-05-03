using System.Collections.Generic;

namespace LanguageExt.Traits.Domain;

/// <summary>
/// Provides rule combinators for composing reusable domain validation rules.
/// </summary>
public static partial class Rule
{
    /// <summary>
    /// Provides rule combinators specialized for values of kind <typeparamref name="F"/> -> <typeparamref name="A"/>.
    /// </summary>
    /// <typeparam name="A">The type of value validated by the composed rules.</typeparam>
    public static partial class ForK<F, A>
    {
        /// <summary>
        /// Identity rule combinator that delegates validation to another rule.
        /// </summary>
        /// <typeparam name="R">The wrapped rule type.</typeparam>
        public class Id<R> : RuleK<Id<R>, F, A>
            where R : RuleK<R, F, A>, new()
        {
            /// <summary>
            /// Gets the wrapped rule instance.
            /// </summary>
            public R Inner => R.Instance;

            /// <summary>
            /// Deconstructs this combinator into its wrapped rule.
            /// </summary>
            /// <param name="rule">The wrapped rule instance.</param>
            public void Deconstruct(out R rule) =>
                rule = R.Instance;

            /// <summary>
            /// Checks whether the specified value satisfies the wrapped rule.
            /// </summary>
            /// <param name="value">The value to check.</param>
            /// <returns>
            /// <c>true</c> when the value satisfies the wrapped rule; otherwise, <c>false</c>.
            /// </returns>
            public static bool Check(K<F, A> value) =>
                R.Check(value);

        }

        /// <summary>
        /// Rule combinator that succeeds only when both composed rules succeed.
        /// </summary>
        /// <typeparam name="R1">The first rule type.</typeparam>
        /// <typeparam name="R2">The second rule type.</typeparam>
        public class All<R1, R2>
            : RuleK<All<R1, R2>, F, A>
            where R1 : RuleK<R1, F, A>, new()
            where R2 : RuleK<R2, F, A>, new()
        {
            /// <summary>
            /// Gets the first composed rule instance.
            /// </summary>
            public R1 First => R1.Instance;

            /// <summary>
            /// Gets the second composed rule instance.
            /// </summary>
            public R2 Second => R2.Instance;

            /// <summary>
            /// Deconstructs this combinator into its composed rules.
            /// </summary>
            /// <param name="rule1">The first rule instance.</param>
            /// <param name="rule2">The second rule instance.</param>
            public void Deconstruct(out R1 rule1, out R2 rule2) =>
                (rule1, rule2) = (R1.Instance, R2.Instance);

            /// <summary>
            /// Checks whether the specified value satisfies both composed rules.
            /// </summary>
            /// <param name="value">The value to check.</param>
            /// <returns>
            /// <c>true</c> when both rules are satisfied; otherwise, <c>false</c>.
            /// </returns>
            public static bool Check(K<F, A> value) =>
                R1.Check(value) && R2.Check(value);
        }

        /// <summary>
        /// Rule combinator that succeeds when at least one of the composed rules succeeds.
        /// </summary>
        /// <typeparam name="R1">The first rule type.</typeparam>
        /// <typeparam name="R2">The second rule type.</typeparam>
        public class Any<R1, R2>
            : RuleK<Any<R1, R2>, F, A>
            where R1 : RuleK<R1, F, A>, new()
            where R2 : RuleK<R2, F, A>, new()
        {
            /// <summary>
            /// Gets the first composed rule instance.
            /// </summary>
            public R1 First => R1.Instance;

            /// <summary>
            /// Gets the second composed rule instance.
            /// </summary>
            public R2 Second => R2.Instance;

            /// <summary>
            /// Deconstructs this combinator into its composed rules.
            /// </summary>
            /// <param name="rule1">The first rule instance.</param>
            /// <param name="rule2">The second rule instance.</param>
            public void Deconstruct(out R1 rule1, out R2 rule2) =>
                (rule1, rule2) = (R1.Instance, R2.Instance);

            /// <summary>
            /// Checks whether the specified value satisfies at least one composed rule.
            /// </summary>
            /// <param name="value">The value to check.</param>
            /// <returns>
            /// <c>true</c> when either rule is satisfied; otherwise, <c>false</c>.
            /// </returns>
            public static bool Check(K<F, A> value) =>
                R1.Check(value) || R2.Check(value);
        }

        /// <summary>
        /// Rule combinator that succeeds when the wrapped rule fails.
        /// </summary>
        /// <typeparam name="R">The negated rule type.</typeparam>
        public class Not<R> : RuleK<Not<R>, F, A>
            where R : RuleK<R, F, A>, new()
        {
            /// <summary>
            /// Gets the negated rule instance.
            /// </summary>
            public R NegatedRule => R.Instance;

            /// <summary>
            /// Deconstructs this combinator into its negated rule.
            /// </summary>
            /// <param name="rule">The negated rule instance.</param>
            public void Deconstruct(out R rule) =>
                rule = R.Instance;

            /// <summary>
            /// Checks whether the specified value does not satisfy the wrapped rule.
            /// </summary>
            /// <param name="value">The value to check.</param>
            /// <returns>
            /// <c>true</c> when the wrapped rule is not satisfied; otherwise, <c>false</c>.
            /// </returns>
            public static bool Check(K<F, A> value) =>
                !R.Check(value);
        }
    }

}
