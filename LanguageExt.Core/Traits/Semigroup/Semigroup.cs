using System.Diagnostics.Contracts;

namespace LanguageExt.TypeClasses;

public interface Semigroup<A> : Trait
    where A : Semigroup<A>
{
    /// <summary>
    /// An associative binary operation.
    /// </summary>
    /// <param name="this">The first operand to the operation</param>
    /// <param name="y">The second operand to the operation</param>
    /// <returns>The result of the operation</returns>
    [Pure]
    public A Append(A y);
    
    /// <summary>
    /// An associative binary operation.
    /// </summary>
    /// <param name="lhs">The first operand to the operation</param>
    /// <param name="rhs">The second operand to the operation</param>
    /// <returns>The result of the operation</returns>
    public static virtual A operator +(A lhs, A rhs) =>
        lhs.Append(rhs);
}
