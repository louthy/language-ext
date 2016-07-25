using System;
using System.Diagnostics.Contracts;
using LanguageExt.TypeClasses;

namespace LanguageExt
{
    public static partial class TypeClassExtensions
    {
        /// <summary>
        /// Sequential application
        /// 
        ///     f(a -> b) -> f a -> f b
        /// </summary>
        [Pure]
        public static Applicative<B> Apply<A, B>(this Applicative<Func<A, B>> x, Applicative<A> y) =>
            from a in x
            from b in y
            select a(b);

        /// <summary>
        /// Sequential application
        /// 
        ///     f(a -> b -> c) -> f a -> f b -> f c
        /// </summary>
        [Pure]
        public static Applicative<C> Apply<A, B, C>(this Applicative<Func<A, B, C>> x, Applicative<A> y, Applicative<B> z) =>
            from a in x
            from b in y
            from c in z
            select a(b, c);

        /// <summary>
        /// Sequential application
        /// 
        ///     f(a -> b -> c) -> f a -> f(b -> c)
        /// </summary>
        [Pure]
        public static Applicative<Func<B, C>> Apply<A, B, C>(this Applicative<Func<A, Func<B, C>>> x, Applicative<A> y) =>
            from a in x
            from b in y
            select a(b);

        /// <summary>
        /// Sequential actions
        /// 
        ///     f a -> f b -> f b
        /// </summary>
        [Pure]
        public static Applicative<B> Action<A, B>(this Applicative<A> x, Applicative<B> y) =>
            from a in x
            from b in y
            select b;

        /// <summary>
        /// Projection from one value to another using f
        /// </summary>
        /// <typeparam name="A">Functor value type</typeparam>
        /// <typeparam name="B">Resulting functor value type</typeparam>
        /// <param name="x">Functor value to map from </param>
        /// <param name="f">Projection function</param>
        /// <returns>Mapped functor</returns>
        [Pure]
        public static Applicative<B> Select<A, B>(
            this Applicative<A> self,
            Func<A, B> map
            ) =>
            (Applicative<B>)self.Map(self, map);


        /// <summary>
        /// Monadic bind
        /// </summary>
        /// <typeparam name="A">Type of the bound value</typeparam>
        /// <typeparam name="U">Type of the bound return value</typeparam>
        /// <param name="self">Monad to bind</param>
        /// <param name="bind">Bind function</param>
        /// <returns>Monad of U</returns>
        [Pure]
        public static Applicative<U> Bind<A, U>(this Applicative<A> self, Func<A, Applicative<U>> bind) =>
            self.Bind(self, bind);

        /// <summary>
        /// Applicative bind and project
        /// </summary>
        public static Applicative<C> SelectMany<A, B, C>(
            this Applicative<A> self,
            Func<A, Applicative<B>> bind,
            Func<A, B, C> project)
            =>
            self.Bind(self,
                t => bind(t).Select(
                    u => project(t, u)));
    }
}
