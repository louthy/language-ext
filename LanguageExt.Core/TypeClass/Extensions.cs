using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Linq;
using LanguageExt.TypeClass;

namespace LanguageExt
{
    public static class TypeClassExtensions
    {
        /// <summary>
        /// Projection from one value to another using f
        /// </summary>
        /// <typeparam name="T">Functor value type</typeparam>
        /// <typeparam name="U">Resulting functor value type</typeparam>
        /// <param name="x">Functor value to map from </param>
        /// <param name="f">Projection function</param>
        /// <returns>Mapped functor</returns>
        [Pure]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static Functor<U> Select<T, U>(
            this Functor<T> self,
            Func<T, U> map
            ) =>
            self.Map(self, map);

        /// <summary>
        /// Projection from one value to another using f
        /// </summary>
        /// <typeparam name="T">Functor value type</typeparam>
        /// <typeparam name="U">Resulting functor value type</typeparam>
        /// <param name="x">Functor value to map from </param>
        /// <param name="f">Projection function</param>
        /// <returns>Mapped functor</returns>
        [Pure]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static AP<U> Select<T, U>(
            this AP<T> self,
            Func<T, U> map
            ) =>
            (AP<U>)self.Map(self, map);

        /// <summary>
        /// Projection from one value to another using f
        /// </summary>
        /// <typeparam name="T">Functor value type</typeparam>
        /// <typeparam name="U">Resulting functor value type</typeparam>
        /// <param name="x">Functor value to map from </param>
        /// <param name="f">Projection function</param>
        /// <returns>Mapped functor</returns>
        [Pure]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static M<U> Select<T, U>(
            this M<T> self,
            Func<T, U> map
            ) =>
            (M<U>)self.Map(self, map);

        /// <summary>
        /// Projection from one value to another using f
        /// </summary>
        /// <typeparam name="T">Functor value type</typeparam>
        /// <typeparam name="U">Resulting functor value type</typeparam>
        /// <param name="x">Functor value to map from </param>
        /// <param name="f">Projection function</param>
        /// <returns>Mapped functor</returns>
        [Pure]
        public static Functor<U> Map<T, U>(
            this Functor<T> self,
            Func<T, U> map
            ) =>
            self.Map(self, map);

        /// <summary>
        /// Projection from one value to another using f
        /// </summary>
        /// <typeparam name="T">Functor value type</typeparam>
        /// <typeparam name="U">Resulting functor value type</typeparam>
        /// <param name="x">Functor value to map from </param>
        /// <param name="f">Projection function</param>
        /// <returns>Mapped functor</returns>
        [Pure]
        public static AP<U> Map<T, U>(
            this AP<T> self,
            Func<T, U> map
            ) =>
            (AP<U>)self.Map(self, map);

        /// <summary>
        /// Projection from one value to another using f
        /// </summary>
        /// <typeparam name="T">Functor value type</typeparam>
        /// <typeparam name="U">Resulting functor value type</typeparam>
        /// <param name="x">Functor value to map from </param>
        /// <param name="f">Projection function</param>
        /// <returns>Mapped functor</returns>
        [Pure]
        public static M<U> Map<T, U>(
            this M<T> self,
            Func<T, U> map
            ) =>
            (M<U>)self.Map(self, map);

        /// <summary>
        /// Monadic bind
        /// </summary>
        /// <typeparam name="T">Type of the bound value</typeparam>
        /// <typeparam name="U">Type of the bound return value</typeparam>
        /// <param name="self">Monad to bind</param>
        /// <param name="bind">Bind function</param>
        /// <returns>Monad of U</returns>
        [Pure]
        public static M<U> Bind<T, U>(this M<T> self, Func<T, M<U>> bind) =>
            self.Bind(self, bind);

        /// <summary>
        /// Monadic bind and project
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static M<V> SelectMany<T, U, V>(
            this M<T> self,
            Func<T, M<U>> bind,
            Func<T, U, V> project) =>
            self.Bind(self,
                t => (M<V>)bind(t).Select(
                    u => project(t, u)));

        /// <summary>
        /// Monadic bind
        /// </summary>
        /// <typeparam name="T">Type of the bound value</typeparam>
        /// <typeparam name="U">Type of the bound return value</typeparam>
        /// <param name="self">Monad to bind</param>
        /// <param name="bind">Bind function</param>
        /// <returns>Monad of U</returns>
        [Pure]
        public static AP<U> Bind<T, U>(this AP<T> self, Func<T, AP<U>> bind) =>
            self.Bind(self, bind);

        /// <summary>
        /// Applicative bind and project
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static AP<V> SelectMany<T, U, V>(
            this AP<T> self,
            Func<T, AP<U>> bind,
            Func<T, U, V> project) =>
            self.Bind(self,
                t => (AP<V>)bind(t).Select(
                    u => project(t, u)));

        /// <summary>
        /// Monad join
        /// </summary>
        public static M<V> Join<T, U, K, V>(
            this M<T> self,
            M<U> inner,
            Func<T, K> outerKeyMap,
            Func<U, K> innerKeyMap,
            Func<T, U, V> project) =>
            from w in
                (from x in self
                 from y in inner
                 select EqualityComparer<K>.Default.Equals(outerKeyMap(x), innerKeyMap(y))
                    ? (M<V>)self.Select(_ => project(x, y))
                    : ((M<V>)self.Select(_ => default(V))).Fail())
            from res in w
            select res;

        /// <summary>
        /// Produce a failure value
        /// </summary>
        [Pure]
        public static M<A> Fail<A>(this M<A> ma, string err = "") =>
            ma.Fail(err);

        /// <summary>
        /// Sequential application
        /// 
        ///     f(a -> b) -> f a -> f b
        /// </summary>
        [Pure]
        public static AP<U> Apply<T, U>(this AP<Func<T, U>> x, AP<T> y) =>
            y.Apply(x, y);

        /// <summary>
        /// Sequential application
        /// 
        ///     f(a -> b -> c) -> f a -> f b -> f c
        /// </summary>
        [Pure]
        public static AP<V> Apply<T, U, V>(this AP<Func<T, U, V>> x, AP<T> y, AP<U> z) =>
            y.Apply(x, y, z);

        /// <summary>
        /// Sequential application
        /// 
        ///     f(a -> b -> c) -> f a -> f(b -> c)
        /// </summary>
        [Pure]
        public static AP<Func<U, V>> Apply<T, U, V>(this AP<Func<T, Func<U, V>>> x, AP<T> y) =>
            y.Apply(x, y);

        /// <summary>
        /// Sequential actions
        /// 
        ///     f a -> f b -> f b
        /// </summary>
        [Pure]
        public static AP<U> Action<T, U>(this AP<T> x, AP<U> y) =>
            x.Action(x, y);

        public static Option<A> AsOption<A>(this AP<A> a) => (Option<A>)a;
        public static Option<A> AsOption<A>(this M<A> a) => (Option<A>)a;
    }
}
