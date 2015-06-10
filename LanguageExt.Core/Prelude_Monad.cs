using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageExt
{
    public static partial class Prelude
    {
        /// <summary>
        /// Writer monad constructor
        /// </summary>
        /// <typeparam name="Out">Writer output</typeparam>
        /// <typeparam name="T">Wrapped type</typeparam>
        /// <param name="value">Wrapped value</param>
        /// <returns>Writer monad</returns>
        public static Writer<Out, T> Writer<Out, T>(T value) => () =>
              new WriterResult<Out, T>(value, new Out[0]);

        /// <summary>
        /// Writer monad constructor
        /// </summary>
        /// <typeparam name="Out">Writer output</typeparam>
        /// <typeparam name="T">Wrapped type</typeparam>
        /// <param name="value">Wrapped value</param>
        /// <param name="ws">Writer log</param>
        /// <returns>Writer monad</returns>
        public static Writer<Out, T> Writer<Out, T>(T value, IEnumerable<Out> ws) => () =>
              new WriterResult<Out, T>(value, ws);

        /// <summary>
        /// Writer monad 'tell'
        /// Adds an item to the writer's output
        /// </summary>
        /// <typeparam name="W"></typeparam>
        /// <param name="value"></param>
        /// <returns>Writer monad</returns>
        public static Writer<Out, Unit> tell<Out>(Out value) => () => 
            new WriterResult<Out, Unit>(Unit.Default, new Out[1] { value });

        /// <summary>
        /// Reader monad constructor
        /// </summary>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="T">Wrapped type</typeparam>
        /// <param name="value">Wrapped value</param>
        /// <returns>Reader monad</returns>
        public static Reader<Env, T> Reader<Env, T>(T value) =>
            env => value;

        /// <summary>
        /// Reader monad 'ask'
        /// Gets the 'environment' so it can be used 
        /// </summary>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="T">Wrapped type</typeparam>
        /// <returns>Reader monad with the environment in as the wrapped value</returns>
        public static Reader<Env, Env> ask<Env, T>() =>
            env => env;

        /// <summary>
        /// Reader monad 'ask'
        /// Gets the 'environment' and maps it
        /// </summary>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="T">Wrapped type</typeparam>
        /// <typeparam name="R">Mapped value</typeparam>
        /// <returns>Reader monad with the mapped environment in as the wrapped value</returns>
        public static Reader<Env, R> ask<Env, T, R>(Func<Env,R> map) =>
            env => map(env);

        /// <summary>
        /// Reader monad 'ask'
        /// Gets the 'environment' and maps it
        /// </summary>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="T">Wrapped type</typeparam>
        /// <returns>Reader monad with the mapped environment in as the wrapped value</returns>
        public static Reader<Env, T> ask<Env, T>(Func<Env, T> map) =>
            env => map(env);

        /// <summary>
        /// Executes a computation in a modified environment
        /// </summary>
        /// <param name="f">The function to modify the environment.</param>
        /// <param name="m">Reader to modify</param>
        /// <returns>Modified reader</returns>
        public static Reader<EnvLocal, T> local<Env, T, EnvLocal>(Func<EnvLocal, Env> f, Reader<Env, T> m) =>
            env => m(f(env));

        /// <summary>
        /// Executes a computation in a modified environment
        /// </summary>
        /// <param name="f">The function to modify the environment.</param>
        /// <param name="m">Reader to modify</param>
        /// <returns>Modified reader</returns>
        public static Reader<Env, T> local<Env, T>(Func<Env, Env> f, Reader<Env, T> m) =>
            env => m(f(env));

        /// <summary>
        /// State monad constructor
        /// </summary>
        /// <typeparam name="S">State type</typeparam>
        /// <typeparam name="T">Wrapped value type</typeparam>
        /// <param name="value">Value</param>
        /// <returns>State monad</returns>
        public static State<S, T> State<S, T>(T value) =>
            state => new StateResult<S, T>(state, value);

        /// <summary>
        /// Get the state from monad into its wrapped value
        /// </summary>
        /// <typeparam name="S">State type</typeparam>
        /// <returns>State monad with state in the value</returns>
        public static State<S, S> get<S>() =>
            state => new StateResult<S, S>(state, state);

        /// <summary>
        /// Set the state 
        /// </summary>
        /// <typeparam name="S">State type</typeparam>
        /// <returns>State monad with state set and with a Unit value</returns>
        public static State<S, Unit> put<S>(S state) =>
            _ => new StateResult<S, Unit>(state, unit);
    }
}
