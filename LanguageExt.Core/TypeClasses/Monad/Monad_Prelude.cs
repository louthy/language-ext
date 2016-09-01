using System;
using System.Collections.Generic;
using LanguageExt.TypeClasses;

namespace LanguageExt
{
    public static partial class TypeClass
    {
        /// <summary>
        /// Monad return
        /// </summary>
        /// <typeparam name="A">Type of the bound monad value</typeparam>
        /// <param name="x">The bound monad value</param>
        /// <returns>Monad of A</returns>
        public static M Return<M, A>(A x, params A[] xs) where M : struct, Monad<A> =>
            (M)default(M).Return(x);

        /// <summary>
        /// Monad return
        /// </summary>
        /// <typeparam name="A">Type of the bound monad value</typeparam>
        /// <param name="a">The bound monad value</param>
        /// <returns>Monad of A</returns>
        public static M Return<M, A>(IEnumerable<A> xs) where M : struct, Monad<A> =>
            (M)default(M).Return(xs);

        /// <summary>
        /// Monadic bind
        /// </summary>
        /// <typeparam name="B">Type of the bound return value</typeparam>
        /// <param name="ma">Monad to bind</param>
        /// <param name="f">Bind function</param>
        /// <returns>Monad of B</returns>
        public static Monad<B> bind<A, B>(Monad<A> ma, Func<A, Monad<B>> f) =>
            ma.Bind(ma, f);

        /// <summary>
        /// Monadic bind
        /// </summary>
        /// <typeparam name="B">Type of the bound return value</typeparam>
        /// <param name="ma">Monad to bind</param>
        /// <param name="f">Bind function</param>
        /// <returns>Monad of B</returns>
        public static MB bind<MB, A, B>(Monad<A> ma, Func<A, MB> f) where MB : struct, Monad<B> =>
            ma.Bind<MB, B>(ma, f);

        /// <summary>
        /// Produce a failure value
        /// </summary>
        public static M fail<M, A>(string err = "") where M : struct, Monad<A> =>
            (M)default(M).Fail(err);

        /// <summary>
        /// Performs a map operation on the monad
        /// </summary>
        /// <typeparam name="B">The mapped type</typeparam>
        /// <param name="ma">Monad to map</param>
        /// <param name="f">Mapping operation</param>
        /// <returns>Mapped monad</returns>
        public static Monad<B> liftM<A, B>(Monad<A> ma, Func<A, B> f) =>
            ma.Map<Monad<B>, A, B>(f);
    }
}
