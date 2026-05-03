using System;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt.Traits.Domain;

/// <summary>
/// Defines a reusable domain rule that can validate values of kind <typeparamref name="A"/> on a
/// monadic <typeparamref name="M"/> context.
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
public interface RuleM<SELF, M, A>
    where SELF : RuleM<SELF, M, A>, new()
    where M : Monad<M>
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
    public static abstract K<M, bool> Check(A v);

    /// <summary>
    /// Checks whether the specified value does not satisfy the wrapped rule.
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <returns>
    /// <c>true</c> when the wrapped rule is not satisfied; otherwise, <c>false</c>.
    /// </returns>
    public static virtual FinT<M, A> ValidateM(
        A v, 
        Func<SELF, A, K<M, Error>> Fail) =>
        from followsRule in SELF.Check(v)
        let mResult = followsRule 
            ? FinT.lift<M, A>(Pure(v)) 
            : FinT.lift(Fail(SELF.Instance, v))
                  .Bind(FinT.Fail<M, A>)
        from result in mResult 
        select result;

}
