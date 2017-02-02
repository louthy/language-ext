using System;
using LanguageExt.TypeClasses;

namespace LanguageExt
{
    /// <summary>
    /// Reader monad type class
    /// </summary>
    [Typeclass]
    public interface MonadReader<Env, A> : Monad<Env, Reader<Env, A>, A>
    {
        /// <summary>
        /// Returns the state from the internals of the monad.
        /// </summary>
        /// <returns>State value where the internal state and the bound value are the same</returns>
        Reader<Env, Env> Ask { get; }

        /// <summary>
        /// Retrieves a function of the current environment.
        /// </summary>
        /// <param name="f">The function to modify the environment.</param>
        /// <returns></returns>
        Reader<Env, A> Local(Func<Env, Env> f, Reader<Env, A> ma);

        /// <summary>
        /// Retrieves a function of the current environment.
        /// </summary>
        /// <param name="f">The function to modify the environment.</param>
        /// <returns></returns>
        Reader<Env, B> Reader<B>(Func<Env, B> f);
    }
}
