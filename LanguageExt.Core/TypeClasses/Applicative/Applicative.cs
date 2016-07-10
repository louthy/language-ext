using System;
using System.Collections.Generic;
using static LanguageExt.Prelude;

namespace LanguageExt.TypeClasses
{
    /// <summary>
    /// Applicative functor type-class
    /// </summary>
    /// <typeparam name="A"></typeparam>
    public interface Applicative<A> : Functor<A>
    {
        /// <summary>
        /// Applicative construction
        /// 
        ///     a -> f a
        /// </summary>
        Applicative<A> Pure(A a);

        /// <summary>
        /// Sequential application
        /// 
        ///     f(a -> b) -> f a -> f b
        /// </summary>
        Applicative<B> Apply<B>(Applicative<Func<A, B>> x, Applicative<A> y);

        /// <summary>
        /// Sequential application
        /// 
        ///     f(a -> b -> c) -> f a -> f b -> f c
        /// </summary>
        Applicative<C> Apply<B, C>(Applicative<Func<A, B, C>> x, Applicative<A> y, Applicative<B> z);

        /// <summary>
        /// Sequential application
        /// 
        ///     f(a -> b -> c) -> f a -> f(b -> c)
        /// </summary>
        Applicative<Func<B,C>> Apply<B, C>(Applicative<Func<A, Func<B, C>>> x, Applicative<A> y);

        /// <summary>
        /// Sequential actions
        /// 
        ///     f a -> f b -> f b
        /// </summary>
        Applicative<B> Action<B>(Applicative<A> x, Applicative<B> y);

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
