using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using LanguageExt.TypeClasses;
using System.Threading.Tasks;

namespace LanguageExt
{
    public static partial class Prelude
    {
        /// <summary>
        /// Apply function to argument
        /// </summary>
        public static B pipe<A, B>(A x, Func<A, B> f) => 
            f(x);

        /// <summary>
        /// Apply function to argument
        /// </summary>
        public static C pipe<A, B, C>(A x, Func<A, B> f, Func<B, C> g) => 
            g(f(x));

        /// <summary>
        /// Apply function to argument
        /// </summary>
        public static D pipe<A, B, C, D>(A x, Func<A, B> f, Func<B, C> g, Func<C, D> h) => 
            h(g(f(x)));

        /// <summary>
        /// Apply function to argument
        /// </summary>
        public static E pipe<A, B, C, D, E>(A x, Func<A, B> f, Func<B, C> g, Func<C, D> h, Func<D, E> i) =>
            i(h(g(f(x))));

        /// <summary>
        /// Apply function to argument
        /// </summary>
        public static F pipe<A, B, C, D, E, F>(A x, Func<A, B> f, Func<B, C> g, Func<C, D> h, Func<D, E> i, Func<E, F> j) =>
            j(i(h(g(f(x)))));

        /// <summary>
        /// Apply function to argument
        /// </summary>
        public static G pipe<A, B, C, D, E, F, G>(A x, Func<A, B> f, Func<B, C> g, Func<C, D> h, Func<D, E> i, Func<E, F> j, Func<F, G> k) =>
            k(j(i(h(g(f(x))))));

        /// <summary>
        /// Apply function to argument
        /// </summary>
        public static H pipe<A, B, C, D, E, F, G, H>(A x, Func<A, B> f, Func<B, C> g, Func<C, D> h, Func<D, E> i, Func<E, F> j, Func<F, G> k, Func<G, H> l) =>
            l(k(j(i(h(g(f(x)))))));

        /// <summary>
        /// Apply function to argument
        /// </summary>
        public static I pipe<A, B, C, D, E, F, G, H, I>(A x, Func<A, B> f, Func<B, C> g, Func<C, D> h, Func<D, E> i, Func<E, F> j, Func<F, G> k, Func<G, H> l, Func<H, I> m) =>
            m(l(k(j(i(h(g(f(x))))))));
    }
}
