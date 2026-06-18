using System;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class IteratorIOExtensions
{
    extension<A, B>(K<IteratorIO, A> self)
    {
        /// <summary>
        /// Monad bind operator
        /// </summary>
        /// <param name="ma">Monad to bind</param>
        /// <param name="f">Binding function</param>
        /// <returns>Mapped monad</returns>
        public static IteratorIO<B> operator >> (K<IteratorIO, A> ma, Func<A, K<IteratorIO, B>> f) =>
            +ma.Bind(f);
        
        /// <summary>
        /// Sequentially compose two actions, discarding any value produced by the first, like sequencing operators (such
        /// as the semicolon) in C#.
        /// </summary>
        /// <param name="lhs">First action to run</param>
        /// <param name="rhs">Second action to run</param>
        /// <returns>Result of the second action</returns>
        public static IteratorIO<B> operator >> (K<IteratorIO, A> lhs, K<IteratorIO, B> rhs) =>
            lhs >> (_ => rhs);
    }
    
    extension<A>(K<IteratorIO, A> self)
    {
        /// <summary>
        /// Sequentially compose two actions.  The second action is a unit-returning action, so the result of the
        /// first action is propagated. 
        /// </summary>
        /// <param name="lhs">First action to run</param>
        /// <param name="rhs">Second action to run</param>
        /// <returns>Result of the first action</returns>
        public static IteratorIO<A> operator >> (K<IteratorIO, A> lhs, K<IteratorIO, Unit> rhs) =>
            lhs >> (x => (_ => x) * rhs);
    }
}
