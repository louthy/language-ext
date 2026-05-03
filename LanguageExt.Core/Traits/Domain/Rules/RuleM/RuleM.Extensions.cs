using System;
using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt.Traits.Domain;

/// <summary>
/// Provides extension methods for working with <see cref="RuleM{SELF, M, A}"/> validations,
/// offering ergonomic overloads for different error construction scenarios.
/// </summary>
public static partial class RuleMExtensions
{
    extension<SELF, M, A>(SELF)
        where SELF : RuleM<SELF, M, A>, new()
        where M : Monad<M>
    {
        /// <summary>
        /// Validates the specified value using the rule, providing full access to the rule instance
        /// and the value when constructing the error.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <param name="Fail">
        /// A function that receives the rule instance and the invalid value to produce an <see cref="Error"/>.
        /// </param>
        /// <returns>
        /// A successful <see cref="FinT{M, A}"/> containing the value when valid;
        /// otherwise, a failed <see cref="FinT{M, A}"/> with the generated error.
        /// </returns>
        public static FinT<M, A> ValidateM(A value, Func<SELF, A, K<M, Error>> Fail) =>
            SELF.ValidateM(value, Fail);

        /// <summary>
        /// Validates the specified value using the rule, providing the invalid value
        /// to construct the error.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <param name="Fail">
        /// A function that receives the invalid value and returns an <see cref="Error"/>.
        /// </param>
        /// <returns>
        /// A successful <see cref="FinT{M, A}"/> containing the value when valid;
        /// otherwise, a failed <see cref="FinT{M, A}"/> with the generated error.
        /// </returns>
        public static FinT<M, A> ValidateM(A value, Func< A, K<M, Error>> Fail) =>
            SELF.ValidateM(value, (_, v) => Fail(v));

        /// <summary>
        /// Validates the specified value using the rule, providing a parameterless error factory.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <param name="Fail">
        /// A function that returns an <see cref="Error"/> when validation fails.
        /// </param>
        /// <returns>
        /// A successful <see cref="FinT{M, A}"/> containing the value when valid;
        /// otherwise, a failed <see cref="FinT{M, A}"/> with the generated error.
        /// </returns>
        public static FinT<M, A> ValidateM(A value, Func<K<M, Error>> Fail) =>
            SELF.ValidateM(value, (_, _) => Fail());

        /// <summary>
        /// Validates the specified value using the rule, returning a constant error
        /// when validation fails.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <param name="Fail">
        /// The <see cref="Error"/> to return when validation fails.
        /// </param>
        /// <returns>
        /// A successful <see cref="FinT{M, A}"/> containing the value when valid;
        /// otherwise, a failed <see cref="FinT{M, A}"/> with the provided error.
        /// </returns>
        public static FinT<M, A> ValidateM(A value, K<M, Error> Fail) =>
            SELF.ValidateM(value, (_, _) => Fail);

    }

}
