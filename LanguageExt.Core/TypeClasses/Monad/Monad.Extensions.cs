using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using LanguageExt.TypeClasses;
using LanguageExt.Instances;

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
        public static MB Select<MONAD, FUNCTOR, MA, MB, A, B>(this MA ma, Func<A, B> f)
            where FUNCTOR : struct, Functor<MA, MB, A, B>
            where MONAD   : struct, Monad<MA, A> =>
                default(FUNCTOR).Map(ma, f);

        /// <summary>
        /// Projection from one value to another using f
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <typeparam name="B">Resulting bound value type</typeparam>
        /// <param name="ma">Monad value to map from </param>
        /// <param name="f">Projection function</param>
        /// <returns>Mapped monad</returns>
        [Pure]
        public static MB LiftM<MONAD, FUNCTOR, MA, MB, A, B>(this MA ma, Func<A, B> f)
            where FUNCTOR : struct, Functor<MA, MB, A, B>
            where MONAD   : struct, Monad<MA, A> =>
                default(FUNCTOR).Map(ma, f);

        /// <summary>
        /// Monadic bind and project
        /// </summary>
        [Pure]
        public static MC SelectMany<MONADA, MONADB, MONADC, MA, MB, MC, A, B, C>(
            this MA self,
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
            this MA self,
            Func<A, IEnumerable<B>> bind,
            Func<A, B, C> project)
            where MONADA : struct, Monad<MA, A> =>
            default(MONADA).Bind<MSeq<C>, IEnumerable<C>, C>(self, a =>
                bind(a).Select(b => project(a, b)));

        /// <summary>
        /// Monad join
        /// </summary>
        [Pure]
        public static MD Join<EQ, MONADA, MONADB, MONADD, MA, MB, MD, A, B, C, D>(
            this MA self,
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
        /// Produce a failure value
        /// </summary>
        [Pure]
        public static MA Fail<MONAD, MA, A>(this MA ma, string err = "") where MONAD : struct, Monad<MA, A> =>
            default(MONAD).Fail(err);
    }
}
