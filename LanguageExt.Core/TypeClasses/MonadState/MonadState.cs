using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageExt;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using LanguageExt.TypeClasses;

namespace LanguageExt
{
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
        State<S, Unit> Put<B>(B state);

        /// <summary>
        /// Embed a simple state action into the monad
        /// </summary>
        /// <param name="f">Action to embed</param>
        /// <returns>Updated state monad</returns>
        State<S, A> State(Func<S, (A, S, bool)> f);
    }
}
