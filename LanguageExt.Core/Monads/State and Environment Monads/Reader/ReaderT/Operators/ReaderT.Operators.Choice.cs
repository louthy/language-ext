using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class ReaderTExtensions
{
    extension<Env, M, A>(K<ReaderT<Env, M>, A> self)
        where M : Monad<M>, Choice<M>
    {
        /// <summary>
        /// Choice operator.  Usually means if the first argument succeeds, return it, otherwise return the second
        /// argument.
        /// </summary>
        /// <param name="lhs">Left hand side operand</param>
        /// <param name="rhs">Right hand side operand</param>
        /// <returns></returns>
        public static ReaderT<Env, M, A> operator |(K<ReaderT<Env, M>, A> lhs, K<ReaderT<Env, M>, A> rhs) =>
            new (env => lhs.As().runReader(env) | rhs.As().runReader(env));

        /// <summary>
        /// Choice operator.  Usually means if the first argument succeeds, return it, otherwise return the second
        /// argument.
        /// </summary>
        /// <param name="lhs">Left hand side operand</param>
        /// <param name="rhs">Right hand side operand</param>
        /// <returns></returns>
        public static ReaderT<Env, M, A> operator |(K<ReaderT<Env, M>, A> lhs, Pure<A> rhs) =>
            new (env => lhs.As().runReader(env) | rhs);
    }
}
