using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt.Common;
using LanguageExt.Effects.Traits;
using LanguageExt.Pipes;
using LanguageExt.Thunks;

namespace LanguageExt
{
    public static partial class Prelude
    {
        /// <summary>
        /// Construct an effect that will either succeed, have an exceptional, or unexceptional failure
        /// </summary>
        /// <param name="f">Function to capture the effect</param>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <returns>Asynchronous IO monad that captures the effect</returns>
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, A> AffMaybe<RT, A>(Func<RT, ValueTask<Fin<A>>> f) where RT : struct, HasCancel<RT> =>
            LanguageExt.Aff<RT, A>.EffectMaybe(f);

        /// <summary>
        /// Construct an effect that will either succeed, have an exceptional, or unexceptional failure
        /// </summary>
        /// <param name="f">Function to capture the effect</param>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <returns>Asynchronous IO monad that captures the effect</returns>
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<A> AffMaybe<A>(Func<ValueTask<Fin<A>>> f) =>
            LanguageExt.Aff<A>.EffectMaybe(f);

        /// <summary>
        /// Construct an effect that will either succeed or have an exceptional failure
        /// </summary>
        /// <param name="f">Function to capture the effect</param>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <returns>Asynchronous IO monad that captures the effect</returns>
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, A> Aff<RT, A>(Func<RT, ValueTask<A>> f) where RT : struct, HasCancel<RT> =>
            LanguageExt.Aff<RT, A>.Effect(f);

        /// <summary>
        /// Construct an effect that will either succeed or have an exceptional failure
        /// </summary>
        /// <param name="f">Function to capture the effect</param>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <returns>Asynchronous IO monad that captures the effect</returns>
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<A> Aff<A>(Func<ValueTask<A>> f) =>
            LanguageExt.Aff<A>.Effect(f);

        /// <summary>
        /// Construct an successful effect with a pure value
        /// </summary>
        /// <param name="value">Pure value to construct the monad with</param>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <returns>Asynchronous IO monad that captures the pure value</returns>
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<A> SuccessAff<A>(A value) =>
            LanguageExt.Aff<A>.Success(value);

        /// <summary>
        /// Construct an successful effect with a pure value
        /// </summary>
        /// <param name="value">Pure value to construct the monad with</param>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <returns>Asynchronous IO monad that captures the pure value</returns>
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, A> SuccessAff<RT, A>(A value) where RT : struct, HasCancel<RT> =>
            LanguageExt.Aff<RT, A>.Success(value);

        /// <summary>
        /// Construct a failed effect
        /// </summary>
        /// <param name="error">Error that represents the failure</param>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <returns>Synchronous IO monad that captures the failure</returns>
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<A> FailAff<A>(Error error) =>
            LanguageExt.Aff<A>.Fail(error);

        /// <summary>
        /// Construct a failed effect
        /// </summary>
        /// <param name="error">Error that represents the failure</param>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <returns>Synchronous IO monad that captures the failure</returns>
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, A> FailAff<RT, A>(Error error) where RT : struct, HasCancel<RT> =>
            LanguageExt.Aff<RT, A>.Fail(error);

        /// <summary>
        /// Create a new local context for the environment by mapping the outer environment and then
        /// using the result as a new context when running the IO monad provided
        /// </summary>
        /// <param name="f">Function to map the outer environment into a new one to run `ma`</param>
        /// <param name="ma">IO monad to run in the new context</param>
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<OuterRT, A> localAff<OuterRT, InnerRT, A>(Func<OuterRT, InnerRT> f, Aff<InnerRT, A> ma)
            where OuterRT : struct, HasCancel<OuterRT>
            where InnerRT : struct, HasCancel<InnerRT> =>
            new(oenv => ma.Thunk(f(oenv)));

        /// <summary>
        /// Runtime
        /// </summary>
        [Pure, MethodImpl(Opt.Default)]
        public static Eff<RT, RT> runtime<RT>() where RT : struct =>
            new (Fin<RT>.Succ);

        /// <summary>
        /// Launch the async computation without awaiting the result
        /// </summary>
        /// <remarks>
        /// If the parent expression has `cancel` called on it, then it will also cancel the forked child
        /// expression.
        ///
        /// `Fork` returns an `Eff Unit` as its bound result value.  If you run it, it will cancel the
        /// forked child expression.
        /// </remarks>
        /// <returns>Returns an `Eff Unit` as its bound value.  If it runs, it will cancel the
        /// forked child expression</returns>
        public static Eff<RT, Eff<Unit>> fork<RT, A>(Aff<RT, A> ma) where RT : struct, HasCancel<RT> =>
            ma.Fork();

        /// <summary>
        /// Launch the async computation without awaiting the result
        /// </summary>
        /// <remarks>
        /// If the parent expression has `cancel` called on it, then it will also cancel the forked child
        /// expression.
        ///
        /// `Fork` returns an `Eff Unit` as its bound result value.  If you run it, it will cancel the
        /// forked child expression.
        /// </remarks>
        /// <returns>Returns an `Eff Unit` as its bound value.  If it runs, it will cancel the
        /// forked child expression</returns>
        public static Eff<RT, Eff<Unit>> fork<RT, A>(Effect<RT, A> ma) where RT : struct, HasCancel<RT> =>
            ma.RunEffect().Fork();

        /// <summary>
        /// Launch the async computation without awaiting the result
        /// </summary>
        public static Eff<Unit> fork<A>(Aff<A> ma) =>
            ma.Fork();

        /// <summary>
        /// Create a new cancellation context and run the provided Aff in that context
        /// </summary>
        /// <param name="ma">Operation to run in the next context</param>
        /// <typeparam name="RT">Runtime environment</typeparam>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <returns>An asynchronous effect that captures the operation running in context</returns>
        public static Aff<RT, A> localCancel<RT, A>(Aff<RT, A> ma) where RT : struct, HasCancel<RT> =>
            AffMaybe<RT, A>(async rt =>
                            {
                                var rt1 = rt.LocalCancel;
                                using (rt1.CancellationTokenSource)
                                {
                                    return await ma.Run(rt1).ConfigureAwait(false);
                                }
                            });
        
        /// <summary>
        /// Create a new cancellation context and run the provided Aff in that context
        /// </summary>
        /// <param name="ma">Operation to run in the next context</param>
        /// <typeparam name="RT">Runtime environment</typeparam>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <returns>An asynchronous effect that captures the operation running in context</returns>
        public static Eff<RT, A> localCancel<RT, A>(Eff<RT, A> ma) where RT : struct, HasCancel<RT> =>
            EffMaybe<RT, A>(rt =>
                            {
                                var rt1 = rt.LocalCancel;
                                using (rt1.CancellationTokenSource)
                                {
                                    return ma.Run(rt1);
                                }
                            });
                                                               
        /// <summary>
        /// Cancel the asynchronous operation
        /// </summary>
        /// <typeparam name="RT">Runtime</typeparam>
        /// <returns>Unit</returns>
        public static Aff<RT, Unit> cancel<RT>() where RT : struct, HasCancel<RT> =>            
            AffMaybe<RT, Unit>(static env =>
                                {
                                    if (env.CancellationTokenSource == null)
                                    {
                                        return Fin<Unit>.Fail(Error.New($"Runtime: '{typeof(RT).FullName}' hasn't been initialised with a valid CancellationTokenSource")).AsValueTask();
                                    }
                                    else
                                    {
                                        env.CancellationTokenSource.Cancel();
                                        return Fin<Unit>.Succ(default).AsValueTask();
                                    }
                                });

        /// <summary>
        /// Cancellation token
        /// </summary>
        /// <typeparam name="RT">Runtime environment</typeparam>
        /// <returns>CancellationToken</returns>
        public static Eff<RT, CancellationToken> cancelToken<RT>()
            where RT : struct, HasCancel<RT> =>
            Eff<RT, CancellationToken>(static env => env.CancellationToken);

        /// <summary>
        /// Cancellation token source
        /// </summary>
        /// <typeparam name="RT">Runtime environment</typeparam>
        /// <returns>CancellationTokenSource</returns>
        public static Eff<RT, CancellationTokenSource> cancelTokenSource<RT>()
            where RT : struct, HasCancel<RT> =>
            Eff<RT, CancellationTokenSource>(static env => env.CancellationTokenSource);

        /// <summary>
        /// Force the operation to end after a time out delay
        /// </summary>
        /// <param name="timeoutDelay">Delay for the time out</param>
        /// <returns>Either success if the operation completed before the timeout, or Errors.TimedOut</returns>
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, A> timeout<RT, A>(TimeSpan timeoutDelay, Aff<RT, A> ma) where RT : struct, HasCancel<RT> =>
            ma.Timeout(timeoutDelay);


        /// <summary>
        /// Force the operation to end after a time out delay
        /// </summary>
        /// <remarks>Note, the original operation continues even after this returns.  To cancel the original operation
        /// at the same time as the timeout triggers, use Aff<RT, A> instead of Aff<A> - as it supports cancellation
        /// tokens, and so can automatically cancel the long-running operation</remarks>
        /// <param name="timeoutDelay">Delay for the time out</param>
        /// <returns>Either success if the operation completed before the timeout, or Errors.TimedOut</returns>
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<A> timeout<A>(TimeSpan timeoutDelay, Aff<A> ma) =>
            ma.Timeout(timeoutDelay);

        /// <summary>
        /// Unit effect
        /// </summary>
        /// <remarks>Always succeeds with a Unit value</remarks>
        public static readonly Aff<Unit> unitAff = SuccessEff(unit);

        /// <summary>
        /// True effect
        /// </summary>
        /// <remarks>Always succeeds with a boolean true value</remarks>
        public static readonly Aff<bool> trueAff = SuccessEff(true);

        /// <summary>
        /// False effect
        /// </summary>
        /// <remarks>Always succeeds with a boolean false value</remarks>
        public static readonly Aff<bool> falseAff = SuccessEff(false);

        /// <summary>
        /// Sequentially run IO operations, returning the result of the last one
        /// </summary>
        public static Aff<RT, R> aff<RT, A, R>(
            Aff<RT, A> ma,
            Aff<RT, R> mr) 
            where RT : struct, HasCancel<RT> =>
            from a in ma
            from r in mr
            select r;

        /// <summary>
        /// Sequentially run IO operations, returning the result of the last one
        /// </summary>
        public static Aff<RT, R> aff<RT, A, B, R>(
            Aff<RT, A> ma,
            Aff<RT, B> mb,
            Aff<RT, R> mr) 
            where RT : struct, HasCancel<RT> =>
            from a in ma
            from b in mb
            from r in mr
            select r;

        /// <summary>
        /// Sequentially run IO operations, returning the result of the last one
        /// </summary>
        public static Aff<RT, R> aff<RT, A, B, C, R>(
            Aff<RT, A> ma,
            Aff<RT, B> mb,
            Aff<RT, C> mc,
            Aff<RT, R> mr) 
            where RT : struct, HasCancel<RT> =>
            from a in ma
            from b in mb
            from c in mc
            from r in mr
            select r;

        /// <summary>
        /// Sequentially run IO operations, returning the result of the last one
        /// </summary>
        public static Aff<RT, R> aff<RT, A, B, C, D, R>(
            Aff<RT, A> ma,
            Aff<RT, B> mb,
            Aff<RT, C> mc,
            Aff<RT, D> md,
            Aff<RT, R> mr) 
            where RT : struct, HasCancel<RT> =>
            from a in ma
            from b in mb
            from c in mc
            from d in md
            from r in mr
            select r;

        /// <summary>
        /// Sequentially run IO operations, returning the result of the last one
        /// </summary>
        public static Aff<RT, R> aff<RT, A, B, C, D, E, R>(
            Aff<RT, A> ma,
            Aff<RT, B> mb,
            Aff<RT, C> mc,
            Aff<RT, D> md,
            Aff<RT, E> me,
            Aff<RT, R> mr) 
            where RT : struct, HasCancel<RT> =>
            from a in ma
            from b in mb
            from c in mc
            from d in md
            from e in me
            from r in mr
            select r;

        /// <summary>
        /// Sequentially run IO operations, returning the result of the last one
        /// </summary>
        public static Aff<RT, R> aff<RT, A, B, C, D, E, F, R>(
            Aff<RT, A> ma,
            Aff<RT, B> mb,
            Aff<RT, C> mc,
            Aff<RT, D> md,
            Aff<RT, E> me,
            Aff<RT, F> mf,
            Aff<RT, R> mr) 
            where RT : struct, HasCancel<RT> =>
            from a in ma
            from b in mb
            from c in mc
            from d in md
            from e in me
            from f in mf
            from r in mr
            select r;

        /// <summary>
        /// Sequentially run IO operations, returning the result of the last one
        /// </summary>
        public static Aff<RT, R> aff<RT, A, B, C, D, E, F, G, R>(
            Aff<RT, A> ma,
            Aff<RT, B> mb,
            Aff<RT, C> mc,
            Aff<RT, D> md,
            Aff<RT, E> me,
            Aff<RT, F> mf,
            Aff<RT, G> mg,
            Aff<RT, R> mr) 
            where RT : struct, HasCancel<RT> =>
            from a in ma
            from b in mb
            from c in mc
            from d in md
            from e in me
            from f in mf
            from r in mr
            select r;


        // Eff

        /// <summary>
        /// Sequentially run IO operations, returning the result of the last one
        /// </summary>
        public static Eff<RT, R> eff<RT, A, R>(
            Eff<RT, A> ma,
            Eff<RT, R> mr) 
            where RT : struct =>
            from a in ma
            from r in mr
            select r;

        /// <summary>
        /// Sequentially run IO operations, returning the result of the last one
        /// </summary>
        public static Eff<RT, R> eff<RT, A, B, R>(
            Eff<RT, A> ma,
            Eff<RT, B> mb,
            Eff<RT, R> mr) 
            where RT : struct =>
            from a in ma
            from b in mb
            from r in mr
            select r;

        /// <summary>
        /// Sequentially run IO operations, returning the result of the last one
        /// </summary>
        public static Eff<RT, R> eff<RT, A, B, C, R>(
            Eff<RT, A> ma,
            Eff<RT, B> mb,
            Eff<RT, C> mc,
            Eff<RT, R> mr)
            where RT : struct =>
            from a in ma
            from b in mb
            from c in mc
            from r in mr
            select r;

        /// <summary>
        /// Sequentially run IO operations, returning the result of the last one
        /// </summary>
        public static Eff<RT, R> eff<RT, A, B, C, D, R>(
            Eff<RT, A> ma,
            Eff<RT, B> mb,
            Eff<RT, C> mc,
            Eff<RT, D> md,
            Eff<RT, R> mr) 
            where RT : struct =>
            from a in ma
            from b in mb
            from c in mc
            from d in md
            from r in mr
            select r;

        /// <summary>
        /// Sequentially run IO operations, returning the result of the last one
        /// </summary>
        public static Eff<RT, R> eff<RT, A, B, C, D, E, R>(
            Eff<RT, A> ma,
            Eff<RT, B> mb,
            Eff<RT, C> mc,
            Eff<RT, D> md,
            Eff<RT, E> me,
            Eff<RT, R> mr) where RT : struct =>
            from a in ma
            from b in mb
            from c in mc
            from d in md
            from e in me
            from r in mr
            select r;

        /// <summary>
        /// Sequentially run IO operations, returning the result of the last one
        /// </summary>
        public static Eff<RT, R> eff<RT, A, B, C, D, E, F, R>(
            Eff<RT, A> ma,
            Eff<RT, B> mb,
            Eff<RT, C> mc,
            Eff<RT, D> md,
            Eff<RT, E> me,
            Eff<RT, F> mf,
            Eff<RT, R> mr) 
            where RT : struct =>
            from a in ma
            from b in mb
            from c in mc
            from d in md
            from e in me
            from f in mf
            from r in mr
            select r;

        /// <summary>
        /// Sequentially run IO operations, returning the result of the last one
        /// </summary>
        public static Eff<RT, R> eff<RT, A, B, C, D, E, F, G, R>(
            Eff<RT, A> ma,
            Eff<RT, B> mb,
            Eff<RT, C> mc,
            Eff<RT, D> md,
            Eff<RT, E> me,
            Eff<RT, F> mf,
            Eff<RT, G> mg,
            Eff<RT, R> mr) 
            where RT : struct =>
            from a in ma
            from b in mb
            from c in mc
            from d in md
            from e in me
            from f in mf
            from g in mg
            from r in mr
            select r;
    }
}
