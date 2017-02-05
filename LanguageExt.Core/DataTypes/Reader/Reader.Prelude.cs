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
            default(MReader<SReader<Env, A>, Reader<Env, A>, Env, A>).Return(value);

        /// <summary>
        /// Retrieves the reader monad environment.
        /// </summary>
        /// <typeparam name="Env">Environment</typeparam>
        /// <returns>Reader monad with the environment in as the bound value</returns>
        [Pure]
        public static Reader<Env, Env> ask<Env>() =>
            default(MReader<SReader<Env, Env>, Reader<Env, Env>, Env, Env>).Ask<SReader<Env, Env>, Reader<Env, Env>>();

        /// <summary>
        /// Retrieves a function of the current environment.
        /// </summary>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="R">Bound and mapped value type</typeparam>
        /// <returns>Reader monad with the mapped environment in as the bound value</returns>
        [Pure]
        public static Reader<Env, R> asks<Env, R>(Func<Env, R> f) =>
            default(MReader<SReader<Env, R>, Reader<Env, R>, Env, R>).Reader(f);

        /// <summary>
        /// Executes a computation in a modified environment
        /// </summary>
        /// <param name="f">The function to modify the environment.</param>
        /// <param name="m">Reader to modify</param>
        /// <returns>Modified reader</returns>
        [Pure]
        public static Reader<Env, A> local<Env, A>(Func<Env, Env> f, Reader<Env, A> ma) =>
            default(MReader<SReader<Env, A>, Reader<Env, A>, Env, A>).Local(f, ma);

        /// <summary>
        /// Chooses the first monad result that has a Some(x) for the value
        /// </summary>
        [Pure]
        public static Reader<Env, Option<A>> choose<Env, A>(params Reader<Env, Option<A>>[] monads) =>
            default(MReader<SReader<Env, Option<A>>, Reader<Env, Option<A>>, Env, Option<A>>).Return(state =>
            {
                foreach (var monad in monads)
                {
                    var (x, s, bottom) = monad.Eval(state);
                    if (!bottom && x.IsSome)
                    {
                        return (x, s, bottom);
                    }
                }
                return (default(A), state, true);
            });

        /// <summary>
        /// Run the reader and catch exceptions
        /// </summary>
        [Pure]
        public static Reader<Env, A> tryread<Env, A>(Reader<Env, A> m) =>
            default(MReader<SReader<Env, A>, Reader<Env, A>, Env, A>).Return(state =>
            {
                try
                {
                    return m.Eval(state);
                }
                catch
                {
                    return (default(A), state, true);
                }
            });
    }
}
