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
            where A : IDisposable =>
            computation.Use(map);

        /// <summary>
        /// Use with Try monad in LINQ expressions to auto-clean up disposable items
        /// </summary>
        public static Try<B> use<A, B>(Try<A> computation, Func<A, Try<B>> bind)
            where A : IDisposable =>
            computation.Use(bind);

        /// <summary>
        /// Use with Task in LINQ expressions to auto-clean up disposable items
        /// </summary>
        public async static Task<B> use<A, B>(Task<A> computation, Func<A, B> map)
            where A : IDisposable =>
            use(await computation.ConfigureAwait(false), map);

        /// <summary>
        /// Use with Task in LINQ expressions to auto-clean up disposable items
        /// </summary>
        public static async Task<B> use<A, B>(Task<A> computation, Func<A, Task<B>> bind)
            where A : IDisposable =>
            await use(await computation.ConfigureAwait(false), bind).ConfigureAwait(false);

        /// <summary>
        /// Functional implementation of the using(...) { } pattern
        /// </summary>
        /// <param name="generator">Generator of disposable to use</param>
        /// <param name="asyncMap">Inner map function that uses the disposable value</param>
        /// <returns>Result of await asyncMap(generator())</returns>
        public static Task<B> use<A, B>(Func<A> generator, Func<A, Task<B>> asyncMap)
            where A : IDisposable =>
            use(generator(), asyncMap);

        /// <summary>
        /// Functional implementation of the using(...) { } pattern
        /// </summary>
        /// <param name="disposable">Disposable to use</param>
        /// <param name="asyncMap">Inner map function that uses the disposable value</param>
        /// <returns>Result of await asyncMap(disposable)</returns>
        public static async Task<B> use<A, B>(A disposable, Func<A, Task<B>> asyncMap)
            where A : IDisposable
        {
            try
            {
                return await asyncMap(disposable).ConfigureAwait(false);
            }
            finally
            {
                disposable?.Dispose();
            }
        }

        /// <summary>
        /// Functional implementation of the using(...) { } pattern
        /// </summary>
        /// <param name="generator">Disposable to use</param>
        /// <param name="f">Inner map function that uses the disposable value</param>
        /// <returns>Result of f(generator())</returns>
        public static B use<A, B>(Func<A> generator, Func<A, B> f)
            where A : IDisposable =>
            use(generator(), f);

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
            Try(disposable).Use(f);

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
