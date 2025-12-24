using System;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class WriterExtensions
{
    extension<W, A, B>(K<Writer<W>, A> self)
        where W : Monoid<W>
    {
        /// <summary>
        /// Monad bind operator
        /// </summary>
        /// <param name="ma">Monad to bind</param>
        /// <param name="f">Binding function</param>
        /// <returns>Mapped monad</returns>
        public static Writer<W, B> operator >> (K<Writer<W>, A> ma, Func<A, K<Writer<W>, B>> f) =>
            +ma.Bind(f);
        
        /// <summary>
        /// Sequentially compose two actions, discarding any value produced by the first, like sequencing operators (such
        /// as the semicolon) in C#.
        /// </summary>
        /// <param name="lhs">First action to run</param>
        /// <param name="rhs">Second action to run</param>
        /// <returns>Result of the second action</returns>
        public static Writer<W, B> operator >> (K<Writer<W>, A> lhs, K<Writer<W>, B> rhs) =>
            lhs >> (_ => rhs);
    }
    
    extension<W, A>(K<Writer<W>, A> self)
        where W : Monoid<W>
    {
        /// <summary>
        /// Sequentially compose two actions.  The second action is a unit-returning action, so the result of the
        /// first action is propagated. 
        /// </summary>
        /// <param name="lhs">First action to run</param>
        /// <param name="rhs">Second action to run</param>
        /// <returns>Result of the first action</returns>
        public static Writer<W, A> operator >> (K<Writer<W>, A> lhs, K<Writer<W>, Unit> rhs) =>
            lhs >> (x => (_ => x) * rhs);
    }
}
