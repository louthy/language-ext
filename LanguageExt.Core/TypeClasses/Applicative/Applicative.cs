using System;
using System.Collections.Generic;
using static LanguageExt.Prelude;

namespace LanguageExt.TypeClasses
{
    /// <summary>
    /// Applicative functor type-class
    /// </summary>
    /// <typeparam name="A"></typeparam>
    [Typeclass]
    public interface Applicative<A> : Functor<A>
    {
        /// <summary>
        /// Applicative construction
        /// 
        ///     a -> f a
        /// </summary>
        Applicative<A> Pure(A x, params A[] xs);

        /// <summary>
        /// Monadic bind
        /// </summary>
        /// <typeparam name="B">Type of the bound return value</typeparam>
        /// <param name="ma">Monad to bind</param>
        /// <param name="f">Bind function</param>
        /// <returns>Monad of B</returns>
        Applicative<B> Bind<B>(Applicative<A> ma, Func<A, Applicative<B>> f);
    }
}
