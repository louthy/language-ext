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
    /// <returns>Optional result. Can only fail if the original Atom has a validator</returns>
    public static K<F, Option<A>> mutate<F, Env, A>(Func<A, A> f)
        where F   : Functor<F>
        where Env : Mutates<F, A> =>
        Env.Mutable.Map(m => m.Swap(f));

    /// <summary>
    /// Mutate an atomic value in an environment
    /// </summary>
    /// <param name="f">Function to mapping the value atomically</param>
    /// <typeparam name="M">Readable & Monad trait</typeparam>
    /// <typeparam name="Env">Environment type</typeparam>
    /// <typeparam name="A">Type of the value to read from the environment</typeparam>
    /// <returns>Optional result. Can only fail if the original Atom has a validator</returns>
    public static K<M, A> mutateOrEmpty<M, Env, A>(Func<A, A> f)
        where M   : Monad<M>, MonoidK<M>
        where Env : Mutates<M, A> =>
        Env.Mutable
           .Map(m => m.Swap(f))
           .Bind(r => r.Match(Some: M.Pure, None: M.Empty<A>));

    /// <summary>
    /// Mutate an atomic value in an environment
    /// </summary>
    /// <param name="f">Function to mapping the value atomically</param>
    /// <typeparam name="M">Readable & Monad trait</typeparam>
    /// <typeparam name="Env">Environment type</typeparam>
    /// <typeparam name="A">Type of the value to read from the environment</typeparam>
    /// <returns>Optional result. Can only fail if the original Atom has a validator</returns>
    public static K<M, A> mutateOrFail<Err, M, Env, A>(Func<A, A> f, Err fail)
        where M   : Monad<M>, Fallible<Err, M>
        where Env : Mutates<M, A> =>
        mutate<M, Env, A>(f)
           .Bind(r => r.Match(Some: M.Pure, None: () => M.Fail<A>(fail)));

    /// <summary>
    /// Mutate an atomic value in an environment
    /// </summary>
    /// <param name="f">Function to mapping the value atomically</param>
    /// <typeparam name="M">Readable & Monad trait</typeparam>
    /// <typeparam name="Env">Environment type</typeparam>
    /// <typeparam name="A">Type of the value to read from the environment</typeparam>
    /// <returns>Optional result. Can only fail if the original Atom has a validator</returns>
    public static K<M, A> mutateOrFail<M, Env, A>(Func<A, A> f, Error fail)
        where M   : Monad<M>, Fallible<M>
        where Env : Mutates<M, A> =>
        mutate<M, Env, A>(f)
           .Bind(r => r.Match(Some: M.Pure, None: () => M.Fail<A>(fail)));

    /// <summary>
    /// Mutate an atomic value in an environment
    /// </summary>
    /// <param name="f">Function to mapping the value atomically</param>
    /// <typeparam name="M">Readable & Monad trait</typeparam>
    /// <typeparam name="Env">Environment type</typeparam>
    /// <typeparam name="A">Type of the value to read from the environment</typeparam>
    /// <returns>Optional result. Can only fail if the original Atom has a validator</returns>
    public static K<M, A> mutateOrFail<M, Env, A>(Func<A, A> f)
        where M   : Readable<M, Env>, Monad<M>, Fallible<M>
        where Env : Mutates<M, A> =>
        mutate<M, Env, A>(f)
           .Bind(r => r.Match(Some: M.Pure, None: () => M.Fail<A>(Errors.None)));
}
