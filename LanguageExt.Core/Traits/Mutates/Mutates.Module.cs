using System;
using LanguageExt.Common;

namespace LanguageExt.Traits;

public static class Mutates
{
    /// <summary>
    /// Mutate an atomic value in an environment
    /// </summary>
    /// <param name="f">Function to mapping the value atomically</param>
    /// <typeparam name="F">Readable & Monad trait</typeparam>
    /// <typeparam name="Env">Environment type</typeparam>
    /// <typeparam name="A">Type of the value to read from the environment</typeparam>
    /// <returns>Result of mutation, or original value if the underlying Atom's validator failed</returns>
    public static K<F, A> mutate<F, Env, A>(Func<A, A> f)
        where F   : Functor<F>
        where Env : Mutates<F, A> =>
        Env.Mutable.Map(m => m.Swap(f));
}
