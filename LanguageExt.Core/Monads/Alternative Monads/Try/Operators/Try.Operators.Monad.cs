using System;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class TryExtensions
{
    extension<A, B>(K<Try, A> self)
    {
        /// <summary>
        /// Monad bind operator
        /// </summary>
        /// <param name="ma">Monad to bind</param>
        /// <param name="f">Binding function</param>
        /// <returns>Mapped monad</returns>
        public static Try<B> operator >> (K<Try, A> ma, Func<A, K<Try, B>> f) =>
            +ma.Bind(f);
        
        /// <summary>
        /// Sequentially compose two actions, discarding any value produced by the first, like sequencing operators (such
        /// as the semicolon) in C#.
        /// </summary>
        /// <param name="lhs">First action to run</param>
        /// <param name="rhs">Second action to run</param>
        /// <returns>Result of the second action</returns>
        public static Try<B> operator >> (K<Try, A> lhs, K<Try, B> rhs) =>
            lhs >> (_ => rhs);
    }
    
    extension<A>(K<Try, A> self)
    {
        /// <summary>
        /// Sequentially compose two actions.  The second action is a unit-returning action, so the result of the
        /// first action is propagated. 
        /// </summary>
        /// <param name="lhs">First action to run</param>
        /// <param name="rhs">Second action to run</param>
        /// <returns>Result of the first action</returns>
        public static Try<A> operator >> (K<Try, A> lhs, K<Try, Unit> rhs) =>
            lhs >> (x => (_ => x) * rhs);
    }
}
