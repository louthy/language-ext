using System.Collections.Generic;

namespace LanguageExt.Traits.Domain;

/// <summary>
/// Provides rule combinators for composing reusable domain validation rules on
/// monadic transformer <typeparamref name="T"/> -> <typeparamref name="M"/> context.
/// </summary>
public static partial class RuleT<T, M>
    where T : MonadT<T, M>
    where M : Monad<M>
{
    /// <summary>
    /// Provides rule combinators specialized for values of kind <typeparamref name="T"/>
    /// -> <typeparamref name="M"/> -> <typeparamref name="A"/>.
    /// </summary>
    /// <typeparam name="A">The type of value validated by the composed rules.</typeparam>
    public static partial class For<A>
    {
        /// <summary>
        /// Identity rule combinator that delegates validation to another rule.
        /// </summary>
        /// <typeparam name="R">The wrapped rule type.</typeparam>
        public class Id<R> : RuleT<Id<R>, T, M, A>
            where R : RuleT<R, T, M, A>, new()
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
            public static K<T, bool> Check(K<M, A> value) =>
                R.Check(value);
        }

        /// <summary>
        /// Rule combinator that succeeds only when both composed rules succeed.
        /// </summary>
        /// <typeparam name="R1">The first rule type.</typeparam>
        /// <typeparam name="R2">The second rule type.</typeparam>
        public class All<R1, R2>
            : RuleT<All<R1, R2>, T, M, A>
            where R1 : RuleT<R1, T, M, A>, new()
            where R2 : RuleT<R2, T, M, A>, new()
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
            public static K<T, bool> Check(K<M, A> value) =>
                from v in T.Lift(value)
                from r1Val in R1.Check(M.Pure(v))
                from r2Val in R2.Check(M.Pure(v))
                select r1Val && r2Val;

        }

        /// <summary>
        /// Rule combinator that succeeds when at least one of the composed rules succeeds.
        /// </summary>
        /// <typeparam name="R1">The first rule type.</typeparam>
        /// <typeparam name="R2">The second rule type.</typeparam>
        public class Any<R1, R2>
            : RuleT<Any<R1, R2>, T, M, A>
            where R1 : RuleT<R1, T, M, A>, new()
            where R2 : RuleT<R2, T, M, A>, new()
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
            public static K<T, bool> Check(K<M, A> value) =>
                from v in T.Lift(value)
                from r1Val in R1.Check(M.Pure(v))
                from r2Val in R2.Check(M.Pure(v))
                select r1Val || r2Val;
        }

        /// <summary>
        /// Rule combinator that succeeds when the wrapped rule fails.
        /// </summary>
        /// <typeparam name="R">The negated rule type.</typeparam>
        public class Not<R> : RuleT<Not<R>, T, M, A>
            where R : RuleT<R, T, M, A>, new()
        {
            /// <summary>
            /// Gets the negated rule instance.
            /// </summary>
            public R Negated => R.Instance;

            /// <summary>
            /// Deconstructs this combinator into its negated rule.
            /// </summary>
            /// <param name="rule">The negated rule instance.</param>
            public void Deconstruct(out R rule) =>
                rule = Negated;

            /// <summary>
            /// Checks whether the specified value does not satisfy the wrapped rule.
            /// </summary>
            /// <param name="value">The value to check.</param>
            /// <returns>
            /// <c>true</c> when the wrapped rule is not satisfied; otherwise, <c>false</c>.
            /// </returns>
            public static K<T, bool> Check(K<M, A> value) =>
                from rVal in R.Check(value)
                select !rVal;
        }

        /// <summary>
        /// Identity rule lifting combinator that lift's a <see cref="Rule{R, A}" /> to be executed
        /// on a monadic <typeparamref name="M"/> context.
        /// </summary>
        public class LiftT<R> : RuleT<LiftT<R>, T, M, A>
            where R : RuleK<R, M, A>, new()
        {
            /// <summary>
            /// Gets the lifted rule instance.
            /// </summary>
            public R Lifted => R.Instance;

            /// <summary>
            /// Deconstructs this combinator into its negated rule.
            /// </summary>
            /// <param name="rule">The negated rule instance.</param>
            public void Deconstruct(out R rule) =>
                rule = Lifted;

            /// <summary>
            /// Checks whether the specified value does not satisfy the wrapped rule.
            /// </summary>
            /// <param name="value">The value to check.</param>
            /// <returns>
            /// <c>true</c> when the wrapped rule is not satisfied; otherwise, <c>false</c>.
            /// </returns>
            public static K<T, bool> Check(K<M, A> value) =>
                T.Pure(R.Check(value));
        }
    }
}
