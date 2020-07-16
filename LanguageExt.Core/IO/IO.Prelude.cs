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
        [Pure, MethodImpl(mops)]
        public static IO<Env, A> EffectMaybe<Env, A>(Func<Env, ValueTask<Fin<A>>> f) where Env : Cancellable =>
            IO<Env, A>.EffectMaybe(f);
        
        [Pure, MethodImpl(mops)]
        public static IO<Env, A> Effect<Env, A>(Func<Env, ValueTask<A>> f) where Env : Cancellable =>
            IO<Env, A>.Effect(f);
        
        [Pure, MethodImpl(mops)]
        public static IO<A> EffectMaybe<A>(Func<ValueTask<Fin<A>>> f) =>
            IO<A>.EffectMaybe(f);
        
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
        public static IO<OuterEnv, A> local<OuterEnv, InnerEnv, A>(Func<OuterEnv, InnerEnv> f, IO<InnerEnv, A> ma) 
            where OuterEnv : Cancellable
            where InnerEnv : Cancellable =>
            new IO<OuterEnv, A>(ThunkAsync<OuterEnv, A>.Lazy(oenv => ma.thunk.Value(f(oenv))));

        /// <summary>
        /// Asynchronous environment
        /// </summary>
        [Pure, MethodImpl(IO.mops)]
        public static IO<Env, Env> env<Env>() where Env : Cancellable => 
            new IO<Env, Env>(ThunkAsync<Env, Env>.Lazy(x => new ValueTask<Fin<Env>>(Fin<Env>.Succ(x))));
        
        /// <summary>
        /// Queues the provided asynchronous IO operation to be run on a new thread
        /// </summary>
        /// <param name="ma">Operation to run on the new thread</param>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="A">Bound value</typeparam>
        /// <returns>Non-blocking, returns immediately</returns>
        public static IO<Env, Unit> fork<Env, A>(IO<Env, A> ma) where Env : Cancellable =>
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
    }
}