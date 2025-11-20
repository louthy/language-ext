using System;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class EitherTExtensions
{
    extension<L, M, A, B>(K<EitherT<L, M>, A> self)
        where M : Monad<M>
    {
        /// <summary>
        /// Monad bind operator
        /// </summary>
        /// <param name="ma">Monad to bind</param>
        /// <param name="f">Binding function</param>
        /// <returns>Mapped monad</returns>
        public static EitherT<L, M, B> operator >> (K<EitherT<L, M>, A> ma, Func<A, K<EitherT<L, M>, B>> f) =>
            +ma.Bind(f);
        
        /// <summary>
        /// Sequentially compose two actions, discarding any value produced by the first, like sequencing operators (such
        /// as the semicolon) in C#.
        /// </summary>
        /// <param name="lhs">First action to run</param>
        /// <param name="rhs">Second action to run</param>
        /// <returns>Result of the second action</returns>
        public static EitherT<L, M, B> operator >> (K<EitherT<L, M>, A> lhs, K<EitherT<L, M>, B> rhs) =>
            lhs >> (_ => rhs);
    }
    
    extension<L, M, A, B>(K<EitherT<L, M>, A> self)
        where M : MonadIO<M>
    {
        /// <summary>
        /// Monad bind operator
        /// </summary>
        /// <param name="ma">Monad to bind</param>
        /// <param name="f">Binding function</param>
        /// <returns>Mapped monad</returns>
        public static EitherT<L, M, B> operator >> (K<EitherT<L, M>, A> ma, Func<A, K<IO, B>> f) =>
            +ma.Bind(x => +f(x));
        
        /// <summary>
        /// Sequentially compose two actions, discarding any value produced by the first, like sequencing operators (such
        /// as the semicolon) in C#.
        /// </summary>
        /// <param name="lhs">First action to run</param>
        /// <param name="rhs">Second action to run</param>
        /// <returns>Result of the second action</returns>
        public static EitherT<L, M, B> operator >> (K<EitherT<L, M>, A> lhs, K<IO, B> rhs) =>
            lhs >> (_ => rhs);
    }
    
    extension<L, M, A>(K<EitherT<L, M>, A> self)
        where M : Monad<M>
    {
        /// <summary>
        /// Sequentially compose two actions.  The second action is a unit-returning action, so the result of the
        /// first action is propagated. 
        /// </summary>
        /// <param name="lhs">First action to run</param>
        /// <param name="rhs">Second action to run</param>
        /// <returns>Result of the first action</returns>
        public static EitherT<L, M, A> operator >> (K<EitherT<L, M>, A> lhs, K<EitherT<L, M>, Unit> rhs) =>
            lhs >> (x => (_ => x) * rhs);
    }
        
    extension<L, M, A>(K<EitherT<L, M>, A> self)
        where M : MonadIO<M>
    {
        /// <summary>
        /// Sequentially compose two actions.  The second action is a unit-returning action, so the result of the
        /// first action is propagated. 
        /// </summary>
        /// <param name="lhs">First action to run</param>
        /// <param name="rhs">Second action to run</param>
        /// <returns>Result of the first action</returns>
        public static EitherT<L, M, A> operator >> (K<EitherT<L, M>, A> lhs, K<IO, Unit> rhs) =>
            lhs >> (x => (_ => x) * rhs);
    }
}
