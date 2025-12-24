using System;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class WriterTExtensions
{
    extension<W, M, A, B>(K<WriterT<W, M>, A> self)
        where M : Monad<M>
        where W : Monoid<W>
    {
        /// <summary>
        /// Monad bind operator
        /// </summary>
        /// <param name="ma">Monad to bind</param>
        /// <param name="f">Binding function</param>
        /// <returns>Mapped monad</returns>
        public static WriterT<W, M, B> operator >> (K<WriterT<W, M>, A> ma, Func<A, K<WriterT<W, M>, B>> f) =>
            +ma.Bind(f);
        
        /// <summary>
        /// Sequentially compose two actions, discarding any value produced by the first, like sequencing operators (such
        /// as the semicolon) in C#.
        /// </summary>
        /// <param name="lhs">First action to run</param>
        /// <param name="rhs">Second action to run</param>
        /// <returns>Result of the second action</returns>
        public static WriterT<W, M, B> operator >> (K<WriterT<W, M>, A> lhs, K<WriterT<W, M>, B> rhs) =>
            lhs >> (_ => rhs);
    }
    
    extension<W, M, A>(K<WriterT<W, M>, A> self)
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
        public static WriterT<W, M, A> operator >> (K<WriterT<W, M>, A> lhs, K<WriterT<W, M>, Unit> rhs) =>
            lhs >> (x => (_ => x) * rhs);
    }
}
