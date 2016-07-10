using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using LanguageExt.TypeClasses;

namespace LanguageExt
{
    public static partial class TypeClassExtensions
    {
        /// <summary>
        /// Monadic bind
        /// </summary>
        /// <typeparam name="T">Type of the bound value</typeparam>
        /// <typeparam name="U">Type of the bound return value</typeparam>
        /// <param name="self">Monad to bind</param>
        /// <param name="bind">Bind function</param>
        /// <returns>Monad of U</returns>
        [Pure]
        public static Monad<U> Bind<T, U>(this Monad<T> self, Func<T, Monad<U>> bind) =>
            self.Bind(self, bind);

        /// <summary>
        /// Monadic bind and project
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static Monad<V> SelectMany<T, U, V>(
            this Monad<T> self,
            Func<T, Monad<U>> bind,
            Func<T, U, V> project) =>
            self.Bind(self,
                t => bind(t).Select(
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
        public static Applicative<U> Bind<T, U>(this Applicative<T> self, Func<T, Applicative<U>> bind) =>
            self.Bind(self, bind);

        /// <summary>
        /// Applicative bind and project
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static Applicative<V> SelectMany<T, U, V>(
            this Applicative<T> self,
            Func<T, Applicative<U>> bind,
            Func<T, U, V> project) =>
            self.Bind(self,
                t => bind(t).Select(
                    u => project(t, u)));

        /// <summary>
        /// Monad join
        /// </summary>
        public static Monad<V> Join<T, U, K, V>(
            this Monad<T> self,
            Monad<U> inner,
            Func<T, K> outerKeyMap,
            Func<U, K> innerKeyMap,
            Func<T, U, V> project) =>
            from w in
                (from x in self
                 from y in inner
                 select EqualityComparer<K>.Default.Equals(outerKeyMap(x), innerKeyMap(y))
                    ?  self.Select(_ => project(x, y))
                    : (self.Select(_ => default(V))).Fail())
            from res in w
            select res;

        /// <summary>
        /// Produce a failure value
        /// </summary>
        [Pure]
        public static Monad<A> Fail<A>(this Monad<A> ma, string err = "") =>
            ma.Fail(err);
    }
}
