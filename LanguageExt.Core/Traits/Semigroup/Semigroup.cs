using System.Diagnostics.Contracts;

namespace LanguageExt.Traits;

public interface Semigroup<A>
    where A : Semigroup<A>
{
    /// <summary>
    /// An associative binary operation.
    /// </summary>
    /// <param name="this">The first operand to the operation</param>
    /// <param name="rhs">The second operand to the operation</param>
    /// <returns>The result of the operation</returns>
    [Pure]
    public A Combine(A rhs);
    
    /// <summary>
    /// An associative binary operation.
    /// </summary>
    /// <param name="lhs">The first operand to the operation</param>
    /// <param name="rhs">The second operand to the operation</param>
    /// <returns>The result of the operation</returns>
    public static virtual A operator +(A lhs, A rhs) =>
        lhs.Combine(rhs);
}
