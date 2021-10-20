using System;
using System.Diagnostics.Contracts;
using LanguageExt.Attributes;

namespace LanguageExt.TypeClasses
{
    /// <summary>
    /// State monad type class
    /// </summary>
    [Typeclass("M*")]
    public interface MonadState<S, A> : Typeclass
    {
        /// <summary>
        /// Returns the state from the internals of the monad.
        /// </summary>
        /// <returns>State value where the internal state and the bound value are the same</returns>
        [Pure]
        State<S, S> Get();

        /// <summary>
        /// Replaces the state inside the monad.
        /// </summary>
        /// <typeparam name="S">Type of the value to use as the state</typeparam>
        /// <param name="state">State value to use</param>
        /// <returns>Updated state monad</returns>
        [Pure]
        State<S, Unit> Put(S state);

        /// <summary>
        /// Embed a simple state action into the monad
        /// </summary>
        /// <param name="f">Action to embed</param>
        /// <returns>Updated state monad</returns>
        [Pure]
        State<S, A> State(Func<S, A> f);
    }
}
