using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class StateTExtensions
{
    extension<S, M, A>(K<StateT<S, M>, A> self)
        where M : Monad<M>, SemigroupK<M>
    {
        /// <summary>
        /// Semigroup combine operator: an associative binary operation.
        /// </summary>
        /// <param name="lhs">Left-hand side operand</param>
        /// <param name="rhs">Right-hand side operand</param>
        /// <returns></returns>
        public static StateT<S, M, A> operator +(K<StateT<S, M>, A> lhs, K<StateT<S, M>, A> rhs) =>
            new(s => lhs.As().runState(s) + rhs.As().runState(s));

        /// <summary>
        /// Semigroup combine operator: an associative binary operation.
        /// </summary>
        /// <param name="lhs">Left-hand side operand</param>
        /// <param name="rhs">Right-hand side operand</param>
        /// <returns></returns>
        public static StateT<S, M, A> operator +(K<StateT<S, M>, A> lhs, Pure<A> rhs) =>
            new(s => lhs.As().runState(s) + M.Pure((rhs.Value, env: s)));
    }
    
    extension<E, S, M, A>(K<StateT<S, M>, A> self)
        where M : Monad<M>, SemigroupK<M>, Fallible<E, M>
    {
        /// <summary>
        /// Semigroup combine operator: an associative binary operation.
        /// </summary>
        /// <param name="lhs">Left-hand side operand</param>
        /// <param name="rhs">Right-hand side operand</param>
        /// <returns></returns>
        public static StateT<S, M, A> operator +(K<StateT<S, M>, A> lhs, Fail<E> rhs) =>
            new(s => lhs.As().runState(s) + M.Fail<(A, S)>(rhs.Value));
    }
}
