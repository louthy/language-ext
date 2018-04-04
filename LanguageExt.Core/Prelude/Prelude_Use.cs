﻿using System;
using System.Threading.Tasks;

namespace LanguageExt
{
    public static partial class Prelude
    {
        /// <summary>
        /// Use with Try monad in LINQ expressions to auto-clean up disposable items
        /// </summary>
        public static Try<U> use<T, U>(Try<T> computation, Func<T, U> map) where T : class, IDisposable =>
            computation.Use(map);

        /// <summary>
        /// Use with Try monad in LINQ expressions to auto-clean up disposable items
        /// </summary>
        public static Try<U> use<T, U>(Try<T> computation, Func<T, Try<U>> bind) where T : class, IDisposable =>
            computation.Use(bind);

        /// <summary>
        /// Use with Task in LINQ expressions to auto-clean up disposable items
        /// </summary>
        public static Task<U> use<T, U>(Task<T> computation, Func<T, U> map) where T : class, IDisposable =>
            computation.Map(d => use(d, map));

        /// <summary>
        /// Use with Task in LINQ expressions to auto-clean up disposable items
        /// </summary>
        public static async Task<U> use<T, U>(Task<T> computation, Func<T, Task<U>> bind) where T : class, IDisposable
        {
            var t = await computation;
            try
            {
                return await bind(t);
            }
            finally
            {
                t?.Dispose();
            }
        }

        /// <summary>
        /// Functional implementation of the using(...) { } pattern
        /// </summary>
        /// <param name="generator">Disposable to use</param>
        /// <param name="f">Inner map function that uses the disposable value</param>
        /// <returns>Result of f(disposable)</returns>
        public static R use<T, R>(Func<T> generator, Func<T, R> f)
            where T : class, IDisposable =>
            generator().Apply(d => use(d, f));

        /// <summary>
        /// Functional implementation of the using(...) { } pattern
        /// </summary>
        /// <param name="disposable">Disposable to use</param>
        /// <param name="f">Inner map function that uses the disposable value</param>
        /// <returns>Result of f(disposable)</returns>
        public static R use<T, R>(T disposable, Func<T, R> f)
            where T : IDisposable
        {
            try
            {
                return f(disposable);
            }
            finally
            {
                disposable?.Dispose();
            }
        }

        /// <summary>
        /// Functional implementation of the using(...) { } pattern
        /// </summary>
        /// <param name="disposable">Disposable to use</param>
        /// <param name="f">Inner map function that uses the disposable value</param>
        /// <returns>Result of f(disposable)</returns>
        public static Try<R> tryuse<T, R>(Func<T> disposable, Func<T, R> f)
            where T : IDisposable =>
            Try(disposable)
                .Map(d => use(d, f));

        /// <summary>
        /// Functional implementation of the using(...) { } pattern
        /// </summary>
        /// <param name="disposable">Disposable to use</param>
        /// <param name="f">Inner map function that uses the disposable value</param>
        /// <returns>Result of f(disposable)</returns>
        public static Try<R> tryuse<T, R>(T disposable, Func<T, R> f)
            where T : IDisposable => () =>
            use(disposable, f);
    }
}
