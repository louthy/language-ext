using System;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class SourceExtensions
{
    extension<A, B>(K<Source, A> self)
    {
        /// <summary>
        /// Monad bind operator
        /// </summary>
        /// <param name="ma">Monad to bind</param>
        /// <param name="f">Binding function</param>
        /// <returns>Mapped monad</returns>
        public static Source<B> operator >> (K<Source, A> ma, Func<A, K<Source, B>> f) =>
            +ma.Bind(f);
        
        /// <summary>
        /// Sequentially compose two actions, discarding any value produced by the first, like sequencing operators (such
        /// as the semicolon) in C#.
        /// </summary>
        /// <param name="lhs">First action to run</param>
        /// <param name="rhs">Second action to run</param>
        /// <returns>Result of the second action</returns>
        public static Source<B> operator >> (K<Source, A> lhs, K<Source, B> rhs) =>
            lhs >> (_ => rhs);
    }
    
    extension<A>(K<Source, A> self)
    {
        /// <summary>
        /// Sequentially compose two actions.  The second action is a unit-returning action, so the result of the
        /// first action is propagated. 
        /// </summary>
        /// <param name="lhs">First action to run</param>
        /// <param name="rhs">Second action to run</param>
        /// <returns>Result of the first action</returns>
        public static Source<A> operator >> (K<Source, A> lhs, K<Source, Unit> rhs) =>
            lhs >> (x => (_ => x) * rhs);
    }
}
