using System;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class ValidationTExtensions
{
    extension<FF, M, A, B>(K<ValidationT<FF, M>, A> self)
        where M : Monad<M>
    {
        /// <summary>
        /// Monad bind operator
        /// </summary>
        /// <param name="ma">Monad to bind</param>
        /// <param name="f">Binding function</param>
        /// <returns>Mapped monad</returns>
        public static ValidationT<FF, M, B> operator >> (K<ValidationT<FF, M>, A> ma, Func<A, K<ValidationT<FF, M>, B>> f) =>
            +ma.Bind(f);
        
        /// <summary>
        /// Sequentially compose two actions, discarding any value produced by the first, like sequencing operators (such
        /// as the semicolon) in C#.
        /// </summary>
        /// <param name="lhs">First action to run</param>
        /// <param name="rhs">Second action to run</param>
        /// <returns>Result of the second action</returns>
        public static ValidationT<FF, M, B> operator >> (K<ValidationT<FF, M>, A> lhs, K<ValidationT<FF, M>, B> rhs) =>
            lhs >> (_ => rhs);
    }
    
    extension<FF, M, A>(K<ValidationT<FF, M>, A> self)
        where M : Monad<M>
    {
        /// <summary>
        /// Sequentially compose two actions.  The second action is a unit-returning action, so the result of the
        /// first action is propagated. 
        /// </summary>
        /// <param name="lhs">First action to run</param>
        /// <param name="rhs">Second action to run</param>
        /// <returns>Result of the first action</returns>
        public static ValidationT<FF, M, A> operator >> (K<ValidationT<FF, M>, A> lhs, K<ValidationT<FF, M>, Unit> rhs) =>
            lhs >> (x => (_ => x) * rhs);
    }
}
