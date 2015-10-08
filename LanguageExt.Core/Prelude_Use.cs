using System;
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
        public static Try<LinqDisposable<T>> use<T>(Try<T> computation)
            where T : IDisposable =>
            computation.Map(x => new LinqDisposable<T>(x));

        /// <summary>
        /// Use with Try monad in LINQ expressions to auto-clean up disposable items
        /// </summary>
        public static LinqDisposable<T> use<T>(T disposable)
            where T : IDisposable
        {
            return new LinqDisposable<T>(disposable);
        }

        /// <summary>
        /// Use with Try monad in LINQ expressions to auto-clean up disposable items
        /// </summary>
        public static LinqDisposable<T> use<T>(Func<T> generator)
            where T : IDisposable
        {
            return new LinqDisposable<T>(generator());
        }

        /// <summary>
        /// Functional implementation of the using(...) { } pattern
        /// </summary>
        /// <param name="generator">Disposable to use</param>
        /// <param name="f">Inner map function that uses the disposable value</param>
        /// <returns>Result of f(disposable)</returns>
        public static R use<T, R>(Func<T> generator, Func<T, R> f)
            where T : IDisposable
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
