using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt.Common;
using LanguageExt.Effects.Traits;
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
        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, A> AffMaybe<Env, A>(Func<Env, ValueTask<Fin<A>>> f) where Env : struct, HasCancel<Env> =>
            LanguageExt.Aff<Env, A>.EffectMaybe(f);

        /// <summary>
        /// Construct an effect that will either succeed, have an exceptional, or unexceptional failure
        /// </summary>
        /// <param name="f">Function to capture the effect</param>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <returns>Asynchronous IO monad that captures the effect</returns>
        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<A> AffMaybe<A>(Func<ValueTask<Fin<A>>> f) =>
            LanguageExt.Aff<A>.EffectMaybe(f);

        /// <summary>
        /// Construct an effect that will either succeed or have an exceptional failure
        /// </summary>
        /// <param name="f">Function to capture the effect</param>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <returns>Asynchronous IO monad that captures the effect</returns>
        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, A> Aff<Env, A>(Func<Env, ValueTask<A>> f) where Env : struct, HasCancel<Env> =>
            LanguageExt.Aff<Env, A>.Effect(f);

        /// <summary>
        /// Construct an effect that will either succeed or have an exceptional failure
        /// </summary>
        /// <param name="f">Function to capture the effect</param>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <returns>Asynchronous IO monad that captures the effect</returns>
        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<A> Aff<A>(Func<ValueTask<A>> f) =>
            LanguageExt.Aff<A>.Effect(f);

        /// <summary>
        /// Construct an successful effect with a pure value
        /// </summary>
        /// <param name="value">Pure value to construct the monad with</param>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <returns>Asynchronous IO monad that captures the pure value</returns>
        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<A> SuccessAff<A>(A value) =>
            LanguageExt.Aff<A>.Success(value);

        /// <summary>
        /// Construct an successful effect with a pure value
        /// </summary>
        /// <param name="value">Pure value to construct the monad with</param>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <returns>Asynchronous IO monad that captures the pure value</returns>
        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<RT, A> SuccessAff<RT, A>(A value) where RT : struct, HasCancel<RT> =>
            LanguageExt.Aff<RT, A>.Success(value);

        /// <summary>
        /// Construct a failed effect
        /// </summary>
        /// <param name="error">Error that represents the failure</param>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <returns>Synchronous IO monad that captures the failure</returns>
        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<A> FailAff<A>(Error error) =>
            LanguageExt.Aff<A>.Fail(error);

        /// <summary>
        /// Construct a failed effect
        /// </summary>
        /// <param name="error">Error that represents the failure</param>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <returns>Synchronous IO monad that captures the failure</returns>
        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<RT, A> FailAff<RT, A>(Error error) where RT : struct, HasCancel<RT> =>
            LanguageExt.Aff<RT, A>.Fail(error);

        /// <summary>
        /// Create a new local context for the environment by mapping the outer environment and then
        /// using the result as a new context when running the IO monad provided
        /// </summary>
        /// <param name="f">Function to map the outer environment into a new one to run `ma`</param>
        /// <param name="ma">IO monad to run in the new context</param>
        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<OuterEnv, A> localAff<OuterEnv, InnerEnv, A>(Func<OuterEnv, InnerEnv> f, Aff<InnerEnv, A> ma)
            where OuterEnv : struct, HasCancel<OuterEnv>
            where InnerEnv : struct, HasCancel<InnerEnv> =>
            new Aff<OuterEnv, A>(ThunkAsync<OuterEnv, A>.Lazy(oenv => ma.Thunk.Value(f(oenv))));

        /// <summary>
        /// Environment
        /// </summary>
        [Pure, MethodImpl(AffOpt.mops)]
        public static Eff<Env, Env> runtime<Env>() where Env : struct =>
            new Eff<Env, Env>(Thunk<Env, Env>.Lazy(Fin<Env>.Succ));

        /// <summary>
        /// Launch the async computation without awaiting the result
        /// </summary>
        /// <remarks>
        /// If the parent expression has `cancel` called on it, then it will also cancel the forked child
        /// expression.
        ///
        /// `Fork` returns an `Eff<Unit>` as its bound result value.  If you run it, it will cancel the
        /// forked child expression.
        /// </remarks>
        /// <returns>Returns an `Eff<Unit>` as its bound value.  If it runs, it will cancel the
        /// forked child expression</returns>
        public static Eff<Env, Eff<Unit>> fork<Env, A>(Aff<Env, A> ma) where Env : struct, HasCancel<Env> =>
            ma.Fork();

        /// <summary>
        /// Launch the async computation without awaiting the result
        /// </summary>
        public static Eff<Unit> fork<A>(Aff<A> ma) =>
            ma.Fork();

        /// <summary>
        /// Create a new cancellation context and run the provided Aff in that context
        /// </summary>
        /// <param name="ma">Operation to run in the next context</param>
        /// <typeparam name="Env">Runtime environment</typeparam>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <returns>An asynchronous effect that captures the operation running in context</returns>
        public static Aff<Env, A> localCancel<Env, A>(Aff<Env, A> ma) where Env : struct, HasCancel<Env> =>
            localAff<Env, Env, A>(static env => env.LocalCancel, ma);

        /// <summary>
        /// Create a new cancellation context and run the provided Aff in that context
        /// </summary>
        /// <param name="ma">Operation to run in the next context</param>
        /// <typeparam name="Env">Runtime environment</typeparam>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <returns>An asynchronous effect that captures the operation running in context</returns>
        public static Eff<Env, A> localCancel<Env, A>(Eff<Env, A> ma) where Env : struct, HasCancel<Env> =>
            localEff<Env, Env, A>(static env => env.LocalCancel, ma);

        /// <summary>
        /// Cancel the asynchronous operation
        /// </summary>
        /// <typeparam name="Env">Environment</typeparam>
        /// <returns>Unit</returns>
        public static Eff<Env, Unit> cancel<Env>() where Env : struct, HasCancel<Env> =>
            EffMaybe<Env, Unit>(static env =>
                                {
                                    if (env.CancellationTokenSource == null)
                                    {
                                        return Fin<Unit>.Fail(Error.New($"Environment: '{typeof(Env).FullName}' hasn't been initialised with a valid CancellationTokenSource"));
                                    }
                                    else
                                    {
                                        env.CancellationTokenSource.Cancel();
                                        return Fin<Unit>.Succ(default);
                                    }
                                });

        /// <summary>
        /// Cancellation token
        /// </summary>
        /// <typeparam name="Env">Runtime environment</typeparam>
        /// <returns>CancellationToken</returns>
        public static Eff<Env, CancellationToken> cancelToken<Env>()
            where Env : struct, HasCancel<Env> =>
            Eff<Env, CancellationToken>(static env => env.CancellationToken);

        /// <summary>
        /// Cancellation token source
        /// </summary>
        /// <typeparam name="Env">Runtime environment</typeparam>
        /// <returns>CancellationTokenSource</returns>
        public static Eff<Env, CancellationTokenSource> cancelTokenSource<Env>()
            where Env : struct, HasCancel<Env> =>
            Eff<Env, CancellationTokenSource>(static env => env.CancellationTokenSource);

        /// <summary>
        /// Force the operation to end after a time out delay
        /// </summary>
        /// <param name="timeoutDelay">Delay for the time out</param>
        /// <returns>Either success if the operation completed before the timeout, or Errors.TimedOut</returns>
        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<RT, A> timeout<RT, A>(TimeSpan timeoutDelay, Aff<RT, A> ma) where RT : struct, HasCancel<RT> =>
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
        public static Aff<Env, R> aff<Env, A, R>(Aff<Env, A> ma,
            Aff<Env, R> mr) where Env : struct, HasCancel<Env> =>
            from a in ma
            from r in mr
            select r;

        /// <summary>
        /// Sequentially run IO operations, returning the result of the last one
        /// </summary>
        public static Aff<Env, R> aff<Env, A, B, R>(Aff<Env, A> ma,
            Aff<Env, B> mb,
            Aff<Env, R> mr) where Env : struct, HasCancel<Env> =>
            from a in ma
            from b in mb
            from r in mr
            select r;

        /// <summary>
        /// Sequentially run IO operations, returning the result of the last one
        /// </summary>
        public static Aff<Env, R> aff<Env, A, B, C, R>(Aff<Env, A> ma,
            Aff<Env, B> mb,
            Aff<Env, C> mc,
            Aff<Env, R> mr) where Env : struct, HasCancel<Env> =>
            from a in ma
            from b in mb
            from c in mc
            from r in mr
            select r;

        /// <summary>
        /// Sequentially run IO operations, returning the result of the last one
        /// </summary>
        public static Aff<Env, R> aff<Env, A, B, C, D, R>(Aff<Env, A> ma,
            Aff<Env, B> mb,
            Aff<Env, C> mc,
            Aff<Env, D> md,
            Aff<Env, R> mr) where Env : struct, HasCancel<Env> =>
            from a in ma
            from b in mb
            from c in mc
            from d in md
            from r in mr
            select r;

        /// <summary>
        /// Sequentially run IO operations, returning the result of the last one
        /// </summary>
        public static Aff<Env, R> aff<Env, A, B, C, D, E, R>(Aff<Env, A> ma,
            Aff<Env, B> mb,
            Aff<Env, C> mc,
            Aff<Env, D> md,
            Aff<Env, E> me,
            Aff<Env, R> mr) where Env : struct, HasCancel<Env> =>
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
        public static Aff<Env, R> aff<Env, A, B, C, D, E, F, R>(Aff<Env, A> ma,
            Aff<Env, B> mb,
            Aff<Env, C> mc,
            Aff<Env, D> md,
            Aff<Env, E> me,
            Aff<Env, F> mf,
            Aff<Env, R> mr) where Env : struct, HasCancel<Env> =>
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
        public static Aff<Env, R> aff<Env, A, B, C, D, E, F, G, R>(Aff<Env, A> ma,
            Aff<Env, B> mb,
            Aff<Env, C> mc,
            Aff<Env, D> md,
            Aff<Env, E> me,
            Aff<Env, F> mf,
            Aff<Env, G> mg,
            Aff<Env, R> mr) where Env : struct, HasCancel<Env> =>
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
        public static Eff<Env, R> eff<Env, A, R>(Eff<Env, A> ma,
            Eff<Env, R> mr) where Env : struct =>
            from a in ma
            from r in mr
            select r;

        /// <summary>
        /// Sequentially run IO operations, returning the result of the last one
        /// </summary>
        public static Eff<Env, R> eff<Env, A, B, R>(Eff<Env, A> ma,
            Eff<Env, B> mb,
            Eff<Env, R> mr) where Env : struct =>
            from a in ma
            from b in mb
            from r in mr
            select r;

        /// <summary>
        /// Sequentially run IO operations, returning the result of the last one
        /// </summary>
        public static Eff<Env, R> eff<Env, A, B, C, R>(Eff<Env, A> ma,
            Eff<Env, B> mb,
            Eff<Env, C> mc,
            Eff<Env, R> mr) where Env : struct =>
            from a in ma
            from b in mb
            from c in mc
            from r in mr
            select r;

        /// <summary>
        /// Sequentially run IO operations, returning the result of the last one
        /// </summary>
        public static Eff<Env, R> eff<Env, A, B, C, D, R>(Eff<Env, A> ma,
            Eff<Env, B> mb,
            Eff<Env, C> mc,
            Eff<Env, D> md,
            Eff<Env, R> mr) where Env : struct =>
            from a in ma
            from b in mb
            from c in mc
            from d in md
            from r in mr
            select r;

        /// <summary>
        /// Sequentially run IO operations, returning the result of the last one
        /// </summary>
        public static Eff<Env, R> eff<Env, A, B, C, D, E, R>(Eff<Env, A> ma,
            Eff<Env, B> mb,
            Eff<Env, C> mc,
            Eff<Env, D> md,
            Eff<Env, E> me,
            Eff<Env, R> mr) where Env : struct =>
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
        public static Eff<Env, R> eff<Env, A, B, C, D, E, F, R>(Eff<Env, A> ma,
            Eff<Env, B> mb,
            Eff<Env, C> mc,
            Eff<Env, D> md,
            Eff<Env, E> me,
            Eff<Env, F> mf,
            Eff<Env, R> mr) where Env : struct =>
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
        public static Eff<Env, R> eff<Env, A, B, C, D, E, F, G, R>(Eff<Env, A> ma,
            Eff<Env, B> mb,
            Eff<Env, C> mc,
            Eff<Env, D> md,
            Eff<Env, E> me,
            Eff<Env, F> mf,
            Eff<Env, G> mg,
            Eff<Env, R> mr) where Env : struct =>
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
