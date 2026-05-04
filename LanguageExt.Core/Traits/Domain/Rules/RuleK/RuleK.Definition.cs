using System;
using LanguageExt.Common;

namespace LanguageExt.Traits.Domain;

/// <summary>
/// Defines a reusable domain rule that can validate values of kind <typeparamref name="F"/> -> <typeparamref name="A"/>.
/// </summary>
/// <typeparam name="SELF">
/// The concrete rule type. Used to enable static abstract members and reusable rule instances.
/// </typeparam>
/// <typeparam name="A">
/// The kind validated by the rule.
/// </typeparam>
/// <typeparam name="A">
/// The type of value validated by the rule.
/// </typeparam>
public interface RuleK<SELF, F, A>
    where SELF : RuleK<SELF, F, A>, new()
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
    static abstract bool Check(K<F, A> value);

    /// <summary>
    /// Checks whether the specified value does not satisfy the wrapped rule.
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <returns>
    /// <c>true</c> when the wrapped rule is not satisfied; otherwise, <c>false</c>.
    /// </returns>
    public static virtual Fin<K<F, A>> ValidateK(K<F, A> value, Func<SELF, K<F, A>, Error> Fail) =>
        SELF.Check(value) ? Prelude.Pure(value) : Fail(SELF.Instance, value);
}

