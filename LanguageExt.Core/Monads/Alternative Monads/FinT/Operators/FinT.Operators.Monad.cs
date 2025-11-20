using System;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class FinTExtensions
{
    extension<M, A, B>(K<FinT<M>, A> self)
        where M : Monad<M>
    {
        /// <summary>
        /// Monad bind operator
        /// </summary>
        /// <param name="ma">Monad to bind</param>
        /// <param name="f">Binding function</param>
        /// <returns>Mapped monad</returns>
        public static FinT<M, B> operator >> (K<FinT<M>, A> ma, Func<A, K<FinT<M>, B>> f) =>
            +ma.Bind(f);
        
        /// <summary>
        /// Sequentially compose two actions, discarding any value produced by the first, like sequencing operators (such
        /// as the semicolon) in C#.
        /// </summary>
        /// <param name="lhs">First action to run</param>
        /// <param name="rhs">Second action to run</param>
        /// <returns>Result of the second action</returns>
        public static FinT<M, B> operator >> (K<FinT<M>, A> lhs, K<FinT<M>, B> rhs) =>
            lhs >> (_ => rhs);
    }
    
    extension<M, A, B>(K<FinT<M>, A> self)
        where M : MonadIO<M>
    {
        /// <summary>
        /// Monad bind operator
        /// </summary>
        /// <param name="ma">Monad to bind</param>
        /// <param name="f">Binding function</param>
        /// <returns>Mapped monad</returns>
        public static FinT<M, B> operator >> (K<FinT<M>, A> ma, Func<A, K<IO, B>> f) =>
            +ma.Bind(x => +f(x));
        
        /// <summary>
        /// Sequentially compose two actions, discarding any value produced by the first, like sequencing operators (such
        /// as the semicolon) in C#.
        /// </summary>
        /// <param name="lhs">First action to run</param>
        /// <param name="rhs">Second action to run</param>
        /// <returns>Result of the second action</returns>
        public static FinT<M, B> operator >> (K<FinT<M>, A> lhs, K<IO, B> rhs) =>
            lhs >> (_ => rhs);
    }
    
    extension<M, A>(K<FinT<M>, A> self)
        where M : Monad<M>
    {
        /// <summary>
        /// Sequentially compose two actions.  The second action is a unit-returning action, so the result of the
        /// first action is propagated. 
        /// </summary>
        /// <param name="lhs">First action to run</param>
        /// <param name="rhs">Second action to run</param>
        /// <returns>Result of the first action</returns>
        public static FinT<M, A> operator >> (K<FinT<M>, A> lhs, K<FinT<M>, Unit> rhs) =>
            lhs >> (x => (_ => x) * rhs);
    }
        
    extension<M, A>(K<FinT<M>, A> self)
        where M : MonadIO<M>
    {
        /// <summary>
        /// Sequentially compose two actions.  The second action is a unit-returning action, so the result of the
        /// first action is propagated. 
        /// </summary>
        /// <param name="lhs">First action to run</param>
        /// <param name="rhs">Second action to run</param>
        /// <returns>Result of the first action</returns>
        public static FinT<M, A> operator >> (K<FinT<M>, A> lhs, K<IO, Unit> rhs) =>
            lhs >> (x => (_ => x) * rhs);
    }
}
