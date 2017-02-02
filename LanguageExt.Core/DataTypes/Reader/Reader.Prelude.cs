using LanguageExt.ClassInstances;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace LanguageExt
{
    public static partial class Prelude
    {
        /// <summary>
        /// Reader monad constructor
        /// </summary>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="T">Wrapped type</typeparam>
        /// <param name="value">Wrapped value</param>
        /// <returns>Reader monad</returns>
        [Pure]
        public static Reader<Env, T> Reader<Env, T>(T value) =>
            new Reader<Env, T>(env => (value, env, false));

        /// <summary>
        /// Retrieves the reader monad environment.
        /// </summary>
        /// <typeparam name="Env">Environment</typeparam>
        /// <returns>Reader monad with the environment in as the bound value</returns>
        [Pure]
        public static Reader<Env, Env> ask<Env>() =>
            default(MReader<Env, Env>).Ask;

        /// <summary>
        /// Retrieves a function of the current environment.
        /// </summary>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="R">Bound and mapped value type</typeparam>
        /// <returns>Reader monad with the mapped environment in as the bound value</returns>
        [Pure]
        public static Reader<Env, R> asks<Env, R>(Func<Env, R> f) =>
            default(MReader<Env, R>).Reader(f);

        /// <summary>
        /// Executes a computation in a modified environment
        /// </summary>
        /// <param name="f">The function to modify the environment.</param>
        /// <param name="m">Reader to modify</param>
        /// <returns>Modified reader</returns>
        [Pure]
        public static Reader<Env, A> local<Env, A>(Func<Env, Env> f, Reader<Env, A> ma) =>
            default(MReader<Env, A>).Local(f, ma);

        /// <summary>
        /// Chooses the first monad result that has a Some(x) for the value
        /// </summary>
        [Pure]
        public static Reader<S, Option<T>> choose<S, T>(params Reader<S, Option<T>>[] monads) =>
            default(MReader<S, Option<T>>).Return(state =>
            {
                foreach (var monad in monads)
                {
                    var (x, s, bottom) = monad.Eval(state);
                    if (!bottom && x.IsSome)
                    {
                        return (x, s, bottom);
                    }
                }
                return (default(T), state, true);
            });

        [Pure]
        public static Reader<Env, A> tryread<Env, A>(Func<Reader<Env, A>> f) =>
            default(MReader<Env, A>).Return(state =>
            {
                try
                {
                    return f().Eval(state);
                }
                catch
                {
                    return (default(A), state, true);
                }
            });
    }
}
