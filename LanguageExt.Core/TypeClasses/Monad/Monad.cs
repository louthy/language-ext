using System;

namespace LanguageExt.TypeClasses
{
    /// <summary>
    /// Monad type-class
    /// </summary>
    /// <typeparam name="A">Bound value</typeparam>
    [Typeclass]
    public interface Monad<A> : Applicative<A>, Foldable<A>
    {
        /// <summary>
        /// Monad return
        /// </summary>
        /// <typeparam name="A">Type of the bound monad value</typeparam>
        /// <param name="x">The bound monad value</param>
        /// <returns>Monad of A</returns>
        Monad<A> Return(A x, params A[] xs);

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
        Monad<A> Fail(string err = "");
    }
}
