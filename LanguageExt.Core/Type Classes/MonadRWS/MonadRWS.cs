using System;
using LanguageExt.Attributes;
using System.Diagnostics.Contracts;

namespace LanguageExt.TypeClasses
{
    /// <summary>
    /// RWS monad type class
    /// </summary>
    [Typeclass("M*")]
    public interface MonadRWS<MonoidW, R, W, S, A> : Typeclass
        where MonoidW : struct, Monoid<W>
    {
        /// <summary>
        /// Retrieves the current environment
        /// </summary>
        /// <returns>Bound value where the environment value and the bound value are the same</returns>
        [Pure]
        RWS<MonoidW, R, W, S, R> Ask();

        /// <summary>
        /// Retrieves a function of the current environment.
        /// </summary>
        /// <param name="f">The function to modify the environment.</param>
        /// <returns></returns>
        [Pure]
        RWS<MonoidW, R, W, S, A> Local(RWS<MonoidW, R, W, S, A> ma, Func<R, R> f);

        /// <summary>
        /// Tells the monad what you want it to hear.  The monad carries this 'packet'
        /// upwards, merging it if needed (hence the Monoid requirement).
        /// </summary>
        /// <typeparam name="W">Type of the value tell</typeparam>
        /// <param name="what">The value to tell</param>
        /// <returns>Updated writer monad</returns>
        [Pure]
        RWS<MonoidW, R, W, S, Unit> Tell(W what);

        /// <summary>
        /// 'listen' is an action that executes the monad and adds
        /// its output to the value of the computation.
        /// </summary>
        [Pure]
        RWS<MonoidW, R, W, S, (A, B)> Listen<B>(RWS<MonoidW, R, W, S, A> ma, Func<W, B> f);

        /// <summary>
        /// Returns the state from the internals of the monad.
        /// </summary>
        /// <returns>State value where the internal state and the bound value are the same</returns>
        [Pure]
        RWS<MonoidW, R, W, S, S> Get();

        /// <summary>
        /// Replaces the state inside the monad.
        /// </summary>
        /// <typeparam name="S">Type of the value to use as the state</typeparam>
        /// <param name="state">State value to use</param>
        /// <returns>Updated state monad</returns>
        [Pure]
        RWS<MonoidW, R, W, S, Unit> Put(S state);
    }
}
