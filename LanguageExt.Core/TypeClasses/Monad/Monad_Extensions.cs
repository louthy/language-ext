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
        /// Projection from one value to another using f
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <typeparam name="B">Resulting bound value type</typeparam>
        /// <param name="ma">Monad value to map from </param>
        /// <param name="f">Projection function</param>
        /// <returns>Mapped monad</returns>
        [Pure]
        public static Monad<B> Select<A, B>(
            this Monad<A> ma,
            Func<A, B> f
            ) =>
            ma.Map<Monad<B>, A, B>(f);

        /// <summary>
        /// Projection from one value to another using f
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <typeparam name="B">Resulting bound value type</typeparam>
        /// <param name="ma">Monad value to map from </param>
        /// <param name="f">Projection function</param>
        /// <returns>Mapped monad</returns>
        public static Monad<B> LiftM<A, B>(
            this Monad<A> ma, 
            Func<A, B> f) =>
            ma.Map<Monad<B>, A, B>(f);

        /// <summary>
        /// Monadic bind
        /// </summary>
        /// <typeparam name="A">Type of the bound value</typeparam>
        /// <typeparam name="B">Type of the bound return value</typeparam>
        /// <param name="self">Monad to bind</param>
        /// <param name="bind">Bind function</param>
        /// <returns>Monad of U</returns>
        [Pure]
        public static Monad<B> Bind<A, B>(this Monad<A> self, Func<A, Monad<B>> bind) =>
            self.Bind(self, bind);

        /// <summary>
        /// Monadic bind and project
        /// </summary>
        public static Monad<C> SelectMany<A, B, C>(
            this Monad<A> self,
            Func<A, Monad<B>> bind,
            Func<A, B, C> project)
            =>
            self.Bind(self,
                t => bind(t).Select(
                    u => project(t, u)));

        /// <summary>
        /// Monad join
        /// </summary>
        public static Monad<D> Join<A, B, C, D>(
            this Monad<A> self,
            Monad<B> inner,
            Func<A, C> outerKeyMap,
            Func<B, C> innerKeyMap,
            Func<A, B, D> project) =>
            from w in
                (from x in self
                 from y in inner.Bind(y =>
                    EqualityComparer<C>.Default.Equals(outerKeyMap(x), innerKeyMap(y))
                        ?  self.Select(_ => project(x, y))
                        : (self.Select(_ => default(D))).Fail())
                 select y)
            select w;

        /// <summary>
        /// Produce a failure value
        /// </summary>
        [Pure]
        public static Monad<A> Fail<A>(this Monad<A> ma, string err = "") =>
            ma.Fail(err);
    }
}
