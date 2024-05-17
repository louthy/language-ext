using System.Diagnostics.Contracts;

namespace LanguageExt.Traits;

/// <summary>
/// Equivalent of semigroups for working with higher-kinded types
/// </summary>
/// <typeparam name="M">Higher kind</typeparam>
public interface SemigroupK<M>
    where M : SemigroupK<M>
{
    /// <summary>
    /// An associative binary operation.
    /// </summary>
    /// <param name="lhs">The first operand to the operation</param>
    /// <param name="rhs">The second operand to the operation</param>
    /// <returns>The result of the operation</returns>
    [Pure]
    public static abstract K<M, A> Combine<A>(K<M, A> lhs, K<M, A> rhs);
}
