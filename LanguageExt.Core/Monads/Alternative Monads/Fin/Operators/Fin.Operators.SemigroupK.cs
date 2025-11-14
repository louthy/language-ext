using LanguageExt.Common;
using LanguageExt.Traits;
namespace LanguageExt;

public static partial class FinExtensions
{
    extension<A>(K<Fin, A> _) 
    {
        /// <summary>
        /// Semigroup combine operator: an associative binary operation.
        /// </summary>
        /// <param name="lhs">Left-hand side operand</param>
        /// <param name="rhs">Right-hand side operand</param>
        /// <returns></returns>
        public static Fin<A> operator +(K<Fin, A> lhs, K<Fin, A> rhs) =>
            +lhs.Combine(rhs);
        
        /// <summary>
        /// Semigroup combine operator: an associative binary operation.
        /// </summary>
        /// <param name="lhs">Left-hand side operand</param>
        /// <param name="rhs">Right-hand side operand</param>
        /// <returns></returns>
        public static Fin<A> operator +(K<Fin, A> lhs, Pure<A> rhs) =>
            +lhs.Combine(rhs.ToFin());
        
        /// <summary>
        /// Semigroup combine operator: an associative binary operation.
        /// </summary>
        /// <param name="lhs">Left-hand side operand</param>
        /// <param name="rhs">Right-hand side operand</param>
        /// <returns></returns>
        public static Fin<A> operator +(K<Fin, A> lhs, Fail<Error> rhs) =>
            +lhs.Combine(Fin.Fail<A>(rhs.Value));
        
        /// <summary>
        /// Semigroup combine operator: an associative binary operation.
        /// </summary>
        /// <param name="lhs">Left-hand side operand</param>
        /// <param name="rhs">Right-hand side operand</param>
        /// <returns></returns>
        public static Fin<A> operator +(K<Fin, A> lhs, Error rhs) =>
            +lhs.Combine(Fin.Fail<A>(rhs));
    }
}
