using LanguageExt.Traits;

namespace LanguageExt;

public static partial class WriterTExtensions
{
    extension<W, M, A>(K<WriterT<W, M>, A> self)
        where M : Monad<M>, SemigroupK<M>
        where W : Monoid<W>
    {
        /// <summary>
        /// Semigroup combine operator: an associative binary operation.
        /// </summary>
        /// <param name="lhs">Left-hand side operand</param>
        /// <param name="rhs">Right-hand side operand</param>
        /// <returns></returns>
        public static WriterT<W, M, A> operator +(K<WriterT<W, M>, A> lhs, K<WriterT<W, M>, A> rhs) =>
            new(output => lhs.As().runWriter(output) + rhs.As().runWriter(output));

        /// <summary>
        /// Semigroup combine operator: an associative binary operation.
        /// </summary>
        /// <param name="lhs">Left-hand side operand</param>
        /// <param name="rhs">Right-hand side operand</param>
        /// <returns></returns>
        public static WriterT<W, M, A> operator +(K<WriterT<W, M>, A> lhs, Pure<A> rhs) =>
            new(s => lhs.As().runWriter(s) + M.Pure((rhs.Value, env: s)));
    }
    
    extension<E, W, M, A>(K<WriterT<W, M>, A> self)
        where M : Monad<M>, SemigroupK<M>, Fallible<E, M>
        where W : Monoid<W>
    {
        /// <summary>
        /// Semigroup combine operator: an associative binary operation.
        /// </summary>
        /// <param name="lhs">Left-hand side operand</param>
        /// <param name="rhs">Right-hand side operand</param>
        /// <returns></returns>
        public static WriterT<W, M, A> operator +(K<WriterT<W, M>, A> lhs, Fail<E> rhs) =>
            new(output => lhs.As().runWriter(output) + M.Fail<(A, W)>(rhs.Value));
    }
}
