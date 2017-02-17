using System;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;

namespace LanguageExt
{
    /// <summary>
    /// Reader monad type class
    /// </summary>
    [Typeclass]
    public interface MonadReader<SReaderA, SRA, Env, A>
        where SReaderA : struct, ReaderMonadValue<SRA, Env, A>
    {
        /// <summary>
        /// Returns the state from the internals of the monad.
        /// </summary>
        /// <returns>State value where the internal state and the bound value are the same</returns>
        [Pure]
        SEnv Ask<SReaderEnv, SEnv>() where SReaderEnv : struct, ReaderMonadValue<SEnv, Env, Env>;

        /// <summary>
        /// Retrieves a function of the current environment.
        /// </summary>
        /// <param name="f">The function to modify the environment.</param>
        /// <returns></returns>
        [Pure]
        SRA Local(Func<Env, Env> f, SRA ma);

        /// <summary>
        /// Retrieves a function of the current environment.
        /// </summary>
        /// <param name="f">The function to modify the environment.</param>
        /// <returns></returns>
        [Pure]
        SRA Reader(Func<Env, A> f);
    }
}
