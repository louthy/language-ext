using System;
using LanguageExt.TypeClasses;

namespace LanguageExt
{
    /// <summary>
    /// RWS monad type class
    /// </summary>
    [Typeclass]
    public interface MonadRWS<R, W, S, A> : Monad<(R, W, S), RWS<R, W, S, A>, A>
    {
        /// <summary>
        /// Returns the environment from the internals of the monad.
        /// </summary>
        /// <returns>Reader where the environment and the bound value are the same</returns>
        RWS<R, W, S, R> Ask { get; }

        /// <summary>
        /// Retrieves a function of the current environment.
        /// </summary>
        /// <param name="f">The function to modify the environment.</param>
        /// <returns></returns>
        RWS<R, W, S, A> Local(Func<R, R> f, Reader<R, A> ma);

        /// <summary>
        /// Returns the state from the internals of the monad.
        /// </summary>
        /// <returns>State value where the internal state and the bound value are the same</returns>
        RWS<R, W, S, S> Get { get; }

        /// <summary>
        /// Replaces the state inside the monad.
        /// </summary>
        /// <typeparam name="B">Type of the value to use as the state</typeparam>
        /// <param name="state">State value to use</param>
        /// <returns>Updated state monad</returns>
        RWS<R, W, S, Unit> Put(S state);
    }
}
