using System;
using LanguageExt.Common;

namespace LanguageExt.Traits.Domain;

/// <summary>
/// Defines a reusable domain rule that can validate values of type <typeparamref name="A"/>.
/// </summary>
/// <typeparam name="SELF">
/// The concrete rule type. Used to enable static abstract members and reusable rule instances.
/// </typeparam>
/// <typeparam name="A">
/// The type of value validated by the rule.
/// </typeparam>
public interface Rule<SELF, A>
    where SELF : Rule<SELF, A>, new()
{
    /// <summary>
    /// Gets the default singleton-like instance of the rule.
    /// </summary>
    public static virtual SELF Instance { get; } = new();

    /// <summary>
    /// Checks whether the specified value satisfies this rule.
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <returns>
    /// <c>true</c> when the value satisfies the rule; otherwise, <c>false</c>.
    /// </returns>
    public static abstract bool Check(A value);

    /// <summary>
    /// Validates the specified value against this rule.
    /// </summary>
    /// <param name="value">The value to validate.</param>
    /// <param name="Fail">
    /// A function that creates the validation error when the value does not satisfy the rule.
    /// Receives the rule instance and the invalid value.
    /// </param>
    /// <returns>
    /// A successful <see cref="Fin{A}"/> containing <paramref name="value"/> when valid;
    /// otherwise, a failed <see cref="Fin{A}"/> containing the generated error.
    /// </returns>
    public static virtual Fin<A> Validate(
        A value, Func<SELF, A, Error> Fail) =>
        SELF.Check(value) ? value : Fail(SELF.Instance, value);

}

