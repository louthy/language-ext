using LanguageExt.Traits;

namespace LanguageExt;

public static partial class SourceExtensions
{
    extension<A>(K<Source, A> self)
    {
        /// <summary>
        /// Semigroup combine operator: an associative binary operation.
        /// </summary>
        /// <param name="lhs">Left-hand side operand</param>
        /// <param name="rhs">Right-hand side operand</param>
        /// <returns></returns>
        public static Source<A> operator +(K<Source, A> lhs, K<Source, A> rhs) =>
            +lhs.Combine(rhs);

        /// <summary>
        /// Semigroup combine operator: an associative binary operation.
        /// </summary>
        /// <param name="lhs">Left-hand side operand</param>
        /// <param name="rhs">Right-hand side operand</param>
        /// <returns></returns>
        public static Source<A> operator +(K<Source, A> lhs, Pure<A> rhs) =>
            +lhs.Combine(Source.pure(rhs.Value));
    }
}
