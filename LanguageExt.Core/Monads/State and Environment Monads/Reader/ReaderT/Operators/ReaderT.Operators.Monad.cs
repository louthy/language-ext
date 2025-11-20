using System;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class ReaderTExtensions
{
    extension<Env, M, A, B>(K<ReaderT<Env, M>, A> self)
        where M : Monad<M>
    {
        /// <summary>
        /// Monad bind operator
        /// </summary>
        /// <param name="ma">Monad to bind</param>
        /// <param name="f">Binding function</param>
        /// <returns>Mapped monad</returns>
        public static ReaderT<Env, M, B> operator >> (K<ReaderT<Env, M>, A> ma, Func<A, K<ReaderT<Env, M>, B>> f) =>
            +ma.Bind(f);
        
        /// <summary>
        /// Sequentially compose two actions, discarding any value produced by the first, like sequencing operators (such
        /// as the semicolon) in C#.
        /// </summary>
        /// <param name="lhs">First action to run</param>
        /// <param name="rhs">Second action to run</param>
        /// <returns>Result of the second action</returns>
        public static ReaderT<Env, M, B> operator >> (K<ReaderT<Env, M>, A> lhs, K<ReaderT<Env, M>, B> rhs) =>
            lhs >> (_ => rhs);
    }
    
    extension<Env, M, A>(K<ReaderT<Env, M>, A> self)
        where M : Monad<M>
    {
        /// <summary>
        /// Sequentially compose two actions.  The second action is a unit-returning action, so the result of the
        /// first action is propagated. 
        /// </summary>
        /// <param name="lhs">First action to run</param>
        /// <param name="rhs">Second action to run</param>
        /// <returns>Result of the first action</returns>
        public static ReaderT<Env, M, A> operator >> (K<ReaderT<Env, M>, A> lhs, K<ReaderT<Env, M>, Unit> rhs) =>
            lhs >> (x => (_ => x) * rhs);
    }
}
