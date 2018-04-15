using System;
using System.Threading.Tasks;

namespace LanguageExt
{
    public static partial class Prelude
    {
        /// <summary>
        /// Use with Try monad in LINQ expressions to auto-clean up disposable items
        /// </summary>
        public static Try<B> use<A, B>(Try<A> computation, Func<A, B> map)
            where A : class, IDisposable =>
            computation.Use(map);

        /// <summary>
        /// Use with Try monad in LINQ expressions to auto-clean up disposable items
        /// </summary>
        public static Try<B> use<A, B>(Try<A> computation, Func<A, Try<B>> bind)
            where A : class, IDisposable =>
            computation.Use(bind);

        /// <summary>
        /// Use with Task in LINQ expressions to auto-clean up disposable items
        /// </summary>
        public static Task<B> use<A, B>(Task<A> computation, Func<A, B> map)
            where A : class, IDisposable =>
            computation.Map(d => use(d, map));

        /// <summary>
        /// Use with Task in LINQ expressions to auto-clean up disposable items
        /// </summary>
        public static async Task<B> use<A, B>(Task<A> computation, Func<A, Task<B>> bind)
            where A : class, IDisposable
        {
            var a = await computation;
            try
            {
                return await bind(a);
            }
            finally
            {
                a?.Dispose();
            }
        }

        /// <summary>
        /// Functional implementation of the using(...) { } pattern
        /// </summary>
        /// <param name="generator">Disposable to use</param>
        /// <param name="f">Inner map function that uses the disposable value</param>
        /// <returns>Result of f(disposable)</returns>
        public static B use<A, B>(Func<A> generator, Func<A, B> f)
            where A : class, IDisposable
        {
            var value = generator();
            try
            {
                return f(value);
            }
            finally
            {
                value?.Dispose();
            }
        }

        /// <summary>
        /// Functional implementation of the using(...) { } pattern
        /// </summary>
        /// <param name="disposable">Disposable to use</param>
        /// <param name="f">Inner map function that uses the disposable value</param>
        /// <returns>Result of f(disposable)</returns>
        public static B use<A, B>(A disposable, Func<A, B> f)
            where A : IDisposable
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
        public static Try<B> tryuse<A, B>(Func<A> disposable, Func<A, B> f)
            where A : IDisposable =>
            Try(disposable)
                .Map(d => use(d, f));

        /// <summary>
        /// Functional implementation of the using(...) { } pattern
        /// </summary>
        /// <param name="disposable">Disposable to use</param>
        /// <param name="f">Inner map function that uses the disposable value</param>
        /// <returns>Result of f(disposable)</returns>
        public static Try<B> tryuse<A, B>(A disposable, Func<A, B> f)
            where A : IDisposable => () =>
            use(disposable, f);
    }
}
