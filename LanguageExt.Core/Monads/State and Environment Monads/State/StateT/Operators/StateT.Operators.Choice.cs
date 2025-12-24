using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class StateTExtensions
{
    extension<S, M, A>(K<StateT<S, M>, A> self)
        where M : Monad<M>, Choice<M>
    {
        /// <summary>
        /// Choice operator.  Usually means if the first argument succeeds, return it, otherwise return the second
        /// argument.
        /// </summary>
        /// <param name="lhs">Left hand side operand</param>
        /// <param name="rhs">Right hand side operand</param>
        /// <returns></returns>
        public static StateT<S, M, A> operator |(K<StateT<S, M>, A> lhs, K<StateT<S, M>, A> rhs) =>
            new (s => lhs.As().runState(s) | rhs.As().runState(s));

        /// <summary>
        /// Choice operator.  Usually means if the first argument succeeds, return it, otherwise return the second
        /// argument.
        /// </summary>
        /// <param name="lhs">Left hand side operand</param>
        /// <param name="rhs">Right hand side operand</param>
        /// <returns></returns>
        public static StateT<S, M, A> operator |(K<StateT<S, M>, A> lhs, Pure<A> rhs) =>
            new (s => lhs.As().runState(s) | rhs.Map(x => (x, env: s)));
    }
}
