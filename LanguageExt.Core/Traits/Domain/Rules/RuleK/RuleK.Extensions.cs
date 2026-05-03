using System;
using LanguageExt;
using LanguageExt.Common;
using LanguageExt.Traits;
using LanguageExt.Traits.Domain;

namespace LanguageExt;

/// <summary>
/// Provides extension methods for working with <see cref="RuleK{SELF, K, A}"/> validations,
/// offering ergonomic overloads for different error construction scenarios.
/// </summary>
public static partial class RuleKExtensions
{
    extension<R1, F, A>(R1)
        where R1 : RuleK<R1, F, A>, new()
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
        /// A successful <see cref="Fin{A}"/> containing the value when valid;
        /// otherwise, a failed <see cref="Fin{A}"/> with the generated error.
        /// </returns>
        public static Fin<K<F, A>> ValidateK(
            K<F, A> value, Func<R1, K<F, A>, Error> Fail) =>
            R1.ValidateK(value, Fail);

        /// <summary>
        /// Validates the specified value using the rule, providing the invalid value
        /// to construct the error.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <param name="Fail">
        /// A function that receives the invalid value and returns an <see cref="Error"/>.
        /// </param>
        /// <returns>
        /// A successful <see cref="Fin{A}"/> containing the value when valid;
        /// otherwise, a failed <see cref="Fin{A}"/> with the generated error.
        /// </returns>
        public static Fin<K<F, A>> ValidateK(K<F, A> value, Func<K<F, A>, Error> Fail) =>
            R1.ValidateK(value, (_, v) => Fail(v));

        /// <summary>
        /// Validates the specified value using the rule, providing a parameterless error factory.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <param name="Fail">
        /// A function that returns an <see cref="Error"/> when validation fails.
        /// </param>
        /// <returns>
        /// A successful <see cref="Fin{A}"/> containing the value when valid;
        /// otherwise, a failed <see cref="Fin{A}"/> with the generated error.
        /// </returns>
        public static Fin<K<F, A>> ValidateK(K<F, A> value, Func<Error> Fail) =>
            R1.ValidateK(value, (_, _) => Fail());

        /// <summary>
        /// Validates the specified value using the rule, returning a constant error
        /// when validation fails.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <param name="Fail">
        /// The <see cref="Error"/> to return when validation fails.
        /// </param>
        /// <returns>
        /// A successful <see cref="Fin{A}"/> containing the value when valid;
        /// otherwise, a failed <see cref="Fin{A}"/> with the provided error.
        /// </returns>
        public static Fin<K<F, A>> ValidateK(K<F, A> value, Error Fail) =>
            R1.ValidateK(value, (_, _) => Fail);
    }
}
