using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class ValidationExtensions
{
    extension<F, A>(K<Validation<F>, A> self)
    {
        /// <summary>
        /// Choice operator.  Usually means if the first argument succeeds, return it, otherwise return the second
        /// argument.
        /// </summary>
        /// <param name="lhs">Left hand side operand</param>
        /// <param name="rhs">Right hand side operand</param>
        /// <returns></returns>
        public static Validation<F, A> operator |(K<Validation<F>, A> lhs, K<Validation<F>, A> rhs) =>
            +lhs.Choose(rhs);

        /// <summary>
        /// Choice operator.  Usually means if the first argument succeeds, return it, otherwise return the second
        /// argument.
        /// </summary>
        /// <param name="lhs">Left hand side operand</param>
        /// <param name="rhs">Right hand side operand</param>
        /// <returns></returns>
        public static Validation<F, A> operator |(K<Validation<F>, A> lhs, Pure<A> rhs) =>
            +lhs.Choose(Validation.SuccessI<F, A>(rhs.Value));
    }
}
