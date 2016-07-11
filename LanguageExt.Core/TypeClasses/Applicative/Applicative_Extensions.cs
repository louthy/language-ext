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
        public static Applicative<U> Apply<T, U>(this Applicative<Func<T, U>> x, Applicative<T> y) =>
            from a in x
            from b in y
            select a(b);

        /// <summary>
        /// Sequential application
        /// 
        ///     f(a -> b -> c) -> f a -> f b -> f c
        /// </summary>
        [Pure]
        public static Applicative<V> Apply<T, U, V>(this Applicative<Func<T, U, V>> x, Applicative<T> y, Applicative<U> z) =>
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
        public static Applicative<Func<U, V>> Apply<T, U, V>(this Applicative<Func<T, Func<U, V>>> x, Applicative<T> y) =>
            from a in x
            from b in y
            select a(b);

        /// <summary>
        /// Sequential actions
        /// 
        ///     f a -> f b -> f b
        /// </summary>
        [Pure]
        public static Applicative<U> Action<T, U>(this Applicative<T> x, Applicative<U> y) =>
            from a in x
            from b in y
            select b;

        /// <summary>
        /// Projection from one value to another using f
        /// </summary>
        /// <typeparam name="T">Functor value type</typeparam>
        /// <typeparam name="U">Resulting functor value type</typeparam>
        /// <param name="x">Functor value to map from </param>
        /// <param name="f">Projection function</param>
        /// <returns>Mapped functor</returns>
        [Pure]
        public static Applicative<U> Select<T, U>(
            this Applicative<T> self,
            Func<T, U> map
            ) =>
            (Applicative<U>)self.Map(self, map);
    }
}
