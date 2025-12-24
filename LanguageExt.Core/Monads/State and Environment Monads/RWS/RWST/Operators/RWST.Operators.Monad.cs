using System;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class RWSTExtensions
{
    extension<R, W, S, M, A, B>(K<RWST<R, W, S, M>, A> self)
        where M : Monad<M>
        where W : Monoid<W>
    {
        /// <summary>
        /// Monad bind operator
        /// </summary>
        /// <param name="ma">Monad to bind</param>
        /// <param name="f">Binding function</param>
        /// <returns>Mapped monad</returns>
        public static RWST<R, W, S, M, B> operator >> (K<RWST<R, W, S, M>, A> ma, Func<A, K<RWST<R, W, S, M>, B>> f) =>
            +ma.Bind(f);
        
        /// <summary>
        /// Sequentially compose two actions, discarding any value produced by the first, like sequencing operators (such
        /// as the semicolon) in C#.
        /// </summary>
        /// <param name="lhs">First action to run</param>
        /// <param name="rhs">Second action to run</param>
        /// <returns>Result of the second action</returns>
        public static RWST<R, W, S, M, B> operator >> (K<RWST<R, W, S, M>, A> lhs, K<RWST<R, W, S, M>, B> rhs) =>
            lhs >> (_ => rhs);
    }
    
    extension<R, W, S, M, A>(K<RWST<R, W, S, M>, A> self)
        where M : Monad<M>
        where W : Monoid<W>
    {
        /// <summary>
        /// Sequentially compose two actions.  The second action is a unit-returning action, so the result of the
        /// first action is propagated. 
        /// </summary>
        /// <param name="lhs">First action to run</param>
        /// <param name="rhs">Second action to run</param>
        /// <returns>Result of the first action</returns>
        public static RWST<R, W, S, M, A> operator >> (K<RWST<R, W, S, M>, A> lhs, K<RWST<R, W, S, M>, Unit> rhs) =>
            lhs >> (x => (_ => x) * rhs);
    }
}
