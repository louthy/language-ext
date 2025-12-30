using LanguageExt.Traits;

namespace LanguageExt;

public static partial class SourceExtensions
{
    extension<M, A>(K<SourceT<M>, A> self)
        where M : MonadIO<M>, Alternative<M>
    {
        /// <summary>
        /// Semigroup combine operator: an associative binary operation.
        /// </summary>
        /// <param name="lhs">Left-hand side operand</param>
        /// <param name="rhs">Right-hand side operand</param>
        /// <returns></returns>
        public static SourceT<M, A> operator +(K<SourceT<M>, A> lhs, K<SourceT<M>, A> rhs) =>
            +lhs.Combine(rhs);

        /// <summary>
        /// Semigroup combine operator: an associative binary operation.
        /// </summary>
        /// <param name="lhs">Left-hand side operand</param>
        /// <param name="rhs">Right-hand side operand</param>
        /// <returns></returns>
        public static SourceT<M, A> operator +(K<SourceT<M>, A> lhs, Pure<A> rhs) =>
            +lhs.Combine(SourceT.pure<M, A>(rhs.Value));
    }
}
