using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class IOExtensions
{
    extension<A>(K<IO, A> self)
    {
        /// <summary>
        /// Semigroup combine operator: an associative binary operation.
        /// </summary>
        /// <param name="lhs">Left-hand side operand</param>
        /// <param name="rhs">Right-hand side operand</param>
        /// <returns></returns>
        public static IO<A> operator +(K<IO, A> lhs, K<IO, A> rhs) =>
            +lhs.Combine(rhs);

        /// <summary>
        /// Semigroup combine operator: an associative binary operation.
        /// </summary>
        /// <param name="lhs">Left-hand side operand</param>
        /// <param name="rhs">Right-hand side operand</param>
        /// <returns></returns>
        public static IO<A> operator +(K<IO, A> lhs, Pure<A> rhs) =>
            +lhs.Combine(IO.pure(rhs.Value));

        /// <summary>
        /// Semigroup combine operator: an associative binary operation.
        /// </summary>
        /// <param name="lhs">Left-hand side operand</param>
        /// <param name="rhs">Right-hand side operand</param>
        /// <returns></returns>
        public static IO<A> operator +(K<IO, A> lhs, Fail<Error> rhs) =>
            +lhs.Combine(IO.fail<A>(rhs.Value));

        /// <summary>
        /// Semigroup combine operator: an associative binary operation.
        /// </summary>
        /// <param name="lhs">Left-hand side operand</param>
        /// <param name="rhs">Right-hand side operand</param>
        /// <returns></returns>
        public static IO<A> operator +(K<IO, A> lhs, Error rhs) =>
            +lhs.Combine(IO.fail<A>(rhs));
    }
}
