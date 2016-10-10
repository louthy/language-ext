﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LanguageExt.Trans;

namespace LanguageExt
{
    public static partial class Prelude
    {
        /// <summary>
        /// Use with Try monad in LINQ expressions to auto-clean up disposable items
        /// </summary>
        public static Try<U> use<T, U>(Try<T> computation, Func<T, U> map) where T : class, IDisposable => () =>
        {
            var resT = computation.Try();
            if (resT.IsFaulted)
            {
                return new TryResult<U>(resT.Exception);
            }
            else
            {
                try
                {
                    var resU = map(resT.Value);
                    resT.Value?.Dispose();
                    return new TryResult<U>(resU);
                }
                catch(Exception e)
                {
                    return new TryResult<U>(e);
                }
                finally
                {
                    resT.Value?.Dispose();
                }
            }
        };

        /// <summary>
        /// Use with Try monad in LINQ expressions to auto-clean up disposable items
        /// </summary>
        public static Try<U> use<T, U>(Try<T> computation, Func<T, Try<U>> bind) where T : class, IDisposable => () =>
        {
            var resT = computation.Try();
            if (resT.IsFaulted)
            {
                return new TryResult<U>(resT.Exception);
            }
            else
            {
                try
                {
                    var resU = bind(resT.Value);
                    resT.Value?.Dispose();
                    return resU.Try();
                }
                catch (Exception e)
                {
                    return new TryResult<U>(e);
                }
                finally
                {
                    resT.Value?.Dispose();
                }
            }
        };

        /// <summary>
        /// Use with Task in LINQ expressions to auto-clean up disposable items
        /// </summary>
        public static async Task<U> use<T, U>(Task<T> computation, Func<T, U> map) where T : class, IDisposable
        {
            T t = null;
            try
            {
                t = await computation;
                return map(t);
            }
            finally
            {
                t?.Dispose();
            }
        }

        /// <summary>
        /// Use with Task in LINQ expressions to auto-clean up disposable items
        /// </summary>
        public static async Task<U> use<T, U>(Task<T> computation, Func<T, Task<U>> bind) where T : class, IDisposable
        {
            T t = null;
            try
            {
                t = await computation;
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
            where T : class, IDisposable
        {
            var value = generator();
            try
            {
                return f(value);
            }
            finally
            {
                value.Dispose();
            }
        }

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
                disposable.Dispose();
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
                .Map(v =>
                {
                    try
                    {
                        return f(v);
                    }
                    finally
                    {
                        v.Dispose();
                    }
                });

        /// <summary>
        /// Functional implementation of the using(...) { } pattern
        /// </summary>
        /// <param name="disposable">Disposable to use</param>
        /// <param name="f">Inner map function that uses the disposable value</param>
        /// <returns>Result of f(disposable)</returns>
        public static Try<R> tryuse<T, R>(T disposable, Func<T, R> f)
            where T : IDisposable => () =>
        {
            try
            {
                return f(disposable);
            }
            finally
            {
                disposable.Dispose();
            }
        };
    }
}
