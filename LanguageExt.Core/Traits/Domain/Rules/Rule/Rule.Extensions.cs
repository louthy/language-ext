using System;
using LanguageExt.Common;
using LanguageExt.Traits.Domain;

namespace LanguageExt;

/// <summary>
/// Provides extension methods for working with <see cref="Rule{SELF, A}"/> validations,
/// offering ergonomic overloads for different error construction scenarios.
/// </summary>
public static partial class RuleExtensions
{
    extension<R1, A>(R1)
        where R1 : Rule<R1, A>, new()
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
        public static Fin<A> Validate(A value, Func<R1, A, Error> Fail) =>
            R1.Validate(value, Fail);

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
        public static Fin<A> Validate(A value, Func<A, Error> Fail) =>
            R1.Validate(value, (_, a) => Fail(a));

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
        public static Fin<A> Validate(A value, Func<Error> Fail) =>
            R1.Validate(value, (_, _) => Fail());

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
        public static Fin<A> Validate(A value, Error Fail) =>
            R1.Validate(value, (_, _) => Fail);
    }
}
