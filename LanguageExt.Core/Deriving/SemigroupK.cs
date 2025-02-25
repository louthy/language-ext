using LanguageExt.Traits;

namespace LanguageExt;

public static partial class Deriving
{
    /// <summary>
    /// Derived equivalent of semigroups for working with higher-kinded types
    /// </summary>
    public interface SemigroupK<Supertype, Subtype> :
        SemigroupK<Supertype>,
        Natural<Supertype, Subtype>,
        CoNatural<Supertype, Subtype>
        where Supertype : SemigroupK<Supertype, Subtype>
        where Subtype : SemigroupK<Subtype>
    {
        /// <summary>
        /// An associative binary operation.
        /// </summary>
        /// <param name="lhs">The first operand to the operation</param>
        /// <param name="rhs">The second operand to the operation</param>
        /// <returns>The result of the operation</returns>
        static K<Supertype, A> SemigroupK<Supertype>.Combine<A>(K<Supertype, A> lhs, K<Supertype, A> rhs) => 
            Supertype.CoTransform(Supertype.Transform(lhs).Combine(Supertype.Transform(rhs)));
    }
}

