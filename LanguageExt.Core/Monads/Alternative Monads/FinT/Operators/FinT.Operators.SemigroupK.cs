using LanguageExt.Common;
using LanguageExt.Traits;
namespace LanguageExt;

public static partial class FinExtensions
{
    extension<M, A>(K<FinT<M>, A> _)
        where M : Monad<M>
    {
        /// <summary>
        /// Semigroup combine operator: an associative binary operation.
        /// </summary>
        /// <param name="lhs">Left-hand side operand</param>
        /// <param name="rhs">Right-hand side operand</param>
        /// <returns></returns>
        public static FinT<M, A> operator +(K<FinT<M>, A> lhs, K<FinT<M>, A> rhs) =>
            +lhs.Combine(rhs);

        /// <summary>
        /// Semigroup combine operator: an associative binary operation.
        /// </summary>
        /// <param name="lhs">Left-hand side operand</param>
        /// <param name="rhs">Right-hand side operand</param>
        /// <returns></returns>
        public static FinT<M, A> operator +(K<FinT<M>, A> lhs, Pure<A> rhs) =>
            +lhs.Combine(FinT.lift<M, A>(rhs.ToFin()));

        /// <summary>
        /// Semigroup combine operator: an associative binary operation.
        /// </summary>
        /// <param name="lhs">Left-hand side operand</param>
        /// <param name="rhs">Right-hand side operand</param>
        /// <returns></returns>
        public static FinT<M, A> operator +(K<FinT<M>, A> lhs, Fail<Error> rhs) =>
            +lhs.Combine(FinT.Fail<M, A>(rhs.Value));

        /// <summary>
        /// Semigroup combine operator: an associative binary operation.
        /// </summary>
        /// <param name="lhs">Left-hand side operand</param>
        /// <param name="rhs">Right-hand side operand</param>
        /// <returns></returns>
        public static FinT<M, A> operator +(K<FinT<M>, A> lhs, Error rhs) =>
            +lhs.Combine(FinT.Fail<M, A>(rhs));
    }
}
