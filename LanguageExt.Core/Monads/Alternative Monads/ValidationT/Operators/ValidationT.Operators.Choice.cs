using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class ValidationTExtensions
{
    extension<FF, M, A>(K<ValidationT<FF, M>, A> self)
        where M : Monad<M>
    {
        /// <summary>
        /// Choice operator.  Usually means if the first argument succeeds, return it, otherwise return the second
        /// argument.
        /// </summary>
        /// <param name="lhs">Left hand side operand</param>
        /// <param name="rhs">Right hand side operand</param>
        /// <returns></returns>
        public static ValidationT<FF, M, A> operator |(K<ValidationT<FF, M>, A> lhs, K<ValidationT<FF, M>, A> rhs) =>
            +lhs.Choose(rhs);

        /// <summary>
        /// Choice operator.  Usually means if the first argument succeeds, return it, otherwise return the second
        /// argument.
        /// </summary>
        /// <param name="lhs">Left hand side operand</param>
        /// <param name="rhs">Right hand side operand</param>
        /// <returns></returns>
        public static ValidationT<FF, M, A> operator |(K<ValidationT<FF, M>, A> lhs, Pure<A> rhs) =>
            +lhs.Choose(ValidationT.SuccessI<FF, M, A>(rhs.Value));
    }
}
