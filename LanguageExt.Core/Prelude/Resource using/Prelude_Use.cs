using System;
using System.Threading.Tasks;
using LanguageExt.Effects;

namespace LanguageExt
{
    public static partial class Prelude
    {
        /// <summary>
        /// Create a new resource tracking monad: `Use`.  This monad works with
        /// other effect based monads that will automatically clean-up the resource
        /// when the computation is complete.
        /// </summary>
        /// <param name="generate">Resource generator</param>
        /// <param name="dispose">Resource disposer</param>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <returns>`Use` monad that works with other effect monads</returns>
        public static Use<A> use<A>(Func<A> generate, Func<A, Unit> dispose) =>
            Use.New(generate, dispose);
        
        /// <summary>
        /// Create a new resource tracking monad: `Use`.  This monad works with
        /// other effect based monads that will automatically clean-up the resource
        /// when the computation is complete.
        /// </summary>
        /// <param name="generate">Resource generator</param>
        /// <param name="dispose">Resource disposer</param>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <returns>`Use` monad that works with other effect monads</returns>
        public static Use<A> use<A>(Func<A> generate) where A : IDisposable =>
            Use.New(generate);

        /// <summary>
        /// Create a new `Release` monad that works with other, effectful, resource tracking
        /// monads.  The effect of composing this with those other monadic types
        /// is that any underlying tracked resource that is equal (through reference
        /// equality) will have its resource disposed.
        /// </summary>
        /// <param name="value">Resource to release</param>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <returns>Release monads</returns>
        public static Release<A> release<A>(A value) =>
            Release.New(value);

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
    }
}
