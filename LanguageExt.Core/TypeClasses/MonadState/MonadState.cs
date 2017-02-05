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
    public interface MonadState<SStateA, SSA, S, A>
        where SStateA : struct, StateMonadValue<SSA, S, A>
    {
        /// <summary>
        /// Returns the state from the internals of the monad.
        /// </summary>
        /// <returns>State value where the internal state and the bound value are the same</returns>
        [Pure]
        SSS Get<SStateS, SSS>() where SStateS : struct, StateMonadValue<SSS, S, S>;

        /// <summary>
        /// Replaces the state inside the monad.
        /// </summary>
        /// <typeparam name="S">Type of the value to use as the state</typeparam>
        /// <param name="state">State value to use</param>
        /// <returns>Updated state monad</returns>
        [Pure]
        SSU Put<SStateU, SSU>(S state) where SStateU : struct, StateMonadValue<SSU, S, Unit>;

        /// <summary>
        /// Embed a simple state action into the monad
        /// </summary>
        /// <param name="f">Action to embed</param>
        /// <returns>Updated state monad</returns>
        [Pure]
        SSA State(Func<S, (A, S, bool)> f);
    }
}
