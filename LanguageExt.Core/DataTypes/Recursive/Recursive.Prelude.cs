using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageExt.DataTypes.Recursive;

namespace LanguageExt
{
    public static partial class Prelude
    {

        /// <summary>
        /// Returns the base of the recursion
        /// </summary>
        /// <typeparam name="A"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Recursive<A> Return<A>(A a) => new Recursive<A>(a);


        /// <summary>
        /// Creates the next general case of the recursion
        /// </summary>
        /// <typeparam name="A"></typeparam>        
        /// <returns></returns>
        public static Recursive<A> Recurse<A>(Func<Recursive<A>> func) =>
            new Recursive<A>(func);

        /// <summary>
        /// Creates the next general case of the recursion
        /// </summary>
        /// <typeparam name="A"></typeparam>
        /// <typeparam name="B"></typeparam>
        /// <param name="func"></param>
        /// <param name="a"></param>
        /// <returns></returns>
        public static Recursive<B> Recurse<A, B>(Func<A, Recursive<B>> func, A a) =>
            new Recursive<B>(() => func(a));

        /// <summary>
        /// Creates the next general case of the recursion
        /// </summary>
        /// <typeparam name="A"></typeparam>
        /// <typeparam name="B"></typeparam>
        /// <typeparam name="C"></typeparam>
        /// <param name="func"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Recursive<C> Recurse<A, B, C>(Func<A, B, Recursive<C>> func, A a, B b) =>
            new Recursive<C>(() => func(a, b));
        
        /// <summary>
        /// Creates the next general case of the recursion
        /// </summary>
        /// <typeparam name="A"></typeparam>
        /// <typeparam name="B"></typeparam>
        /// <typeparam name="C"></typeparam>
        /// <typeparam name="D"></typeparam>
        /// <param name="func"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public static Recursive<D> Recurse<A, B, C, D>(Func<A, B, C, Recursive<D>> func, A a, B b, C c) =>
            new Recursive<D>(() => func(a, b, c));

        /// <summary>
        /// Creates the next general case of the recursion
        /// </summary>
        /// <typeparam name="A"></typeparam>
        /// <typeparam name="B"></typeparam>
        /// <typeparam name="C"></typeparam>
        /// <typeparam name="D"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <param name="func"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="d"></param>
        /// <returns></returns>
        public static Recursive<E> Recurse<A, B, C, D, E>(Func<A, B, C, D, Recursive<E>> func, A a, B b, C c, D d) =>
            new Recursive<E>(() => func(a, b, c, d));

        /// <summary>
        /// Creates the next general case of the recursion
        /// </summary>
        /// <typeparam name="A"></typeparam>
        /// <typeparam name="B"></typeparam>
        /// <typeparam name="C"></typeparam>
        /// <typeparam name="D"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <param name="func"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="d"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        public static Recursive<F> Recurse<A, B, C, D, E, F>(Func<A, B, C, D, E, Recursive<F>> func, A a, B b, C c, D d, E e) =>
            new Recursive<F>(() => func(a, b, c, d, e));

        /// <summary>
        /// Creates the next general case of the recursion
        /// </summary>
        /// <typeparam name="A"></typeparam>
        /// <typeparam name="B"></typeparam>
        /// <typeparam name="C"></typeparam>
        /// <typeparam name="D"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <typeparam name="G"></typeparam>
        /// <param name="func"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="d"></param>
        /// <param name="e"></param>
        /// <param name="f"></param>
        /// <returns></returns>
        public static Recursive<G> Recurse<A, B, C, D, E, F, G>(Func<A, B, C, D, E, F, Recursive<G>> func, A a, B b, C c, D d, E e, F f) =>
            new Recursive<G>(() => func(a, b, c, d, e, f));


        /// <summary>
        /// Creates the next general case of the recursion
        /// </summary>
        /// <typeparam name="A"></typeparam>
        /// <typeparam name="B"></typeparam>
        /// <typeparam name="C"></typeparam>
        /// <typeparam name="D"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <typeparam name="G"></typeparam>
        /// <typeparam name="H"></typeparam>
        /// <param name="func"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="d"></param>
        /// <param name="e"></param>
        /// <param name="f"></param>
        /// <param name="g"></param>
        /// <returns></returns>
        public static Recursive<H> Recurse<A, B, C, D, E, F, G, H>(Func<A, B, C, D, E, F, G, Recursive<H>> func, A a, B b, C c, D d, E e, F f, G g) =>
            new Recursive<H>(() => func(a, b, c, d, e, f, g));


        /// <summary>
        /// Creates the next general case of the recursion
        /// </summary>
        /// <typeparam name="A"></typeparam>
        /// <typeparam name="B"></typeparam>
        /// <typeparam name="C"></typeparam>
        /// <typeparam name="D"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <typeparam name="G"></typeparam>
        /// <typeparam name="H"></typeparam>
        /// <typeparam name="I"></typeparam>
        /// <param name="func"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="d"></param>
        /// <param name="e"></param>
        /// <param name="f"></param>
        /// <param name="g"></param>
        /// <param name="h"></param>
        /// <returns></returns>
        public static Recursive<I> Recurse<A, B, C, D, E, F, G, H, I>(Func<A, B, C, D, E, F, G, H, Recursive<I>> func, A a, B b, C c, D d, E e, F f, G g, H h) =>
            new Recursive<I>(() => func(a, b, c, d, e, f, g, h));

        /// <summary>
        /// Creates the next general case of the recursion
        /// </summary>
        /// <typeparam name="A"></typeparam>
        /// <typeparam name="B"></typeparam>
        /// <typeparam name="C"></typeparam>
        /// <typeparam name="D"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <typeparam name="G"></typeparam>
        /// <typeparam name="H"></typeparam>
        /// <typeparam name="I"></typeparam>
        /// <typeparam name="J"></typeparam>
        /// <param name="func"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="d"></param>
        /// <param name="e"></param>
        /// <param name="f"></param>
        /// <param name="g"></param>
        /// <param name="h"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        public static Recursive<J> Recurse<A, B, C, D, E, F, G, H, I, J>(Func<A, B, C, D, E, F, G, H, I, Recursive<J>> func, A a, B b, C c, D d, E e, F f, G g, H h, I i) =>
            new Recursive<J>(() => func(a, b, c, d, e, f, g, h, i));

        /// <summary>
        /// Creates the next general case of the recursion
        /// </summary>
        /// <typeparam name="A"></typeparam>
        /// <typeparam name="B"></typeparam>
        /// <typeparam name="C"></typeparam>
        /// <typeparam name="D"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <typeparam name="G"></typeparam>
        /// <typeparam name="H"></typeparam>
        /// <typeparam name="I"></typeparam>
        /// <typeparam name="J"></typeparam>
        /// <typeparam name="K"></typeparam>
        /// <param name="func"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="d"></param>
        /// <param name="e"></param>
        /// <param name="f"></param>
        /// <param name="g"></param>
        /// <param name="h"></param>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        public static Recursive<K> Recurse<A, B, C, D, E, F, G, H, I, J, K>(Func<A, B, C, D, E, F, G, H, I, J, Recursive<K>> func, A a, B b, C c, D d, E e, F f, G g, H h, I i, J j) =>
            new Recursive<K>(() => func(a, b, c, d, e, f, g, h, i, j));

        /// <summary>
        /// Creates the next general case of the recursion
        /// </summary>
        /// <typeparam name="A"></typeparam>
        /// <typeparam name="B"></typeparam>
        /// <typeparam name="C"></typeparam>
        /// <typeparam name="D"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <typeparam name="G"></typeparam>
        /// <typeparam name="H"></typeparam>
        /// <typeparam name="I"></typeparam>
        /// <typeparam name="J"></typeparam>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="L"></typeparam>
        /// <param name="func"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="d"></param>
        /// <param name="e"></param>
        /// <param name="f"></param>
        /// <param name="g"></param>
        /// <param name="h"></param>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        public static Recursive<L> Recurse<A, B, C, D, E, F, G, H, I, J, K, L>(Func<A, B, C, D, E, F, G, H, I, J, K, Recursive<L>> func, A a, B b, C c, D d, E e, F f, G g, H h, I i, J j, K k) =>
            new Recursive<L>(() => func(a, b, c, d, e, f, g, h, i, j, k));


        /// <summary>
        /// Creates the next general case of the recursion
        /// </summary>
        /// <typeparam name="A"></typeparam>
        /// <typeparam name="B"></typeparam>
        /// <typeparam name="C"></typeparam>
        /// <typeparam name="D"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <typeparam name="G"></typeparam>
        /// <typeparam name="H"></typeparam>
        /// <typeparam name="I"></typeparam>
        /// <typeparam name="J"></typeparam>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="L"></typeparam>
        /// <typeparam name="M"></typeparam>
        /// <param name="func"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="d"></param>
        /// <param name="e"></param>
        /// <param name="f"></param>
        /// <param name="g"></param>
        /// <param name="h"></param>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <param name="k"></param>
        /// <param name="l"></param>
        /// <returns></returns>
        public static Recursive<M> Recurse<A, B, C, D, E, F, G, H, I, J, K, L, M>(Func<A, B, C, D, E, F, G, H, I, J, K, L, Recursive<M>> func, A a, B b, C c, D d, E e, F f, G g, H h, I i, J j, K k, L l) =>
            new Recursive<M>(() => func(a, b, c, d, e, f, g, h, i, j, k, l));

        /// <summary>
        /// Creates the next general case of the recursion
        /// </summary>
        /// <typeparam name="A"></typeparam>
        /// <typeparam name="B"></typeparam>
        /// <typeparam name="C"></typeparam>
        /// <typeparam name="D"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <typeparam name="G"></typeparam>
        /// <typeparam name="H"></typeparam>
        /// <typeparam name="I"></typeparam>
        /// <typeparam name="J"></typeparam>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="L"></typeparam>
        /// <typeparam name="M"></typeparam>
        /// <typeparam name="N"></typeparam>
        /// <param name="func"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="d"></param>
        /// <param name="e"></param>
        /// <param name="f"></param>
        /// <param name="g"></param>
        /// <param name="h"></param>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <param name="k"></param>
        /// <param name="l"></param>
        /// <param name="m"></param>
        /// <returns></returns>
        public static Recursive<N> Recurse<A, B, C, D, E, F, G, H, I, J, K, L, M, N>(Func<A, B, C, D, E, F, G, H, I, J, K, L, M, Recursive<N>> func, A a, B b, C c, D d, E e, F f, G g, H h, I i, J j, K k, L l, M m) =>
            new Recursive<N>(() => func(a, b, c, d, e, f, g, h, i, j, k, l, m));


        /// <summary>
        /// Creates the next general case of the recursion
        /// </summary>
        /// <typeparam name="A"></typeparam>
        /// <typeparam name="B"></typeparam>
        /// <typeparam name="C"></typeparam>
        /// <typeparam name="D"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <typeparam name="G"></typeparam>
        /// <typeparam name="H"></typeparam>
        /// <typeparam name="I"></typeparam>
        /// <typeparam name="J"></typeparam>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="L"></typeparam>
        /// <typeparam name="M"></typeparam>
        /// <typeparam name="N"></typeparam>
        /// <typeparam name="O"></typeparam>
        /// <param name="func"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="d"></param>
        /// <param name="e"></param>
        /// <param name="f"></param>
        /// <param name="g"></param>
        /// <param name="h"></param>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <param name="k"></param>
        /// <param name="l"></param>
        /// <param name="m"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public static Recursive<O> Recurse<A, B, C, D, E, F, G, H, I, J, K, L, M, N, O>(Func<A, B, C, D, E, F, G, H, I, J, K, L, M, N, Recursive<O>> func, A a, B b, C c, D d, E e, F f, G g, H h, I i, J j, K k, L l, M m, N n) =>
            new Recursive<O>(() => func(a, b, c, d, e, f, g, h, i, j, k, l, m, n));


        /// <summary>
        /// Creates the next general case of the recursion
        /// </summary>
        /// <typeparam name="A"></typeparam>
        /// <typeparam name="B"></typeparam>
        /// <typeparam name="C"></typeparam>
        /// <typeparam name="D"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <typeparam name="G"></typeparam>
        /// <typeparam name="H"></typeparam>
        /// <typeparam name="I"></typeparam>
        /// <typeparam name="J"></typeparam>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="L"></typeparam>
        /// <typeparam name="M"></typeparam>
        /// <typeparam name="N"></typeparam>
        /// <typeparam name="O"></typeparam>
        /// <typeparam name="P"></typeparam>
        /// <param name="func"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="d"></param>
        /// <param name="e"></param>
        /// <param name="f"></param>
        /// <param name="g"></param>
        /// <param name="h"></param>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <param name="k"></param>
        /// <param name="l"></param>
        /// <param name="m"></param>
        /// <param name="n"></param>
        /// <param name="o"></param>
        /// <returns></returns>
        public static Recursive<P> Recurse<A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P>(Func<A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, Recursive<P>> func, A a, B b, C c, D d, E e, F f, G g, H h, I i, J j, K k, L l, M m, N n, O o) =>
            new Recursive<P>(() => func(a, b, c, d, e, f, g, h, i, j, k, l, m, n, o));

        /// <summary>
        /// Creates the next general case of the recursion
        /// </summary>
        /// <typeparam name="A"></typeparam>
        /// <typeparam name="B"></typeparam>
        /// <typeparam name="C"></typeparam>
        /// <typeparam name="D"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <typeparam name="G"></typeparam>
        /// <typeparam name="H"></typeparam>
        /// <typeparam name="I"></typeparam>
        /// <typeparam name="J"></typeparam>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="L"></typeparam>
        /// <typeparam name="M"></typeparam>
        /// <typeparam name="N"></typeparam>
        /// <typeparam name="O"></typeparam>
        /// <typeparam name="P"></typeparam>
        /// <typeparam name="Q"></typeparam>
        /// <param name="func"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="d"></param>
        /// <param name="e"></param>
        /// <param name="f"></param>
        /// <param name="g"></param>
        /// <param name="h"></param>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <param name="k"></param>
        /// <param name="l"></param>
        /// <param name="m"></param>
        /// <param name="n"></param>
        /// <param name="o"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        public static Recursive<Q> Recurse<A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q>(Func<A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Recursive<Q>> func, A a, B b, C c, D d, E e, F f, G g, H h, I i, J j, K k, L l, M m, N n, O o, P p) =>
            new Recursive<Q>(() => func(a, b, c, d, e, f, g, h, i, j, k, l, m, n, o, p));
    }
}
