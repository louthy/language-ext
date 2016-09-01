using System;

namespace LanguageExt.TypeClasses
{
    /// <summary>
    /// Monad type-class
    /// </summary>
    /// <typeparam name="A">Bound value</typeparam>
    [Typeclass]
    public interface MonadT<MA, A> : FunctorT<MA, A>, FoldableT<MA, A> where MA : struct, Monad<A>, Foldable<A>
    {
        /// <summary>
        /// Monad return
        /// </summary>
        /// <typeparam name="A">Type of the bound monad value</typeparam>
        /// <param name="x">The bound monad value</param>
        /// <returns>Monad of A</returns>
        MonadT<MA, A> Return(MA x);

        /// <summary>
        /// Monadic bind
        /// </summary>
        /// <typeparam name="B">Type of the bound return value</typeparam>
        /// <param name="ma">Monad to bind</param>
        /// <param name="f">Bind function</param>
        /// <returns>Monad of B</returns>
        MonadT<MB, B> Bind<MB, B>(MonadT<MA, A> ma, Func<A, MB> f) where MB : struct, Monad<B>;

        /// <summary>
        /// Produce a failure value
        /// </summary>
        MonadT<MA, A> Fail(string err = "");
    }
}
