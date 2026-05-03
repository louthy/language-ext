using System;
using LanguageExt.Common;

namespace LanguageExt.Traits.Domain;


/// <summary>
/// Defines a reusable domain rule that can validate values of kind <typeparamref name="A"/> on a
/// monad-transformation <typeparamref name="T"/> -> <typeparamref name="M"/> context.
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
public interface RuleT<SELF, T, M, A>
    where SELF : RuleT<SELF, T, M, A>, new()
    where T : MonadT<T, M>
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
    static abstract K<T, bool> Check(K<M, A> ma);

    /// <summary>
    /// Checks whether the specified value satisfies this rule.
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <returns>
    /// <c>true</c> when the value satisfies the rule; otherwise, <c>false</c>.
    /// </returns>
    public static virtual K<T, bool> Check(A a) =>
        SELF.Check(M.Pure(a));

    /// <summary>
    /// Checks whether the specified value does not satisfy the wrapped rule.
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <returns>
    /// <c>true</c> when the wrapped rule is not satisfied; otherwise, <c>false</c>.
    /// </returns>
    public static virtual FinT<T, A> ValidateT(K<M, A> ma, Func<SELF, A, K<T, Error>> Fail) =>
        from value in T.Lift(ma)
        from followsRule in SELF.Check(value)
        let mResult = followsRule
            ? FinT.lift(T.Pure(value)) 
            : FinT.lift(Fail(SELF.Instance, value))
                  .Bind(FinT.Fail<T, A>)
        from result in mResult
        select result;
}
