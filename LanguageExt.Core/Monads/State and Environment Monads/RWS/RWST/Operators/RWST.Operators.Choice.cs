using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class RWSTExtensions
{
    extension<R, W, S, M, A>(K<RWST<R, W, S, M>, A> self)
        where M : Monad<M>, Choice<M>
        where W : Monoid<W>
    {
        /// <summary>
        /// Choice operator.  Usually means if the first argument succeeds, return it, otherwise return the second
        /// argument.
        /// </summary>
        /// <param name="lhs">Left hand side operand</param>
        /// <param name="rhs">Right hand side operand</param>
        /// <returns></returns>
        public static RWST<R, W, S, M, A> operator |(K<RWST<R, W, S, M>, A> lhs, K<RWST<R, W, S, M>, A> rhs) =>
            new (env => lhs.As().runRWST(env) | rhs.As().runRWST(env));

        /// <summary>
        /// Choice operator.  Usually means if the first argument succeeds, return it, otherwise return the second
        /// argument.
        /// </summary>
        /// <param name="lhs">Left hand side operand</param>
        /// <param name="rhs">Right hand side operand</param>
        /// <returns></returns>
        public static RWST<R, W, S, M, A> operator |(K<RWST<R, W, S, M>, A> lhs, Pure<A> rhs) =>
            new (env => lhs.As().runRWST(env) | rhs.Map(x => (x, env.Output, env.State)));
    }
}
