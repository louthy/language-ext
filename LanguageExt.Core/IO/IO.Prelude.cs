using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt.Common;
using LanguageExt.Interfaces;
using LanguageExt.Thunks;

namespace LanguageExt
{
    public static partial class IO
    {
        /// <summary>
        /// Construct an effect that will either succeed, have an exceptional, or unexceptional failure
        /// </summary>
        /// <param name="f">Function to capture the effect</param>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <returns>Asynchronous IO monad that captures the effect</returns>
        [Pure, MethodImpl(mops)]
        public static IO<Env, A> EffectMaybe<Env, A>(Func<Env, ValueTask<Fin<A>>> f) where Env : struct, HasCancel<Env> =>
            IO<Env, A>.EffectMaybe(f);
        
        /// <summary>
        /// Construct an effect that will either succeed, have an exceptional, or unexceptional failure
        /// </summary>
        /// <param name="f">Function to capture the effect</param>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <returns>Asynchronous IO monad that captures the effect</returns>
        [Pure, MethodImpl(mops)]
        public static IO<A> EffectMaybe<A>(Func<ValueTask<Fin<A>>> f) =>
            IO<A>.EffectMaybe(f);
        
        /// <summary>
        /// Construct an effect that will either succeed or have an exceptional failure
        /// </summary>
        /// <param name="f">Function to capture the effect</param>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <returns>Asynchronous IO monad that captures the effect</returns>
        [Pure, MethodImpl(mops)]
        public static IO<Env, A> Effect<Env, A>(Func<Env, ValueTask<A>> f) where Env : struct, HasCancel<Env>  =>
            IO<Env, A>.Effect(f);
        
        /// <summary>
        /// Construct an effect that will either succeed or have an exceptional failure
        /// </summary>
        /// <param name="f">Function to capture the effect</param>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <returns>Asynchronous IO monad that captures the effect</returns>
        [Pure, MethodImpl(mops)]
        public static IO<A> Effect<A>(Func<ValueTask<A>> f) =>
            IO<A>.Effect(f);

        /// <summary>
        /// Create a new local context for the environment by mapping the outer environment and then
        /// using the result as a new context when running the IO monad provided
        /// </summary>
        /// <param name="f">Function to map the outer environment into a new one to run `ma`</param>
        /// <param name="ma">IO monad to run in the new context</param>
        [Pure, MethodImpl(IO.mops)]
        public static IO<OuterEnv, A> localIO<OuterEnv, InnerEnv, A>(Func<OuterEnv, InnerEnv> f, IO<InnerEnv, A> ma) 
            where OuterEnv : struct, HasCancel<OuterEnv>
            where InnerEnv : struct, HasCancel<InnerEnv> =>
            new IO<OuterEnv, A>(ThunkAsync<OuterEnv, A>.Lazy(oenv => ma.thunk.Value(f(oenv))));

        /// <summary>
        /// Environment
        /// </summary>
        [Pure, MethodImpl(IO.mops)]
        public static SIO<Env, Env> runtime<Env>() => 
            new SIO<Env, Env>(Thunk<Env, Env>.Lazy(Fin<Env>.Succ));

        /// <summary>
        /// Queues the provided asynchronous IO operation to be run on a new thread
        /// </summary>
        /// <param name="ma">Operation to run on the new thread</param>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="A">Bound value</typeparam>
        /// <returns>Non-blocking, returns immediately</returns>
        public static IO<Env, Unit> fork<Env, A>(IO<Env, A> ma) where Env : struct, HasCancel<Env> =>
            EffectMaybe<Env, Unit>(env =>
            {
                try
                {
                    ThreadPool.QueueUserWorkItem(_ => ma.RunIO(env));
                    return new ValueTask<Fin<Unit>>(Fin<Unit>.Succ(default));
                }
                catch (Exception e)
                {
                    return new ValueTask<Fin<Unit>>(Fin<Unit>.Fail(e));
                }
            });

        /// <summary>
        /// Queues the provided asynchronous IO operation to be run on a new thread
        /// </summary>
        /// <param name="ma">Operation to run on the new thread</param>
        /// <typeparam name="A">Bound value</typeparam>
        /// <returns>Non-blocking, returns immediately</returns>
        public static IO<Unit> fork<A>(IO<A> ma) =>
            EffectMaybe<Unit>(() =>
            {
                try
                {
                    ThreadPool.QueueUserWorkItem(_ => ma.RunIO());
                    return new ValueTask<Fin<Unit>>(Fin<Unit>.Succ(default));
                }
                catch (Exception e)
                {
                    return new ValueTask<Fin<Unit>>(Fin<Unit>.Fail(e));
                }
            });

        /// <summary>
        /// Cancel the asynchronous operation
        /// </summary>
        /// <typeparam name="Env">Environment</typeparam>
        /// <returns>Unit</returns>
        public static IO<Env, Unit> cancel<Env>() where Env : struct, HasCancel<Env> =>
            from src in cancelTokenSource<Env>()
            from res in IO.EffectMaybe<Env, Unit>(env =>
            {
                if (src == null)
                {
                    return Fin<Unit>.Fail(Error.New($"Environment: '{typeof(Env).FullName}' hasn't been initialised with a valid CancellationTokenSource")).AsValueTask();
                }
                else
                {
                    src.Cancel();
                    return Fin<Unit>.Succ(default).AsValueTask();
                }
            })
            select res;

        /// <summary>
        /// Cancellation token
        /// </summary>
        /// <typeparam name="Env">Runtime environment</typeparam>
        /// <returns>CancellationToken</returns>
        public static SIO<Env, CancellationToken> cancelToken<Env>() 
            where Env : struct, HasCancel<Env> =>
            default(Env).Token;

        /// <summary>
        /// Cancellation token source
        /// </summary>
        /// <typeparam name="Env">Runtime environment</typeparam>
        /// <returns>CancellationTokenSource</returns>
        public static SIO<Env, CancellationTokenSource> cancelTokenSource<Env>() 
            where Env : struct, HasCancel<Env> =>
            default(Env).CancellationTokenSource;
    }
}
