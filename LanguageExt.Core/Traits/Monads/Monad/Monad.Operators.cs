using System;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class MonadExtensions
{
    extension<M, A, B>(K<M, A> self)
        where M : Monad<M>
    {
        /// <summary>
        /// Monad bind operator
        /// </summary>
        /// <param name="ma">Monad to bind</param>
        /// <param name="f">Binding function</param>
        /// <returns>Mapped monad</returns>
        public static K<M, B> operator >> (K<M, A> ma, Func<A, K<M, B>> f) =>
            ma.Bind(f);
        
        /// <summary>
        /// Sequentially compose two actions, discarding any value produced by the first, like sequencing operators
        /// (such as the semicolon) in C#.
        /// </summary>
        /// <param name="lhs">First action to run</param>
        /// <param name="rhs">Second action to run</param>
        /// <returns>Result of the second action</returns>
        public static K<M, B> operator >> (K<M, A> lhs, K<M, B> rhs) =>
            lhs >> (_ => rhs);
        
        /// <summary>
        /// Sequentially compose two actions, discarding any value produced by the second, like sequencing operators
        /// (such as the semicolon) in C#.
        /// </summary>
        /// <param name="lhs">First action to run</param>
        /// <param name="mb">Second action to run</param>
        /// <returns>Result of the second action</returns>
        public static K<M, A> operator << (K<M, A> lhs, K<M, B> rhs) =>
            lhs >> (x => rhs.Map(_ => x));
    }
    
    extension<M, A, B>(K<M, A> self)
        where M : MonadIO<M>
    {
        /// <summary>
        /// Monad bind operator
        /// </summary>
        /// <param name="ma">Monad to bind</param>
        /// <param name="f">Binding function</param>
        /// <returns>Mapped monad</returns>
        public static K<M, B> operator >> (K<M, A> ma, Func<A, K<IO, B>> f) =>
            ma.Bind(x => M.LiftIO(f(x)));
        
        /// <summary>
        /// Sequentially compose two actions, discarding any value produced by the first, like sequencing operators (such
        /// as the semicolon) in C#.
        /// </summary>
        /// <param name="lhs">First action to run</param>
        /// <param name="rhs">Second action to run</param>
        /// <returns>Result of the second action</returns>
        public static K<M, B> operator >> (K<M, A> lhs, K<IO, B> rhs) =>
            lhs >> (_ => rhs);
    }
    
    extension<M, A, B>(K<IO, A> self)
        where M : MonadIO<M>
    {
        /// <summary>
        /// Monad bind operator
        /// </summary>
        /// <param name="ma">Monad to bind</param>
        /// <param name="f">Binding function</param>
        /// <returns>Mapped monad</returns>
        public static K<M, B> operator >> (K<IO, A> ma, Func<A, K<M, B>> f) =>
            M.LiftIO(ma).Bind(f);
        
        /// <summary>
        /// Sequentially compose two actions, discarding any value produced by the first, like sequencing operators (such
        /// as the semicolon) in C#.
        /// </summary>
        /// <param name="lhs">First action to run</param>
        /// <param name="rhs">Second action to run</param>
        /// <returns>Result of the second action</returns>
        public static K<M, B> operator >> (K<IO, A> lhs, K<M, B> rhs) =>
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
    
    extension<M, A>(K<M, A> self)
        where M : MonadIO<M>
    {
        /// <summary>
        /// Sequentially compose two actions.  The second action is a unit-returning action, so the result of the
        /// first action is propagated. 
        /// </summary>
        /// <param name="lhs">First action to run</param>
        /// <param name="rhs">Second action to run</param>
        /// <returns>Result of the first action</returns>
        public static K<M, A> operator >> (K<M, A> lhs, K<IO, Unit> rhs) =>
            lhs >> (x => (_ => x) * rhs);
    }
        
    extension<M, A, B, C>(Func<A, K<M, B>> self) where M : Monad<M>
    {
        /// <summary>
        /// Kleisli composition operator overload. Composes two monad-returning functions, equivalent to Haskell's `>=>` or "fish" operator
        /// </summary>
        /// <param name="bind1">The first function to compose</param>
        /// <param name="bind2">The second function to compose</param>
        /// <returns></returns>
        public static Func<A, K<M, C>> operator >> (Func<A, K<M, B>> bind1, Func<B, K<M, C>> bind2) =>
            a => bind1(a) >> bind2;
    }
}
