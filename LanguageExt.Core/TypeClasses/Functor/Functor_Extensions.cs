using System;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using LanguageExt;
using LanguageExt.TypeClasses;

namespace LanguageExt
{
    public static partial class TypeClassExtensions
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
        public static Applicative<U> Select<T, U>(
            this Applicative<T> self,
            Func<T, U> map
            ) =>
            (Applicative<U>)self.Map(self, map);

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
        public static Monad<U> Select<T, U>(
            this Monad<T> self,
            Func<T, U> map
            ) =>
            (Monad<U>)self.Map(self, map);

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
        public static Applicative<U> Map<T, U>(
            this Applicative<T> self,
            Func<T, U> map
            ) =>
            (Applicative<U>)self.Map(self, map);

        /// <summary>
        /// Projection from one value to another using f
        /// </summary>
        /// <typeparam name="T">Functor value type</typeparam>
        /// <typeparam name="U">Resulting functor value type</typeparam>
        /// <param name="x">Functor value to map from </param>
        /// <param name="f">Projection function</param>
        /// <returns>Mapped functor</returns>
        [Pure]
        public static Monad<U> Map<T, U>(
            this Monad<T> self,
            Func<T, U> map
            ) =>
            (Monad<U>)self.Map(self, map);
    }
}
