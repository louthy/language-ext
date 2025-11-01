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
    [Pure]
    public static virtual A operator +(A lhs, A rhs) =>
        lhs.Combine(rhs);

    /// <summary>
    /// Property that contains the trait in record form.  This allows the trait to be passed
    /// around as a value rather than resolved as a type.  It helps us get around limitations
    /// in the C# constraint system.
    /// </summary>
    [Pure]
    public static virtual SemigroupInstance<A> Instance { get; } =
        new(Combine: Semigroup.combine);
}
