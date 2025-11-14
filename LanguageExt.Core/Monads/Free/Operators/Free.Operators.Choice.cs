using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class FreeExtensions
{
    extension<F, A>(K<Free<F>, A> self)
        where F : Functor<F>
    {
        /// <summary>
        /// Choice operator.  Usually means if the first argument succeeds, return it, otherwise return the second
        /// argument.
        /// </summary>
        /// <param name="lhs">Left hand side operand</param>
        /// <param name="rhs">Right hand side operand</param>
        /// <returns></returns>
        public static Free<F, A> operator |(K<Free<F>, A> lhs, K<Free<F>, A> rhs) =>
            +lhs.Choose(rhs);

        /// <summary>
        /// Choice operator.  Usually means if the first argument succeeds, return it, otherwise return the second
        /// argument.
        /// </summary>
        /// <param name="lhs">Left hand side operand</param>
        /// <param name="rhs">Right hand side operand</param>
        /// <returns></returns>
        public static Free<F, A> operator |(K<Free<F>, A> lhs, Pure<A> rhs) =>
            +lhs.Choose(Free.pure<F, A>(rhs.Value));
    }
}
