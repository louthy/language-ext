using System;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class StateTExtensions
{
    extension<S, M, A, B>(K<StateT<S, M>, A> self)
        where M : Monad<M>
    {
        /// <summary>
        /// Monad bind operator
        /// </summary>
        /// <param name="ma">Monad to bind</param>
        /// <param name="f">Binding function</param>
        /// <returns>Mapped monad</returns>
        public static StateT<S, M, B> operator >> (K<StateT<S, M>, A> ma, Func<A, K<StateT<S, M>, B>> f) =>
            +ma.Bind(f);
        
        /// <summary>
        /// Sequentially compose two actions, discarding any value produced by the first, like sequencing operators (such
        /// as the semicolon) in C#.
        /// </summary>
        /// <param name="lhs">First action to run</param>
        /// <param name="rhs">Second action to run</param>
        /// <returns>Result of the second action</returns>
        public static StateT<S, M, B> operator >> (K<StateT<S, M>, A> lhs, K<StateT<S, M>, B> rhs) =>
            lhs >> (_ => rhs);
    }
    
    extension<S, M, A>(K<StateT<S, M>, A> self)
        where M : Monad<M>
    {
        /// <summary>
        /// Sequentially compose two actions.  The second action is a unit-returning action, so the result of the
        /// first action is propagated. 
        /// </summary>
        /// <param name="lhs">First action to run</param>
        /// <param name="rhs">Second action to run</param>
        /// <returns>Result of the first action</returns>
        public static StateT<S, M, A> operator >> (K<StateT<S, M>, A> lhs, K<StateT<S, M>, Unit> rhs) =>
            lhs >> (x => (_ => x) * rhs);
    }
}
