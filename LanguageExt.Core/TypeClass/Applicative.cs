using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageExt.TypeClass
{
    /// <summary>
    /// Applicative functor
    /// </summary>
    /// <typeparam name="A"></typeparam>
    public interface Applicative<A> : Functor<A>
    {
        // Pure ::a -> f a
        Applicative<A> Pure(A a);

        /// <summary>
        /// Sequential application
        ///  f(a -> b) -> f a -> f b
        /// </summary>
        Applicative<B> Apply<B>(Applicative<Func<A, B>> x, Applicative<A> y);

        /// <summary>
        /// Sequential application
        ///  f(a -> b -> c) -> f a -> f b -> f c
        /// </summary>
        Applicative<C> Apply<B, C>(Applicative<Func<A, B, C>> x, Applicative<A> y, Applicative<B> z);

        /// <summary>
        /// Sequential application
        ///  f(a -> b -> c) -> f a -> f(b -> c)
        /// </summary>
        Applicative<Func<B,C>> Apply<B, C>(Applicative<Func<A, Func<B, C>>> x, Applicative<A> y);

        /// <summary>
        /// Sequential actions
        ///  f a -> f b -> f b
        /// </summary>
        Applicative<B> Action<B>(Applicative<A> x, Applicative<B> y);
    }
}
