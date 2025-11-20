using LanguageExt.Traits;

namespace LanguageExt;

public static partial class ValidationExtensions
{
    extension<F, A>(K<Validation<F>, A> self)
        where F : Semigroup<F>
    {
        /// <summary>
        /// Semigroup combine operator: an associative binary operation.
        /// </summary>
        /// <param name="lhs">Left-hand side operand</param>
        /// <param name="rhs">Right-hand side operand</param>
        /// <returns></returns>
        public static Validation<F, A> operator +(K<Validation<F>, A> lhs, K<Validation<F>, A> rhs) =>
            +lhs.Combine(rhs);

        /// <summary>
        /// Semigroup combine operator: an associative binary operation.
        /// </summary>
        /// <param name="lhs">Left-hand side operand</param>
        /// <param name="rhs">Right-hand side operand</param>
        /// <returns></returns>
        public static Validation<F, A> operator +(K<Validation<F>, A> lhs, Pure<A> rhs) =>
            +lhs.Combine(Validation.SuccessI<F, A>(rhs.Value));

        /// <summary>
        /// Semigroup combine operator: an associative binary operation.
        /// </summary>
        /// <param name="lhs">Left-hand side operand</param>
        /// <param name="rhs">Right-hand side operand</param>
        /// <returns></returns>
        public static Validation<F, A> operator +(K<Validation<F>, A> lhs, Fail<F> rhs) =>
            +lhs.Combine(Validation.FailI<F, A>(rhs.Value));

        /// <summary>
        /// Semigroup combine operator: an associative binary operation.
        /// </summary>
        /// <param name="lhs">Left-hand side operand</param>
        /// <param name="rhs">Right-hand side operand</param>
        /// <returns></returns>
        public static Validation<F, A> operator +(K<Validation<F>, A> lhs, F rhs) =>
            +lhs.Combine(Validation.FailI<F, A>(rhs));
    }
}
