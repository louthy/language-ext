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
            y.Apply(x, y);

        /// <summary>
        /// Sequential application
        /// 
        ///     f(a -> b -> c) -> f a -> f b -> f c
        /// </summary>
        [Pure]
        public static Applicative<V> Apply<T, U, V>(this Applicative<Func<T, U, V>> x, Applicative<T> y, Applicative<U> z) =>
            y.Apply(x, y, z);

        /// <summary>
        /// Sequential application
        /// 
        ///     f(a -> b -> c) -> f a -> f(b -> c)
        /// </summary>
        [Pure]
        public static Applicative<Func<U, V>> Apply<T, U, V>(this Applicative<Func<T, Func<U, V>>> x, Applicative<T> y) =>
            y.Apply(x, y);

        /// <summary>
        /// Sequential actions
        /// 
        ///     f a -> f b -> f b
        /// </summary>
        [Pure]
        public static Applicative<U> Action<T, U>(this Applicative<T> x, Applicative<U> y) =>
            x.Action(x, y);
    }
}
