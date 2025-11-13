using System;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class MonadExtensions
{
    extension<M, A, B>(K<M, A> self)
        where M : Monad<M>
    {
        public static K<M, B> operator >> (K<M, A> ma, Func<A, K<M, B>> f) =>
            ma.Bind(f);
        
        /// <summary>
        /// Sequentially compose two actions, discarding any value produced by the first, like sequencing operators (such
        /// as the semicolon) in C#.
        /// </summary>
        /// <param name="lhs">First action to run</param>
        /// <param name="rhs">Second action to run</param>
        /// <returns>Result of the second action</returns>
        public static K<M, B> operator >> (K<M, A> lhs, K<M, B> rhs) =>
            lhs >> (_ => rhs);
    }
    
    extension<M, A>(K<M, A> self)
        where M : Monad<M>
    {
        /// <summary>
        /// Sequentially compose two actions.  The second action is a unit-returning action, so the result of the
        /// first action is propagated. 
        /// </summary>
        /// <param name="lhs">First action to run</param>
        /// <param name="rhs">Second action to run</param>
        /// <returns>Result of the first action</returns>
        public static K<M, A> operator >> (K<M, A> lhs, K<M, Unit> rhs) =>
            lhs >> (x => (_ => x) * rhs);
    }
}
