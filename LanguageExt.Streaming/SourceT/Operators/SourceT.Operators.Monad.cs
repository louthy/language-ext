using System;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class SourceTExtensions
{
    extension<M, A, B>(K<SourceT<M>, A> self)
        where M : MonadIO<M>, Alternative<M>
    {
        /// <summary>
        /// Monad bind operator
        /// </summary>
        /// <param name="ma">Monad to bind</param>
        /// <param name="f">Binding function</param>
        /// <returns>Mapped monad</returns>
        public static SourceT<M, B> operator >> (K<SourceT<M>, A> ma, Func<A, K<SourceT<M>, B>> f) =>
            +ma.Bind(f);
        
        /// <summary>
        /// Sequentially compose two actions, discarding any value produced by the first, like sequencing operators (such
        /// as the semicolon) in C#.
        /// </summary>
        /// <param name="lhs">First action to run</param>
        /// <param name="rhs">Second action to run</param>
        /// <returns>Result of the second action</returns>
        public static SourceT<M, B> operator >> (K<SourceT<M>, A> lhs, K<SourceT<M>, B> rhs) =>
            lhs >> (_ => rhs);
    }
    
    extension<M, A>(K<SourceT<M>, A> self)
        where M : MonadIO<M>, Alternative<M>
    {
        /// <summary>
        /// Sequentially compose two actions.  The second action is a unit-returning action, so the result of the
        /// first action is propagated. 
        /// </summary>
        /// <param name="lhs">First action to run</param>
        /// <param name="rhs">Second action to run</param>
        /// <returns>Result of the first action</returns>
        public static SourceT<M, A> operator >> (K<SourceT<M>, A> lhs, K<SourceT<M>, Unit> rhs) =>
            lhs >> (x => (_ => x) * rhs);
    }
}
