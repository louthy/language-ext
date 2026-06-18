using LanguageExt.Traits;
namespace LanguageExt;

public static partial class IteratorIOExtensions
{
    extension<A>(K<IteratorIO, A> _) 
    {
        /// <summary>
        /// Semigroup combine operator: an associative binary operation.
        /// </summary>
        /// <param name="lhs">Left-hand side operand</param>
        /// <param name="rhs">Right-hand side operand</param>
        /// <returns></returns>
        public static IteratorIO<A> operator +(K<IteratorIO, A> lhs, K<IteratorIO, A> rhs) =>
            +lhs.Combine(rhs);
        
        /// <summary>
        /// Semigroup combine operator: an associative binary operation.
        /// </summary>
        /// <param name="lhs">Left-hand side operand</param>
        /// <param name="rhs">Right-hand side operand</param>
        /// <returns></returns>
        public static IteratorIO<A> operator +(K<IteratorIO, A> lhs, Pure<A> rhs) =>
            +lhs.Combine(IteratorIO.singleton(rhs.Value));
    }
}
