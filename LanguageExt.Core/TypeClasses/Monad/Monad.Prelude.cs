using System;
using System.Linq;
using System.Collections.Generic;
using LanguageExt.TypeClasses;
using static LanguageExt.TypeClass;
using System.Diagnostics.Contracts;
using LanguageExt.ClassInstances;

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
        [Pure]
        public static MA Return<MONAD, MA, A>(A x) where MONAD : struct, Monad<MA, A> =>
            default(MONAD).Return(x);

        /// <summary>
        /// Monad return
        /// </summary>
        /// <typeparam name="A">Type of the bound monad value</typeparam>
        /// <param name="a">The bound monad value</param>
        /// <returns>Monad of A</returns>
        [Pure]
        public static MA FromSeq<MONAD, MA, A>(IEnumerable<A> xs) where MONAD : struct, Monad<MA, A> =>
            default(MONAD).FromSeq(xs);

        /// <summary>
        /// Monadic bind
        /// </summary>
        /// <typeparam name="B">Type of the bound return value</typeparam>
        /// <param name="ma">Monad to bind</param>
        /// <param name="f">Bind function</param>
        /// <returns>Monad of B</returns>
        [Pure]
        public static MB bind<MONADA, MONADB, MA, MB, A, B>(MA ma, Func<A, MB> f)
            where MONADA : struct, Monad<MA, A>
            where MONADB : struct, Monad<MB, B> =>
            default(MONADA).Bind<MONADB, MB, B>(ma, f);

        /// <summary>
        /// Produce a failure value
        /// </summary>
        [Pure]
        public static MA fail<MONAD, MA, A>(Exception err = null) where MONAD : struct, Monad<MA, A> =>
            default(MONAD).Fail(err);

        /// <summary>
        /// Performs a map operation on the monad
        /// </summary>
        /// <typeparam name="B">The mapped type</typeparam>
        /// <param name="ma">Monad to map</param>
        /// <param name="f">Mapping operation</param>
        /// <returns>Mapped monad</returns>
        [Pure]
        public static MB liftM<MONAD, FUNCTOR, MA, MB, A, B>(MA ma, Func<A, B> f)
            where FUNCTOR : struct, Functor<MA, MB, A, B>
            where MONAD   : struct, Monad<MA, A> =>
            default(FUNCTOR).Map(ma, f);

        /// <summary>
        /// Monad join
        /// </summary>
        [Pure]
        public static MD join<EQ, MONADA, MONADB, MONADD, MA, MB, MD, A, B, C, D>(
            MA self,
            MB inner,
            Func<A, C> outerKeyMap,
            Func<B, C> innerKeyMap,
            Func<A, B, D> project)
            where EQ     : struct, Eq<C>
            where MONADA : struct, Monad<MA, A>
            where MONADB : struct, Monad<MB, B>
            where MONADD : struct, Monad<MD, D> =>
                default(MONADA).Bind<MONADD, MD, D>(self,  x =>
                default(MONADB).Bind<MONADD, MD, D>(inner, y =>
                    default(EQ).Equals(outerKeyMap(x), innerKeyMap(y))
                        ? default(MONADD).Return(project(x,y))
                        : default(MONADD).Fail()));

        /// <summary>
        /// Monadic bind and project
        /// </summary>
        [Pure]
        public static MC SelectMany<MONADA, MONADB, MONADC, MA, MB, MC, A, B, C>(
            MA self,
            Func<A, MB> bind,
            Func<A, B, C> project)
            where MONADA : struct, Monad<MA, A>
            where MONADB : struct, Monad<MB, B>
            where MONADC : struct, Monad<MC, C> =>
                default(MONADA).Bind<MONADC, MC, C>( self,    t => 
                default(MONADB).Bind<MONADC, MC, C>( bind(t), u => 
                default(MONADC).Return(project(t, u))));

        [Pure]
        public static IEnumerable<C> SelectMany<MONADA, MA, A, B, C>(
            MA self,
            Func<A, IEnumerable<B>> bind,
            Func<A, B, C> project)
            where MONADA : struct, Monad<MA, A> =>
            default(MONADA).Bind<MSeq<C>, IEnumerable<C>, C>(self, a =>
                bind(a).Select(b => project(a, b)));

    }
}
