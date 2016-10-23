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
        public static MA Return<MONAD, MA, A>(A x, params A[] xs) where MONAD : struct, Monad<MA, A> =>
            default(MONAD).Return(x);

        /// <summary>
        /// Monad return
        /// </summary>
        /// <typeparam name="A">Type of the bound monad value</typeparam>
        /// <param name="a">The bound monad value</param>
        /// <returns>Monad of A</returns>
        public static MA Return<MONAD, MA, A>(IEnumerable<A> xs) where MONAD : struct, Monad<MA, A> =>
            default(MONAD).Return(xs);

        /// <summary>
        /// Monadic bind
        /// </summary>
        /// <typeparam name="B">Type of the bound return value</typeparam>
        /// <param name="ma">Monad to bind</param>
        /// <param name="f">Bind function</param>
        /// <returns>Monad of B</returns>
        public static MB bind<MONADA, MONADB, MA, MB, A, B>(MA ma, Func<A, MB> f)
            where MONADA : struct, Monad<MA, A>
            where MONADB : struct, Monad<MB, B> =>
            default(MONADA).Bind<MONADB, MB, B>(ma, f);

        /// <summary>
        /// Produce a failure value
        /// </summary>
        public static MA fail<MONAD, MA, A>(Exception err = null) where MONAD : struct, Monad<MA, A> =>
            default(MONAD).Fail(err);

        /// <summary>
        /// Performs a map operation on the monad
        /// </summary>
        /// <typeparam name="B">The mapped type</typeparam>
        /// <param name="ma">Monad to map</param>
        /// <param name="f">Mapping operation</param>
        /// <returns>Mapped monad</returns>
        public static MB liftM<MONAD, FUNCTOR, MA, MB, A, B>(MA ma, Func<A, B> f)
            where FUNCTOR : struct, Functor<MA, MB, A, B>
            where MONAD   : struct, Monad<MA, A> =>
            default(FUNCTOR).Map(ma, f);

        /// <summary>
        /// Monad join
        /// </summary>
        public static MD join<EQ, MONADA, MONADB, MONADD, MA, MB, MD, A, B, C, D>(
            MA self,
            MB inner,
            Func<A, C> outerKeyMap,
            Func<B, C> innerKeyMap,
            Func<A, B, D> project)
            where EQ : struct, Eq<C>
            where MONADA : struct, Monad<MA, A>
            where MONADB : struct, Monad<MB, B>
            where MONADD : struct, Monad<MD, D> =>
                self.Join<EQ, MONADA, MONADB, MONADD, MA, MB, MD, A, B, C, D>(inner, outerKeyMap, innerKeyMap, project);
    }
}
