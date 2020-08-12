using System;
using System.Collections.Generic;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;

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
        public static MA ReturnAsync<MONAD, MA, A>(Task<A> x) where MONAD : struct, MonadAsync<MA, A> =>
            default(MONAD).ReturnAsync(x);

        [Pure]
        public static Task<MB> traverseAsync<MonadA, MonadB, MA, MB, A, B>(MA ma, Func<A, B> f)
            where MonadA : struct, MonadAsync<Unit, Unit, MA, A>
            where MonadB : struct, MonadAsync<Unit, Unit, MB, B> =>
            traverseAsync<Unit, Unit, MonadA, MonadB, MA, MB, A, B>(ma, f)(unit);

        [Pure]
        public static Task<MB> traverseAsync<MonadA, MonadB, MA, MB, A, B>(MA ma, Func<A, Task<B>> f)
            where MonadA : struct, MonadAsync<Unit, Unit, MA, A>
            where MonadB : struct, MonadAsync<Unit, Unit, MB, B> =>
            traverseAsync<Unit, Unit, MonadA, MonadB, MA, MB, A, B>(ma, f)(unit);

        [Pure]
        public static Task<MB> traverseAsync<MonadA, MonadB, MA, MB, A, B>(MA ma, Func<A, MB> f)
            where MonadA : struct, MonadAsync<Unit, Unit, MA, A>
            where MonadB : struct, MonadAsync<Unit, Unit, MB, B> =>
            traverseAsync<Unit, Unit, MonadA, MonadB, MA, MB, A, B>(ma, f)(unit);

        [Pure]
        public static Task<MB> traverseAsync<MonadA, MonadB, MA, MB, A, B>(MA ma, Func<A, Task<MB>> f)
            where MonadA : struct, MonadAsync<Unit, Unit, MA, A>
            where MonadB : struct, MonadAsync<Unit, Unit, MB, B> =>
            traverseAsync<Unit, Unit, MonadA, MonadB, MA, MB, A, B>(ma, f)(unit);

        [Pure]
        public static Func<Env, Task<MB>> traverseAsync<Env, Out, MonadA, MonadB, MA, MB, A, B>(MA ma, Func<A, B> f)
            where MonadA : struct, MonadAsync<Env, Out, MA, A>
            where MonadB : struct, MonadAsync<Env, Out, MB, B> =>
            default(MonadA).Fold(ma, default(MonadB).Zero(), (s, a) => default(MonadB).Plus(s, default(MonadB).ReturnAsync(_ => f(a).AsTask())));

        [Pure]
        public static Func<Env, Task<MB>> traverseAsync<Env, Out, MonadA, MonadB, MA, MB, A, B>(MA ma, Func<A, Task<B>> f)
            where MonadA : struct, MonadAsync<Env, Out, MA, A>
            where MonadB : struct, MonadAsync<Env, Out, MB, B> =>
            default(MonadA).Fold(ma, default(MonadB).Zero(), (s, a) => default(MonadB).Plus(s, default(MonadB).ReturnAsync(async _ => await f(a).ConfigureAwait(false))));

        [Pure]
        public static Func<Env, Task<MB>> traverseAsync<Env, Out, MonadA, MonadB, MA, MB, A, B>(MA ma, Func<A, MB> f)
            where MonadA : struct, MonadAsync<Env, Out, MA, A>
            where MonadB : struct, MonadAsync<Env, Out, MB, B> =>
            default(MonadA).Fold(ma, default(MonadB).Zero(), (s, a) => default(MonadB).Plus(s, f(a)));

        [Pure]
        public static Func<Env, Task<MB>> traverseAsync<Env, Out, MonadA, MonadB, MA, MB, A, B>(MA ma, Func<A, Task<MB>> f)
            where MonadA : struct, MonadAsync<Env, Out, MA, A>
            where MonadB : struct, MonadAsync<Env, Out, MB, B> =>
            default(MonadA).FoldAsync(ma, default(MonadB).Zero(), async (s, a) => default(MonadB).Plus(s, await f(a).ConfigureAwait(false)));

        [Pure]
        public static MB traverseSyncAsync<MonadA, MonadB, MA, MB, A, B>(MA ma, Func<A, MB> f)
            where MonadA : struct, Monad<Unit, Unit, MA, A>
            where MonadB : struct, MonadAsync<Unit, Unit, MB, B> =>
            default(MonadA).Fold(ma, default(MonadB).Zero(), (s, a) => default(MonadB).Plus(s, f(a)))(unit);

        [Pure]
        public static Func<Env, MB> traverseSyncAsync<Env, Out, MonadA, MonadB, MA, MB, A, B>(MA ma, Func<A, MB> f)
            where MonadA : struct, Monad<Env, Out, MA, A>
            where MonadB : struct, MonadAsync<Env, Out, MB, B> =>
            default(MonadA).Fold(ma, default(MonadB).Zero(), (s, a) => default(MonadB).Plus(s, f(a)));

        [Pure]
        public static Task<MB> traverseAsyncSync<MonadA, MonadB, MA, MB, A, B>(MA ma, Func<A, MB> f)
            where MonadA : struct, MonadAsync<Unit, Unit, MA, A>
            where MonadB : struct, Monad<Unit, Unit, MB, B> =>
            default(MonadA).Fold(ma, default(MonadB).Zero(), (s, a) => default(MonadB).Plus(s, f(a)))(unit);

        [Pure]
        public static Func<Env, Task<MB>> traverseAsyncSync<Env, Out, MonadA, MonadB, MA, MB, A, B>(MA ma, Func<A, MB> f)
            where MonadA : struct, MonadAsync<Env, Out, MA, A>
            where MonadB : struct, Monad<Env, Out, MB, B> =>
            default(MonadA).Fold(ma, default(MonadB).Zero(), (s, a) => default(MonadB).Plus(s, f(a)));

        [Pure]
        public static Task<MB> traverseAsyncSync<MonadA, MonadB, MA, MB, A, B>(MA ma, Func<A, Task<MB>> f)
            where MonadA : struct, MonadAsync<Unit, Unit, MA, A>
            where MonadB : struct, Monad<Unit, Unit, MB, B> =>
            default(MonadA).FoldAsync(ma, default(MonadB).Zero(), async (s, a) => default(MonadB).Plus(s, await f(a).ConfigureAwait(false)))(unit);

        [Pure]
        public static Func<Env, Task<MB>> traverseAsyncSync<Env, Out, MonadA, MonadB, MA, MB, A, B>(MA ma, Func<A, Task<MB>> f)
            where MonadA : struct, MonadAsync<Env, Out, MA, A>
            where MonadB : struct, Monad<Env, Out, MB, B> =>
            default(MonadA).FoldAsync(ma, default(MonadB).Zero(), async (s, a) => default(MonadB).Plus(s, await f(a).ConfigureAwait(false)));

        /// <summary>
        /// Monadic bind
        /// </summary>
        /// <typeparam name="B">Type of the bound return value</typeparam>
        /// <param name="ma">Monad to bind</param>
        /// <param name="f">Bind function</param>
        /// <returns>Monad of B</returns>
        [Pure]
        public static MB bindAsync<MONADA, MONADB, MA, MB, A, B>(MA ma, Func<A, MB> f)
            where MONADA : struct, MonadAsync<MA, A>
            where MONADB : struct, MonadAsync<MB, B> =>
            default(MONADA).Bind<MONADB, MB, B>(ma, f);

        /// <summary>
        /// Monadic bind
        /// </summary>
        /// <typeparam name="B">Type of the bound return value</typeparam>
        /// <param name="ma">Monad to bind</param>
        /// <param name="f">Bind function</param>
        /// <returns>Monad of B</returns>
        [Pure]
        public static MB bindAsync<MONADA, MONADB, MA, MB, A, B>(MA ma, Func<A, Task<MB>> f)
            where MONADA : struct, MonadAsync<MA, A>
            where MONADB : struct, MonadAsync<MB, B> =>
            default(MONADA).BindAsync<MONADB, MB, B>(ma, f);

        /// <summary>
        /// Produce a failure value
        /// </summary>
        [Pure]
        public static MA failAsync<MONAD, MA, A>(Exception err = null) where MONAD : struct, MonadAsync<MA, A> =>
            default(MONAD).Fail(err);

        /// <summary>
        /// Performs a map operation on the monad
        /// </summary>
        /// <typeparam name="B">The mapped type</typeparam>
        /// <param name="ma">Monad to map</param>
        /// <param name="f">Mapping operation</param>
        /// <returns>Mapped MonadAsync</returns>
        [Pure]
        public static MB liftMAsync<MONAD, FUNCTOR, MA, MB, A, B>(MA ma, Func<A, B> f)
            where FUNCTOR : struct, FunctorAsync<MA, MB, A, B>
            where MONAD   : struct, MonadAsync<MA, A> =>
            default(FUNCTOR).Map(ma, f);

        /// <summary>
        /// Performs a map operation on the monad
        /// </summary>
        /// <typeparam name="B">The mapped type</typeparam>
        /// <param name="ma">Monad to map</param>
        /// <param name="f">Mapping operation</param>
        /// <returns>Mapped MonadAsync</returns>
        [Pure]
        public static MB liftMAsync<MONAD, FUNCTOR, MA, MB, A, B>(MA ma, Func<A, Task<B>> f)
            where FUNCTOR : struct, FunctorAsync<MA, MB, A, B>
            where MONAD : struct, MonadAsync<MA, A> =>
            default(FUNCTOR).MapAsync(ma, f);

        /// <summary>
        /// Monad join
        /// </summary>
        [Pure]
        public static MD joinAsync<EQ, MONADA, MONADB, MONADD, MA, MB, MD, A, B, C, D>(
            MA self,
            MB inner,
            Func<A, C> outerKeyMap,
            Func<B, C> innerKeyMap,
            Func<A, B, D> project)
            where EQ     : struct, Eq<C>
            where MONADA : struct, MonadAsync<MA, A>
            where MONADB : struct, MonadAsync<MB, B>
            where MONADD : struct, MonadAsync<MD, D> =>
                default(MONADA).Bind<MONADD, MD, D>(self,  x =>
                default(MONADB).Bind<MONADD, MD, D>(inner, y =>
                    default(EQ).Equals(outerKeyMap(x), innerKeyMap(y))
                        ? default(MONADD).ReturnAsync(_ => project(x, y).AsTask())
                        : default(MONADD).Fail()));

        /// <summary>
        /// Monad join
        /// </summary>
        [Pure]
        public static MD joinAsync<EQ, MONADA, MONADB, MONADD, MA, MB, MD, A, B, C, D>(
            MA self,
            MB inner,
            Func<A, C> outerKeyMap,
            Func<B, C> innerKeyMap,
            Func<A, B, Task<D>> project)
            where EQ : struct, Eq<C>
            where MONADA : struct, MonadAsync<MA, A>
            where MONADB : struct, MonadAsync<MB, B>
            where MONADD : struct, MonadAsync<MD, D> =>
                default(MONADA).Bind<MONADD, MD, D>(self, x =>
                default(MONADB).Bind<MONADD, MD, D>(inner, y =>
                   default(EQ).Equals(outerKeyMap(x), innerKeyMap(y))
                       ? default(MONADD).ReturnAsync(async _ => await project(x, y).ConfigureAwait(false))
                       : default(MONADD).Fail()));

        /// <summary>
        /// Monad join
        /// </summary>
        [Pure]
        public static MD joinAsync<EQ, MONADA, MONADB, MONADD, MA, MB, MD, A, B, C, D>(
            MA self,
            MB inner,
            Func<A, Task<C>> outerKeyMap,
            Func<B, Task<C>> innerKeyMap,
            Func<A, B, Task<D>> project)
            where EQ : struct, Eq<C>
            where MONADA : struct, MonadAsync<MA, A>
            where MONADB : struct, MonadAsync<MB, B>
            where MONADD : struct, MonadAsync<MD, D> =>
                default(MONADA).Bind<MONADD, MD, D>(self, x =>
                default(MONADB).BindAsync<MONADD, MD, D>(inner, async y =>
                   default(EQ).Equals(await outerKeyMap(x), await innerKeyMap(y).ConfigureAwait(false))
                       ? default(MONADD).ReturnAsync(async _ => await project(x, y).ConfigureAwait(false))
                       : default(MONADD).Fail()));

        /// <summary>
        /// Monad join
        /// </summary>
        [Pure]
        public static MD joinAsync<EQ, MONADA, MONADB, MONADD, MA, MB, MD, A, B, C, D>(
            MA self,
            MB inner,
            Func<A, Task<C>> outerKeyMap,
            Func<B, Task<C>> innerKeyMap,
            Func<A, B, D> project)
            where EQ : struct, Eq<C>
            where MONADA : struct, MonadAsync<MA, A>
            where MONADB : struct, MonadAsync<MB, B>
            where MONADD : struct, MonadAsync<MD, D> =>
                default(MONADA).Bind<MONADD, MD, D>(self, x =>
                default(MONADB).BindAsync<MONADD, MD, D>(inner, async y =>
                   default(EQ).Equals(await outerKeyMap(x), await innerKeyMap(y).ConfigureAwait(false))
                       ? default(MONADD).ReturnAsync(project(x, y).AsTask())
                       : default(MONADD).Fail()));

        /// <summary>
        /// Monadic bind and project
        /// </summary>
        [Pure]
        public static MC SelectManyAsync<MONADA, MONADB, MONADC, MA, MB, MC, A, B, C>(
            MA self,
            Func<A, MB> bind,
            Func<A, B, C> project)
            where MONADA : struct, MonadAsync<MA, A>
            where MONADB : struct, MonadAsync<MB, B>
            where MONADC : struct, MonadAsync<MC, C> =>
                default(MONADA).Bind<MONADC, MC, C>( self,    t => 
                default(MONADB).Bind<MONADC, MC, C>( bind(t), u => 
                default(MONADC).ReturnAsync(project(t, u).AsTask())));

        /// <summary>
        /// Monadic bind and project
        /// </summary>
        [Pure]
        public static MC SelectManyAsync<MONADA, MONADB, MONADC, MA, MB, MC, A, B, C>(
            MA self,
            Func<A, Task<MB>> bind,
            Func<A, B, C> project)
            where MONADA : struct, MonadAsync<MA, A>
            where MONADB : struct, MonadAsync<MB, B>
            where MONADC : struct, MonadAsync<MC, C> =>
                default(MONADA).BindAsync<MONADC, MC, C>(self, async t =>
                default(MONADB).Bind<MONADC, MC, C>(await bind(t).ConfigureAwait(false), u =>
                default(MONADC).ReturnAsync(project(t, u).AsTask())));

        /// <summary>
        /// Monadic bind and project
        /// </summary>
        [Pure]
        public static MC SelectManyAsync<MONADA, MONADB, MONADC, MA, MB, MC, A, B, C>(
            MA self,
            Func<A, Task<MB>> bind,
            Func<A, B, Task<C>> project)
            where MONADA : struct, MonadAsync<MA, A>
            where MONADB : struct, MonadAsync<MB, B>
            where MONADC : struct, MonadAsync<MC, C> =>
                default(MONADA).BindAsync<MONADC, MC, C>(self, async t =>
                default(MONADB).Bind<MONADC, MC, C>(await bind(t).ConfigureAwait(false), u =>
                default(MONADC).ReturnAsync(project(t, u))));

        /// <summary>
        /// Monadic bind and project
        /// </summary>
        [Pure]
        public static MC SelectManyAsync<MONADA, MONADB, MONADC, MA, MB, MC, A, B, C>(
            MA self,
            Func<A, MB> bind,
            Func<A, B, Task<C>> project)
            where MONADA : struct, MonadAsync<MA, A>
            where MONADB : struct, MonadAsync<MB, B>
            where MONADC : struct, MonadAsync<MC, C> =>
                default(MONADA).Bind<MONADC, MC, C>(self, t =>
                default(MONADB).Bind<MONADC, MC, C>(bind(t), u =>
                default(MONADC).ReturnAsync(project(t, u))));

        /// <summary>
        /// Return monad 'zero'.  None for Option, [] for Lst, ...
        /// </summary>
        /// <typeparam name="MA">Monad type</typeparam>
        /// <typeparam name="A">Bound type</typeparam>
        /// <returns>Zero for the structure</returns>
        [Pure]
        public static MA mzeroAsync<MPLUS, MA, A>() where MPLUS : struct, MonadAsync<MA, A> =>
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
        public static MA mplusAsync<MPLUS, MA, A>(MA x, MA y) where MPLUS : struct, MonadAsync<MA, A> =>
            default(MPLUS).Plus(x, y);

        /// <summary>
        /// Performs the following fold operation: fold(xs, mzero, mplus)
        /// </summary>
        /// <typeparam name="MA">Monad type</typeparam>
        /// <typeparam name="A">Bound type</typeparam>
        /// <param name="xs">The monads to sum</param>
        /// <returns>The summed monads</returns>
        [Pure]
        public static MA msumAsync<MPLUS, MA, A>(params MA[] xs) where MPLUS : struct, MonadAsync<MA, A> =>
            xs.Fold(mzeroAsync<MPLUS, MA, A>(), (s, x) => mplusAsync<MPLUS, MA, A>(s, x));

        /// <summary>
        /// Performs the following fold operation: fold(xs, mzero, mplus)
        /// </summary>
        /// <typeparam name="MA">Monad type</typeparam>
        /// <typeparam name="A">Bound type</typeparam>
        /// <param name="xs">The monads to sum</param>
        /// <returns>The summed monads</returns>
        [Pure]
        public static MA msumAsync<MPLUS, MA, A>(IEnumerable<MA> xs) where MPLUS : struct, MonadAsync<MA, A> =>
            xs.Fold(mzeroAsync<MPLUS, MA, A>(), (s, x) => mplusAsync<MPLUS, MA, A>(s, x));

        /// <summary>
        /// Filters the monad if the predicate doesn't hold
        /// </summary>
        /// <typeparam name="MA">Monad type</typeparam>
        /// <typeparam name="A">Bound type</typeparam>
        /// <param name="ma">The monads filter</param>
        /// <returns>The filtered (or not) MonadAsync</returns>
        [Pure]
        public static MA filterAsync<MPLUS, MA, A>(MA ma, Func<A, bool> pred) where MPLUS : struct, MonadAsync<MA, A> =>
            default(MPLUS).Bind<MPLUS, MA, A>(ma,
                x => pred(x)
                    ? ma
                    : mzeroAsync<MPLUS, MA, A>());

        /// <summary>
        /// Filters the monad if the predicate doesn't hold
        /// </summary>
        /// <typeparam name="MA">Monad type</typeparam>
        /// <typeparam name="A">Bound type</typeparam>
        /// <param name="ma">The monads filter</param>
        /// <returns>The filtered (or not) MonadAsync</returns>
        [Pure]
        public static MA filterAsync<MPLUS, MA, A>(MA ma, Func<A, Task<bool>> pred) where MPLUS : struct, MonadAsync<MA, A> =>
            default(MPLUS).BindAsync<MPLUS, MA, A>(ma,
                async x => await pred(x).ConfigureAwait(false)
                    ? ma
                    : mzeroAsync<MPLUS, MA, A>());
    }
}
