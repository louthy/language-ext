using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class RWSTExtensions
{
    extension<R, W, S, M, A>(K<RWST<R, W, S, M>, A> self)
        where M : Monad<M>, SemigroupK<M>
        where W : Monoid<W>
    {
        /// <summary>
        /// Semigroup combine operator: an associative binary operation.
        /// </summary>
        /// <param name="lhs">Left-hand side operand</param>
        /// <param name="rhs">Right-hand side operand</param>
        /// <returns></returns>
        public static RWST<R, W, S, M, A> operator +(K<RWST<R, W, S, M>, A> lhs, K<RWST<R, W, S, M>, A> rhs) =>
            new(env => lhs.As().runRWST(env) + rhs.As().runRWST(env));

        /// <summary>
        /// Semigroup combine operator: an associative binary operation.
        /// </summary>
        /// <param name="lhs">Left-hand side operand</param>
        /// <param name="rhs">Right-hand side operand</param>
        /// <returns></returns>
        public static RWST<R, W, S, M, A> operator +(K<RWST<R, W, S, M>, A> lhs, Pure<A> rhs) =>
            new(env => lhs.As().runRWST(env) + M.Pure((rhs.Value, env.Output, env.State)));
    }
    
    extension<E, R, W, S, M, A>(K<RWST<R, W, S, M>, A> self)
        where M : Monad<M>, SemigroupK<M>, Fallible<E, M>
        where W : Monoid<W>
    {
        /// <summary>
        /// Semigroup combine operator: an associative binary operation.
        /// </summary>
        /// <param name="lhs">Left-hand side operand</param>
        /// <param name="rhs">Right-hand side operand</param>
        /// <returns></returns>
        public static RWST<R, W, S, M, A> operator +(K<RWST<R, W, S, M>, A> lhs, Fail<E> rhs) =>
            new(env => lhs.As().runRWST(env) + M.Fail<(A, W, S)>(rhs.Value));
    }
}
