using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class ChronicleTExtensions
{
    extension<Ch, M, A>(K<ChronicleT<Ch, M>, A> self)
        where M : Monad<M>, SemigroupK<M>
    {
        /// <summary>
        /// Semigroup combine operator: an associative binary operation.
        /// </summary>
        /// <param name="lhs">Left-hand side operand</param>
        /// <param name="rhs">Right-hand side operand</param>
        /// <returns></returns>
        public static ChronicleT<Ch, M, A> operator +(K<ChronicleT<Ch, M>, A> lhs, K<ChronicleT<Ch, M>, A> rhs) =>
            new (semi => lhs.As().runChronicleT(semi) + rhs.As().runChronicleT(semi));
    }

    extension<Ch, M, A>(K<ChronicleT<Ch, M>, A> self)
        where M : Monad<M>, SemigroupK<M>, Applicative<M>
    {
        /// <summary>
        /// Semigroup combine operator: an associative binary operation.
        /// </summary>
        /// <param name="lhs">Left-hand side operand</param>
        /// <param name="rhs">Right-hand side operand</param>
        /// <returns></returns>
        public static ChronicleT<Ch, M, A> operator +(K<ChronicleT<Ch, M>, A> lhs, Pure<A> rhs) =>
            new(semi => lhs.As().runChronicleT(semi) + M.Pure(These.That<Ch, A>(rhs.Value)));
    }
    
    extension<E, Ch, M, A>(K<ChronicleT<Ch, M>, A> self)
        where M : Monad<M>, SemigroupK<M>, Fallible<E, M>
    {
        /// <summary>
        /// Semigroup combine operator: an associative binary operation.
        /// </summary>
        /// <param name="lhs">Left-hand side operand</param>
        /// <param name="rhs">Right-hand side operand</param>
        /// <returns></returns>
        public static ChronicleT<Ch, M, A> operator +(K<ChronicleT<Ch, M>, A> lhs, Fail<E> rhs) =>
            new (semi => lhs.As().runChronicleT(semi) + M.Fail<These<Ch, A>>(rhs.Value));

    }    
}
