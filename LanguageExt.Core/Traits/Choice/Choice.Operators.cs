using LanguageExt.Traits;

namespace LanguageExt;

public static partial class ChoiceExtensions
{
    extension<F, A>(K<F, A> _)
        where F : Choice<F>
    {
        /// <summary>
        /// Choice operator.  Usually means if the first argument succeeds, return it, otherwise return the second
        /// argument.
        /// </summary>
        /// <param name="lhs">Left hand side operand</param>
        /// <param name="rhs">Right hand side operand</param>
        /// <returns></returns>
        public static K<F, A> operator |(K<F, A> lhs, K<F, A> rhs) =>
            lhs.Choose(rhs);
    }

    extension<F, A>(K<F, A> _)
        where F : Choice<F>, Applicative<F>
    {
        /// <summary>
        /// Choice operator.  Usually means if the first argument succeeds, return it, otherwise return the second
        /// argument.
        /// </summary>
        /// <param name="lhs">Left hand side operand</param>
        /// <param name="rhs">Right hand side operand</param>
        /// <returns></returns>
        public static K<F, A> operator |(K<F, A> lhs, Pure<A> rhs) =>
            lhs.Choose(F.Pure(rhs.Value));
    }
}
