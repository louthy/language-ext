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
        /// <typeparam name="A">Functor value type</typeparam>
        /// <typeparam name="B">Resulting functor value type</typeparam>
        /// <param name="x">Functor value to map from </param>
        /// <param name="f">Projection function</param>
        /// <returns>Mapped functor</returns>
        [Pure]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static Functor<B> Select<A, B>(
            this Functor<A> self,
            Func<A, B> map
            ) =>
            self.Map(self, map);

        /// <summary>
        /// Projection from one value to another using f
        /// </summary>
        /// <typeparam name="A">Functor value type</typeparam>
        /// <typeparam name="B">Resulting functor value type</typeparam>
        /// <param name="x">Functor value to map from </param>
        /// <param name="f">Projection function</param>
        /// <returns>Mapped functor</returns>
        [Pure]
        public static Functor<B> Map<A, B>(
            this Functor<A> self,
            Func<A, B> map
            ) =>
            self.Map(self, map);

        /// <summary>
        /// Projection from one value to another using f
        /// </summary>
        /// <typeparam name="A">Functor value type</typeparam>
        /// <typeparam name="B">Resulting functor value type</typeparam>
        /// <param name="x">Functor value to map from </param>
        /// <param name="f">Projection function</param>
        /// <returns>Mapped functor</returns>
        [Pure]
        public static FU Map<FU, A, B>(
            this Functor<A> self,
            Func<A, B> map)
            where FU : Functor<B>
            =>
            (FU)self.Map(self, map);
    }
}
