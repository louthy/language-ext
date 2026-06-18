using LanguageExt.Traits;
namespace LanguageExt;

public static partial class IteratorExtensions
{
    extension<A>(K<Iterator, A> _) 
    {
        /// <summary>
        /// Semigroup combine operator: an associative binary operation.
        /// </summary>
        /// <param name="lhs">Left-hand side operand</param>
        /// <param name="rhs">Right-hand side operand</param>
        /// <returns></returns>
        public static Iterator<A> operator +(K<Iterator, A> lhs, K<Iterator, A> rhs) =>
            +lhs.Combine(rhs);
        
        /// <summary>
        /// Semigroup combine operator: an associative binary operation.
        /// </summary>
        /// <param name="lhs">Left-hand side operand</param>
        /// <param name="rhs">Right-hand side operand</param>
        /// <returns></returns>
        public static Iterator<A> operator +(K<Iterator, A> lhs, Pure<A> rhs) =>
            +lhs.Combine(Iterator.singleton(rhs.Value));
    }
}
