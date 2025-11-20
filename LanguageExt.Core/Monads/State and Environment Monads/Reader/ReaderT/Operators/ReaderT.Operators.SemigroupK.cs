using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class ReaderTExtensions
{
    extension<Env, M, A>(K<ReaderT<Env, M>, A> self)
        where M : Monad<M>, SemigroupK<M>
    {
        /// <summary>
        /// Semigroup combine operator: an associative binary operation.
        /// </summary>
        /// <param name="lhs">Left-hand side operand</param>
        /// <param name="rhs">Right-hand side operand</param>
        /// <returns></returns>
        public static ReaderT<Env, M, A> operator +(K<ReaderT<Env, M>, A> lhs, K<ReaderT<Env, M>, A> rhs) =>
            new(env => lhs.As().runReader(env) + rhs.As().runReader(env));

        //+lhs.Combine(rhs);

        /// <summary>
        /// Semigroup combine operator: an associative binary operation.
        /// </summary>
        /// <param name="lhs">Left-hand side operand</param>
        /// <param name="rhs">Right-hand side operand</param>
        /// <returns></returns>
        public static ReaderT<Env, M, A> operator +(K<ReaderT<Env, M>, A> lhs, Pure<A> rhs) =>
            new(env => lhs.As().runReader(env) + M.Pure(rhs.Value));
    }
    
    extension<E, Env, M, A>(K<ReaderT<Env, M>, A> self)
        where M : Monad<M>, SemigroupK<M>, Fallible<E, M>
    {
        /// <summary>
        /// Semigroup combine operator: an associative binary operation.
        /// </summary>
        /// <param name="lhs">Left-hand side operand</param>
        /// <param name="rhs">Right-hand side operand</param>
        /// <returns></returns>
        public static ReaderT<Env, M, A> operator +(K<ReaderT<Env, M>, A> lhs, Fail<E> rhs) =>
            new(env => lhs.As().runReader(env) + M.Fail<A>(rhs.Value));
    }
}
