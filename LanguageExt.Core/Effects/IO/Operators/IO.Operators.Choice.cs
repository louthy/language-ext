using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class IOExtensions
{
    extension<A>(K<IO, A> self)
    {
        /// <summary>
        /// Choice operator.  Usually means if the first argument succeeds, return it, otherwise return the second
        /// argument.
        /// </summary>
        /// <param name="lhs">Left hand side operand</param>
        /// <param name="rhs">Right hand side operand</param>
        /// <returns></returns>
        public static IO<A> operator |(K<IO, A> lhs, K<IO, A> rhs) =>
            +lhs.Choose(rhs);

        /// <summary>
        /// Choice operator.  Usually means if the first argument succeeds, return it, otherwise return the second
        /// argument.
        /// </summary>
        /// <param name="lhs">Left hand side operand</param>
        /// <param name="rhs">Right hand side operand</param>
        /// <returns></returns>
        public static IO<A> operator |(K<IO, A> lhs, Pure<A> rhs) =>
            +lhs.Choose(rhs.ToIO());
    }
}
