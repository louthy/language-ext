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
        /// <typeparam name="A">Wrapped type</typeparam>
        /// <param name="value">Wrapped value</param>
        /// <returns>Reader monad</returns>
        [Pure]
        public static Reader<Env, A> Reader<Env, A>(A value) =>
            default(MReader<Env, A>).Return(_ => value);

        /// <summary>
        /// Reader monad constructor
        /// </summary>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="A">Wrapped type</typeparam>
        /// <param name="value">Wrapped value</param>
        /// <returns>Reader monad</returns>
        [Pure]
        public static Reader<Env, A> Reader<Env, A>(Func<Env, A> f) =>
            default(MReader<Env, A>).Return(f);

        /// <summary>
        /// Retrieves the reader monad environment.
        /// </summary>
        /// <typeparam name="Env">Environment</typeparam>
        /// <returns>Reader monad with the environment in as the bound value</returns>
        [Pure]
        public static Reader<Env, Env> ask<Env>() =>
            default(MReader<Env, Env>).Ask();

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
        public static Reader<Env, A> local<Env, A>(Reader<Env, A> ma, Func<Env, Env> f) =>
            default(MReader<Env, A>).Local(ma, f);

        /// <summary>
        /// Chooses the first monad result that has a Some(x) for the value
        /// </summary>
        [Pure]
        public static Reader<Env, Option<A>> choose<Env, A>(params Reader<Env, Option<A>>[] monads) =>
            state =>
            {
                foreach (var monad in monads)
                {
                    var (x, bottom) = monad(state);
                    if (!bottom && x.IsSome)
                    {
                        return (x, bottom);
                    }
                }
                return (default(A), true);
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
                catch
                {
                    return (default(A), true);
                }
            };

        [Pure]
        public static Try<Reader<Env, A>> tryfun<Env, A>(Reader<Env, A> ma) => () => 
            from x in ma
            select x;
    }
}
