using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class ValidationTExtensions
{
    extension<FF, M, A>(K<ValidationT<FF, M>, A> self)
        where M : Monad<M>
        where FF : Semigroup<FF>
    {
        /// <summary>
        /// Semigroup combine operator: an associative binary operation.
        /// </summary>
        /// <param name="lhs">Left-hand side operand</param>
        /// <param name="rhs">Right-hand side operand</param>
        /// <returns></returns>
        public static ValidationT<FF, M, A> operator +(K<ValidationT<FF, M>, A> lhs, K<ValidationT<FF, M>, A> rhs) =>
            +lhs.Combine(rhs);

        /// <summary>
        /// Semigroup combine operator: an associative binary operation.
        /// </summary>
        /// <param name="lhs">Left-hand side operand</param>
        /// <param name="rhs">Right-hand side operand</param>
        /// <returns></returns>
        public static ValidationT<FF, M, A> operator +(K<ValidationT<FF, M>, A> lhs, Pure<A> rhs) =>
            +lhs.Combine(ValidationT.SuccessI<FF, M, A>(rhs.Value));

        /// <summary>
        /// Semigroup combine operator: an associative binary operation.
        /// </summary>
        /// <param name="lhs">Left-hand side operand</param>
        /// <param name="rhs">Right-hand side operand</param>
        /// <returns></returns>
        public static ValidationT<FF, M, A> operator +(K<ValidationT<FF, M>, A> lhs, Fail<FF> rhs) =>
            +lhs.Combine(ValidationT.FailI<FF, M, A>(rhs.Value));

        /// <summary>
        /// Semigroup combine operator: an associative binary operation.
        /// </summary>
        /// <param name="lhs">Left-hand side operand</param>
        /// <param name="rhs">Right-hand side operand</param>
        /// <returns></returns>
        public static ValidationT<FF, M, A> operator +(K<ValidationT<FF, M>, A> lhs, FF rhs) =>
            +lhs.Combine(ValidationT.FailI<FF, M, A>(rhs));
    }
}
