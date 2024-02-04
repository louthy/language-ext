using System;
using System.Diagnostics.Contracts;
using LanguageExt.Common;

namespace LanguageExt;

public static partial class Prelude
{
    /// <summary>
    /// Monadic join
    /// </summary>
    [Pure]
    public static Reader<Env, A> flatten<Env, A>(Reader<Env, Reader<Env, A>> ma) =>
        ma.Bind(identity);

    /// <summary>
    /// Reader monad constructor
    /// </summary>
    /// <typeparam name="Env">Environment</typeparam>
    /// <typeparam name="A">Wrapped type</typeparam>
    /// <param name="value">Wrapped value</param>
    /// <returns>Reader monad</returns>
    [Pure]
    public static Reader<Env, A> Reader<Env, A>(A value) =>
        _ => FinSucc(value);

    /// <summary>
    /// Reader monad constructor
    /// </summary>
    /// <typeparam name="Env">Environment</typeparam>
    /// <typeparam name="A">Wrapped type</typeparam>
    /// <param name="value">Wrapped value</param>
    /// <returns>Reader monad</returns>
    [Pure]
    public static Reader<Env, A> Reader<Env, A>(Func<Env, A> f) =>
        e => FinSucc(f(e));

    /// <summary>
    /// Reader failure
    /// </summary>
    [Pure]
    public static Reader<Env, A> ReaderFail<Env, A>(Error error) =>
        _ => FinFail<A>(error);

    /// <summary>
    /// Retrieves the reader monad environment.
    /// </summary>
    /// <typeparam name="Env">Environment</typeparam>
    /// <returns>Reader monad with the environment in as the bound value</returns>
    [Pure]
    public static Reader<Env, Env> ask<Env>() =>
        FinSucc;

    /// <summary>
    /// Retrieves a function of the current environment.
    /// </summary>
    /// <typeparam name="Env">Environment</typeparam>
    /// <typeparam name="R">Bound and mapped value type</typeparam>
    /// <returns>Reader monad with the mapped environment in as the bound value</returns>
    [Pure]
    public static Reader<Env, R> asks<Env, R>(Func<Env, R> f) =>
        e => f(e);

    /// <summary>
    /// Executes a computation in a modified environment
    /// </summary>
    /// <param name="f">The function to modify the environment.</param>
    /// <param name="m">Reader to modify</param>
    /// <returns>Modified reader</returns>
    [Pure]
    public static Reader<Env, A> local<Env, A>(Reader<Env, A> ma, Func<Env, Env> f) =>
        e => ma.Run(f(e));

    /// <summary>
    /// Chooses the first monad result that has a Some(x) for the value
    /// </summary>
    [Pure]
    public static Reader<Env, Option<A>> choose<Env, A>(params Reader<Env, Option<A>>[] monads) =>
        state =>
        {
            foreach (var monad in monads)
            {
                var resA = monad(state);
                if (resA.IsSucc)
                {
                    return resA;
                }
            }
            return default;
        };

    /// <summary>
    /// Run the reader and catch exceptions
    /// </summary>
    [Pure]
    public static Reader<Env, A> tryread<Env, A>(Reader<Env, A> m) =>
        state =>
        {
            try
            {
                return m(state);
            }
            catch(Exception e)
            {
                return Error.New(e);
            }
        };
}
