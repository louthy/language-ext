using System;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    /// <summary>
    /// State monad type class
    /// </summary>
    [Typeclass]
    public interface MonadState<S, A> : Monad<S, State<S, A>, A>
    {
        /// <summary>
        /// Returns the state from the internals of the monad.
        /// </summary>
        /// <returns>State value where the internal state and the bound value are the same</returns>
        State<S, S> Get();

        /// <summary>
        /// Replaces the state inside the monad.
        /// </summary>
        /// <typeparam name="B">Type of the value to use as the state</typeparam>
        /// <param name="state">State value to use</param>
        /// <returns>Updated state monad</returns>
        State<S, Unit> Put(S state);

        /// <summary>
        /// Embed a simple state action into the monad
        /// </summary>
        /// <param name="f">Action to embed</param>
        /// <returns>Updated state monad</returns>
        State<S, B> State<B>(Func<S, (B, S, bool)> f);
    }
}
