using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class ChronicleTExtensions
{
    extension<M, A>(K<OptionT<M>, A> self)
        where M : Monad<M>, SemigroupK<M>
    {
        /// <summary>
        /// Semigroup combine operator: an associative binary operation.
        /// </summary>
        /// <param name="lhs">Left-hand side operand</param>
        /// <param name="rhs">Right-hand side operand</param>
        /// <returns></returns>
        public static OptionT<M, A> operator +(K<OptionT<M>, A> lhs, K<OptionT<M>, A> rhs) =>
            new (lhs.As().runOption + rhs.As().runOption);
    }

    extension<M, A>(K<OptionT<M>, A> self)
        where M : Monad<M>, SemigroupK<M>, Applicative<M>
    {
        /// <summary>
        /// Semigroup combine operator: an associative binary operation.
        /// </summary>
        /// <param name="lhs">Left-hand side operand</param>
        /// <param name="rhs">Right-hand side operand</param>
        /// <returns></returns>
        public static OptionT<M, A> operator +(K<OptionT<M>, A> lhs, Pure<A> rhs) =>
            new(lhs.As().runOption + M.Pure(rhs.Value).Map(Option.Some));
    }
    
    extension<E, M, A>(K<OptionT<M>, A> self)
        where M : Monad<M>, SemigroupK<M>, Fallible<E, M>
    {
        /// <summary>
        /// Semigroup combine operator: an associative binary operation.
        /// </summary>
        /// <param name="lhs">Left-hand side operand</param>
        /// <param name="rhs">Right-hand side operand</param>
        /// <returns></returns>
        public static OptionT<M, A> operator +(K<OptionT<M>, A> lhs, Fail<E> rhs) =>
            new (lhs.As().runOption + M.Fail<Option<A>>(rhs.Value));

    }    
}
