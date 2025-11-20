using System;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class ValidationExtensions
{
    extension<F, A, B>(K<Validation<F>, A> self)
    {
        /// <summary>
        /// Monad bind operator
        /// </summary>
        /// <param name="ma">Monad to bind</param>
        /// <param name="f">Binding function</param>
        /// <returns>Mapped monad</returns>
        public static Validation<F, B> operator >> (K<Validation<F>, A> ma, Func<A, K<Validation<F>, B>> f) =>
            +ma.Bind(f);
        
        /// <summary>
        /// Sequentially compose two actions, discarding any value produced by the first, like sequencing operators (such
        /// as the semicolon) in C#.
        /// </summary>
        /// <param name="lhs">First action to run</param>
        /// <param name="rhs">Second action to run</param>
        /// <returns>Result of the second action</returns>
        public static Validation<F, B> operator >> (K<Validation<F>, A> lhs, K<Validation<F>, B> rhs) =>
            lhs >> (_ => rhs);
    }
    
    extension<F, A>(K<Validation<F>, A> self)
    {
        /// <summary>
        /// Sequentially compose two actions.  The second action is a unit-returning action, so the result of the
        /// first action is propagated. 
        /// </summary>
        /// <param name="lhs">First action to run</param>
        /// <param name="rhs">Second action to run</param>
        /// <returns>Result of the first action</returns>
        public static Validation<F, A> operator >> (K<Validation<F>, A> lhs, K<Validation<F>, Unit> rhs) =>
            lhs >> (x => (_ => x) * rhs);
    }
}
