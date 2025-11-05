using System;
using LanguageExt.Common;

namespace LanguageExt.Traits;

public static partial class Maybe
{
    /// <summary>
    /// Monad that is either the IO monad or a transformer with the IO monad in its stack
    /// </summary>
    /// <typeparam name="M">Self-referring trait</typeparam>
    public interface MonadIO<M>
        where M : MonadIO<M>
    {
        /// <summary>
        /// Lifts the IO monad into a monad transformer stack.  
        /// </summary>
        /// <param name="ma">IO computation to lift</param>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <returns>The outer monad with the IO monad lifted into it</returns>
        public static virtual K<M, A> LiftIOMaybe<A>(K<IO, A> ma) =>
            M.LiftIOMaybe(ma.As());

        /// <summary>
        /// Lifts the IO monad into a monad transformer stack.  
        /// </summary>
        /// <remarks>
        /// IMPLEMENTATION REQUIRED: If this method isn't overloaded in this monad
        /// or any monad in the stack on the way to the inner-monad, then it will throw
        /// an exception.
        ///
        /// This isn't ideal, it appears to be the only way to achieve this
        /// kind of functionality in C# without resorting to magic. 
        /// </remarks>
        /// <param name="ma">IO computation to lift</param>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <returns>The outer monad with the IO monad lifted into it</returns>
        /// <exception cref="ExceptionalException">If this method isn't overloaded in
        /// the inner monad or any monad in the stack on the way to the inner-monad,
        /// then it will throw an exception.</exception>
        public static virtual K<M, A> LiftIOMaybe<A>(IO<A> ma) =>
            throw new ExceptionalException(Errors.LiftIONotSupported);
    }
}
