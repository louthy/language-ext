using System;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class ReaderExtensions
{
    extension<S, A, B>(K<State<S>, A> self)
    {
        /// <summary>
        /// Monad bind operator
        /// </summary>
        /// <param name="ma">Monad to bind</param>
        /// <param name="f">Binding function</param>
        /// <returns>Mapped monad</returns>
        public static State<S, B> operator >> (K<State<S>, A> ma, Func<A, K<State<S>, B>> f) =>
            +ma.Bind(f);
        
        /// <summary>
        /// Sequentially compose two actions, discarding any value produced by the first, like sequencing operators (such
        /// as the semicolon) in C#.
        /// </summary>
        /// <param name="lhs">First action to run</param>
        /// <param name="rhs">Second action to run</param>
        /// <returns>Result of the second action</returns>
        public static State<S, B> operator >> (K<State<S>, A> lhs, K<State<S>, B> rhs) =>
            lhs >> (_ => rhs);
    }
    
    extension<S, A>(K<State<S>, A> self)
    {
        /// <summary>
        /// Sequentially compose two actions.  The second action is a unit-returning action, so the result of the
        /// first action is propagated. 
        /// </summary>
        /// <param name="lhs">First action to run</param>
        /// <param name="rhs">Second action to run</param>
        /// <returns>Result of the first action</returns>
        public static State<S, A> operator >> (K<State<S>, A> lhs, K<State<S>, Unit> rhs) =>
            lhs >> (x => (_ => x) * rhs);
    }
}
