using System;
using System.Linq;
using System.Collections.Generic;
using LanguageExt.TypeClasses;
using static LanguageExt.TypeClass;
using static LanguageExt.Prelude;
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
            default(MONAD).Return(_ => x);

        [Pure]
        public static MB traverse<MonadA, MonadB, MA, MB, A, B>(MA ma, Func<A, B> f)
            where MonadA : struct, Monad<Unit, Unit, MA, A>
            where MonadB : struct, Monad<Unit, Unit, MB, B> =>
            traverse<Unit, Unit, MonadA, MonadB, MA, MB, A, B>(ma, f)(unit);

        [Pure]
        public static MB traverse<MonadA, MonadB, MA, MB, A, B>(MA ma, Func<A, MB> f)
            where MonadA : struct, Monad<Unit, Unit, MA, A>
            where MonadB : struct, Monad<Unit, Unit, MB, B> =>
            traverse<Unit, Unit, MonadA, MonadB, MA, MB, A, B>(ma, f)(unit);

        [Pure]
        public static Func<Env, MB> traverse<Env, Out, MonadA, MonadB, MA, MB, A, B>(MA ma, Func<A, B> f)
            where MonadA : struct, Monad<Env, Out, MA, A>
            where MonadB : struct, Monad<Env, Out, MB, B> =>
            default(MonadA).Fold(ma, default(MonadB).Zero(), (s, a) => default(MonadB).Plus(s, default(MonadB).Return(_ => f(a))));

        [Pure]
        public static Func<Env, MB> traverse<Env, Out, MonadA, MonadB, MA, MB, A, B>(MA ma, Func<A, MB> f)
            where MonadA : struct, Monad<Env, Out, MA, A>
            where MonadB : struct, Monad<Env, Out, MB, B> =>
            default(MonadA).Fold(ma, default(MonadB).Zero(), (s, a) => default(MonadB).Plus(s, f(a)));

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
            where FUNCTOR : struct, Functor<MA, MB, A, B> =>
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
                        ? default(MONADD).Return(_ => project(x, y))
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
            default(MONADA).Bind<MEnumerable<C>, IEnumerable<C>, C>(self, a =>
                bind(a).Select(b => project(a, b)));

        [Pure]
        public static Seq<C> SelectMany<MONADA, MA, A, B, C>(
            MA self,
            Func<A, Seq<B>> bind,
            Func<A, B, C> project)
            where MONADA : struct, Monad<MA, A> =>
            default(MONADA).Bind<MSeq<C>, Seq<C>, C>(self, a =>
                bind(a).Select(b => project(a, b)));

        /// <summary>
        /// Return monad 'zero'.  None for Option, [] for Lst, ...
        /// </summary>
        /// <typeparam name="MA">Monad type</typeparam>
        /// <typeparam name="A">Bound type</typeparam>
        /// <returns>Zero for the structure</returns>
        [Pure]
        public static MA mzero<MPLUS, MA, A>() where MPLUS : struct, Monad<MA, A> =>
            default(MPLUS).Zero();

        /// <summary>
        /// Return monad x 'plus' monad y
        /// 
        /// Note, this doesn't add the bound values, it works on the monad state
        /// itself.  
        /// 
        /// For example with Option:
        /// 
        ///     None   'plus' None   = None
        ///     Some a 'plus' None   = Some a
        ///     None   'plus' Some b = Some b
        ///     Some a 'plus' Some b = Some a
        /// 
        /// </summary>
        /// <typeparam name="MA">Monad type</typeparam>
        /// <typeparam name="A">Bound type</typeparam>
        /// <param name="x">Left hand side of the operation</param>
        /// <param name="y">Right hand side of the operation</param>
        /// <returns>x 'plus' y </returns>
        [Pure]
        public static MA mplus<MPLUS, MA, A>(MA x, MA y) where MPLUS : struct, Monad<MA, A> =>
            default(MPLUS).Plus(x, y);

        /// <summary>
        /// Performs the following fold operation: fold(xs, mzero, mplus)
        /// </summary>
        /// <typeparam name="MA">Monad type</typeparam>
        /// <typeparam name="A">Bound type</typeparam>
        /// <param name="xs">The monads to sum</param>
        /// <returns>The summed monads</returns>
        [Pure]
        public static MA msum<MPLUS, MA, A>(params MA[] xs) where MPLUS : struct, Monad<MA, A> =>
            xs.Fold(mzero<MPLUS, MA, A>(), (s, x) => mplus<MPLUS, MA, A>(s, x));

        /// <summary>
        /// Performs the following fold operation: fold(xs, mzero, mplus)
        /// </summary>
        /// <typeparam name="MA">Monad type</typeparam>
        /// <typeparam name="A">Bound type</typeparam>
        /// <param name="xs">The monads to sum</param>
        /// <returns>The summed monads</returns>
        [Pure]
        public static MA msum<MPLUS, MA, A>(IEnumerable<MA> xs) where MPLUS : struct, Monad<MA, A> =>
            xs.Fold(mzero<MPLUS, MA, A>(), (s, x) => mplus<MPLUS, MA, A>(s, x));

        /// <summary>
        /// Filters the monad if the predicate doesn't hold
        /// </summary>
        /// <typeparam name="MA">Monad type</typeparam>
        /// <typeparam name="A">Bound type</typeparam>
        /// <param name="ma">The monads filter</param>
        /// <returns>The filtered (or not) monad</returns>
        [Pure]
        public static MA filter<MPLUS, MA, A>(MA ma, Func<A, bool> pred) where MPLUS : struct, Monad<MA, A> =>
            default(MPLUS).Bind<MPLUS, MA, A>(ma,
                x => pred(x)
                    ? ma
                    : mzero<MPLUS, MA, A>());
    }
}
