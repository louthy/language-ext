using System;
using System.Collections.Generic;

namespace LanguageExt.TypeClasses
{
    /// <summary>
    /// Monad type-class
    /// </summary>
    /// <typeparam name="A">Bound value</typeparam>
    [Typeclass]
    public interface Monad<A> : Functor<A>, Foldable<A>
    {
        /// <summary>
        /// Monad return
        /// </summary>
        /// <typeparam name="A">Type of the bound monad value</typeparam>
        /// <param name="x">The bound monad value</param>
        /// <returns>Monad of A</returns>
        Monad<A> Return(A x, params A[] xs);

        /// <summary>
        /// Monad return
        /// </summary>
        /// <typeparam name="A">Type of the bound monad value</typeparam>
        /// <param name="x">The bound monad value(s)</param>
        /// <returns>Monad of A</returns>
        Monad<A> Return(IEnumerable<A> xs);

        /// <summary>
        /// Monadic bind
        /// </summary>
        /// <typeparam name="MB">Type of the monad to return</typeparam>
        /// <typeparam name="B">Type of the bound return value</typeparam>
        /// <param name="ma">Monad to bind</param>
        /// <param name="f">Bind function</param>
        /// <returns>Monad of type MB derived from Monad of B</returns>
        MB Bind<MB, B>(Monad<A> ma, Func<A, MB> f) where MB : struct, Monad<B>;

        /// <summary>
        /// Monadic bind
        /// </summary>
        /// <typeparam name="B">Type of the bound return value</typeparam>
        /// <param name="ma">Monad to bind</param>
        /// <param name="f">Bind function</param>
        /// <returns>Monad of B</returns>
        Monad<B> Bind<B>(Monad<A> ma, Func<A, Monad<B>> f);

        /// <summary>
        /// Produce a failure value
        /// </summary>
        Monad<A> Fail(Exception err = null);

        /// <summary>
        /// Produce a failure value
        /// </summary>
        Monad<A> Fail<F>(F err = default(F));
    }
}
