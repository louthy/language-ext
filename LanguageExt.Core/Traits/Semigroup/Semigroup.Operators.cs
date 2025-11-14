using LanguageExt.Traits;
namespace LanguageExt;

public static class SemigroupExtensions
{
    extension<A>(A _) 
        where A : Semigroup<A>
    {
        /// <summary>
        /// Semigroup combine operator: an associative binary operation.
        /// </summary>
        /// <param name="lhs">Left-hand side operand</param>
        /// <param name="rhs">Right-hand side operand</param>
        /// <returns></returns>
        public static A operator +(A lhs, A rhs) =>
            lhs.Combine(rhs);
    }
}
