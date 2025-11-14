using System;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class FreeExtensions
{
    extension<F, A, B>(K<Free<F>, A> self)
        where F : Functor<F>
    {
        /// <summary>
        /// Monad bind operator
        /// </summary>
        /// <param name="ma">Monad to bind</param>
        /// <param name="f">Binding function</param>
        /// <returns>Mapped monad</returns>
        public static Free<F, B> operator >> (K<Free<F>, A> ma, Func<A, K<Free<F>, B>> f) =>
            +ma.Bind(f);
        
        /// <summary>
        /// Sequentially compose two actions, discarding any value produced by the first, like sequencing operators (such
        /// as the semicolon) in C#.
        /// </summary>
        /// <param name="lhs">First action to run</param>
        /// <param name="rhs">Second action to run</param>
        /// <returns>Result of the second action</returns>
        public static Free<F, B> operator >> (K<Free<F>, A> lhs, K<Free<F>, B> rhs) =>
            lhs >> (_ => rhs);
    }
    
    extension<F, A>(K<Free<F>, A> self)
        where F : Functor<F>
    {
        /// <summary>
        /// Sequentially compose two actions.  The second action is a unit-returning action, so the result of the
        /// first action is propagated. 
        /// </summary>
        /// <param name="lhs">First action to run</param>
        /// <param name="rhs">Second action to run</param>
        /// <returns>Result of the first action</returns>
        public static Free<F, A> operator >> (K<Free<F>, A> lhs, K<Free<F>, Unit> rhs) =>
            lhs >> (x => (_ => x) * rhs);
    }
}
