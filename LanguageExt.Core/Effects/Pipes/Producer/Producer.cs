using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt.Common;
using LanguageExt.Effects.Traits;
using static LanguageExt.Pipes.Proxy;
using static LanguageExt.Prelude;

namespace LanguageExt.Pipes
{
    /// <summary>
    /// Producers can only `yield`
    /// </summary>
    /// <remarks>
    ///       Upstream | Downstream
    ///           +---------+
    ///           |         |
    ///     Void <==       <== Unit
    ///           |         |
    ///     Unit ==>       ==> OUT
    ///           |    |    |
    ///           +----|----+
    ///                |
    ///                A
    /// </remarks>
    public static class Producer
    {
        /// <summary>
        /// Monad return / pure
        /// </summary>
        [Pure, MethodImpl(mops)]
        public static Producer<RT, OUT, R> Pure<RT, OUT, R>(R value) where RT : struct, HasCancel<RT> =>
            new Pure<RT, Void, Unit, Unit, OUT, R>(value).ToProducer();
        
        /// <summary>
        /// Send a value downstream (whilst in a producer)
        /// </summary>
        /// <remarks>
        /// This is the simpler version (fewer generic arguments required) of `yield` that works
        /// for producers. 
        /// </remarks>
        [Pure, MethodImpl(mops)]
        public static Producer<RT, OUT, Unit> yield<RT, OUT>(OUT value) where RT : struct, HasCancel<RT> =>
            respond<RT, Void, Unit, Unit, OUT>(value).ToProducer();

        [Pure, MethodImpl(mops)]
        internal static Producer<RT, X, Unit> yieldAll<RT, X>(EnumerateData<X> xs)
            where RT : struct, HasCancel<RT> =>
            new Enumerate<RT, Void, Unit, Unit, X, X, Unit>(
                xs, 
                yield<RT, X>,
                Pure<RT, X, Unit>).ToProducer();

        [Pure, MethodImpl(mops)]
        public static Producer<RT, X, Unit> yieldAll<RT, X>(IEnumerable<X> xs)
            where RT : struct, HasCancel<RT> =>
            yieldAll<RT, X>(new EnumerateEnumerable<X>(xs));
        
        [Pure, MethodImpl(mops)]
        public static Producer<RT, X, Unit> yieldAll<RT, X>(IAsyncEnumerable<X> xs)
            where RT : struct, HasCancel<RT> =>
            yieldAll<RT, X>(new EnumerateAsyncEnumerable<X>(xs));
        
        [Pure, MethodImpl(mops)]
        public static Producer<RT, X, Unit> yieldAll<RT, X>(IObservable<X> xs)
            where RT : struct, HasCancel<RT> =>
            yieldAll<RT, X>(new EnumerateObservable<X>(xs));
        
        /// <summary>
        /// Repeat a monadic action indefinitely, yielding each result
        /// </summary>
        [Pure, MethodImpl(mops)]
        public static Producer<RT, A, R> repeatM<RT, A, R>(Aff<RT, A> ma) where RT : struct, HasCancel<RT> =>
            compose(lift<RT, A, A>(ma), cat<RT, A, R>());

        /// <summary>
        /// Repeat a monadic action indefinitely, yielding each result
        /// </summary>
        [Pure, MethodImpl(mops)]
        public static Producer<RT, A, Unit> repeatM<RT, A>(Aff<RT, A> ma) where RT : struct, HasCancel<RT> =>
            compose(lift<RT, A, A>(ma), cat<RT, A, Unit>());

        /// <summary>
        /// Repeat a monadic action indefinitely, yielding each result
        /// </summary>
        [Pure, MethodImpl(mops)]
        public static Producer<RT, A, R> repeatM<RT, A, R>(Eff<RT, A> ma) where RT : struct, HasCancel<RT> =>
            Proxy.compose(lift<RT, A, A>(ma), Proxy.cat<RT, A, R>());

        /// <summary>
        /// Repeat a monadic action indefinitely, yielding each result
        /// </summary>
        [Pure, MethodImpl(mops)]
        public static Producer<RT, A, Unit> repeatM<RT, A>(Eff<RT, A> ma) where RT : struct, HasCancel<RT> =>
            Proxy.compose(lift<RT, A, A>(ma), Proxy.cat<RT, A, Unit>());
        
        /// <summary>
        /// Repeat a monadic action indefinitely, yielding each result
        /// </summary>
        [Pure, MethodImpl(mops)]
        public static Producer<RT, A, R> repeatM<RT, A, R>(Aff<A> ma) where RT : struct, HasCancel<RT> =>
            Proxy.compose(lift<RT, A, A>(ma), Proxy.cat<RT, A, R>());

        /// <summary>
        /// Repeat a monadic action indefinitely, yielding each result
        /// </summary>
        [Pure, MethodImpl(mops)]
        public static Producer<RT, A, Unit> repeatM<RT, A>(Aff<A> ma) where RT : struct, HasCancel<RT> =>
            Proxy.compose(lift<RT, A, A>(ma), Proxy.cat<RT, A, Unit>());

        /// <summary>
        /// Repeat a monadic action indefinitely, yielding each result
        /// </summary>
        [Pure, MethodImpl(mops)]
        public static Producer<RT, A, R> repeatM<RT, A, R>(Eff<A> ma) where RT : struct, HasCancel<RT> =>
            Proxy.compose(lift<RT, A, A>(ma), Proxy.cat<RT, A, R>());

        /// <summary>
        /// Repeat a monadic action indefinitely, yielding each result
        /// </summary>
        [Pure, MethodImpl(mops)]
        public static Producer<RT, A, Unit> repeatM<RT, A>(Eff<A> ma) where RT : struct, HasCancel<RT> =>
            Proxy.compose(lift<RT, A, A>(ma), Proxy.cat<RT, A, Unit>());

        
        /// <summary>
        /// Lift the IO monad into the Producer monad transformer (a specialism of the Proxy monad transformer)
        /// </summary>
        [Pure, MethodImpl(mops)]
        public static Producer<RT, OUT, R> lift<RT, OUT, R>(Eff<R> ma) where RT : struct, HasCancel<RT> =>
            lift<RT, Void, Unit, Unit, OUT, R>(ma).ToProducer();

        /// <summary>
        /// Lift the IO monad into the Producer monad transformer (a specialism of the Proxy monad transformer)
        /// </summary>
        [Pure, MethodImpl(mops)]
        public static Producer<RT, OUT, R> lift<RT, OUT, R>(Aff<R> ma) where RT : struct, HasCancel<RT> =>
            lift<RT, Void, Unit, Unit, OUT, R>(ma).ToProducer();

        /// <summary>
        /// Lift the IO monad into the Producer monad transformer (a specialism of the Proxy monad transformer)
        /// </summary>
        [Pure, MethodImpl(mops)]
        public static Producer<RT, OUT, R> lift<RT, OUT, R>(Eff<RT, R> ma) where RT : struct, HasCancel<RT> =>
            lift<RT, Void, Unit, Unit, OUT, R>(ma).ToProducer();

        /// <summary>
        /// Lift the IO monad into the Producer monad transformer (a specialism of the Proxy monad transformer)
        /// </summary>
        [Pure, MethodImpl(mops)]
        public static Producer<RT, OUT, R> lift<RT, OUT, R>(Aff<RT, R> ma) where RT : struct, HasCancel<RT> =>
            lift<RT, Void, Unit, Unit, OUT, R>(ma).ToProducer();
        
        /// <summary>
        /// Lift am IO monad into the `Proxy` monad transformer
        /// </summary>
        [Pure, MethodImpl(mops)]
        public static Producer<RT, OUT, R> use<RT, OUT, R>(Aff<R> ma) 
            where RT : struct, HasCancel<RT>
            where R : IDisposable =>
            use<RT, Void, Unit, Unit, OUT, R>(ma).ToProducer();

        /// <summary>
        /// Lift am IO monad into the `Proxy` monad transformer
        /// </summary>
        [Pure, MethodImpl(mops)]
        public static Producer<RT, OUT, R> use<RT, OUT, R>(Eff<R> ma) 
            where RT : struct, HasCancel<RT>
            where R : IDisposable =>
            use<RT, Void, Unit, Unit, OUT, R>(ma).ToProducer();

        /// <summary>
        /// Lift am IO monad into the `Proxy` monad transformer
        /// </summary>
        [Pure, MethodImpl(mops)]
        public static Producer<RT, OUT, R> use<RT, OUT, R>(Aff<RT, R> ma) 
            where RT : struct, HasCancel<RT> 
            where R : IDisposable =>
            use<RT, Void, Unit, Unit, OUT, R>(ma).ToProducer();

        /// <summary>
        /// Lift am IO monad into the `Proxy` monad transformer
        /// </summary>
        [Pure, MethodImpl(mops)]
        public static Producer<RT, OUT, R> use<RT, OUT, R>(Eff<RT, R> ma) 
            where RT : struct, HasCancel<RT> 
            where R : IDisposable =>
            use<RT, Void, Unit, Unit, OUT, R>(ma).ToProducer();
        
        
        /// <summary>
        /// Lift am IO monad into the `Proxy` monad transformer
        /// </summary>
        [Pure, MethodImpl(mops)]
        public static Producer<RT, OUT, R> use<RT, OUT, R>(Aff<R> ma, Func<R, Unit> dispose) 
            where RT : struct, HasCancel<RT> =>
            use<RT, Void, Unit, Unit, OUT, R>(ma, dispose).ToProducer();

        /// <summary>
        /// Lift am IO monad into the `Proxy` monad transformer
        /// </summary>
        [Pure, MethodImpl(mops)]
        public static Producer<RT, OUT, R> use<RT, OUT, R>(Eff<R> ma, Func<R, Unit> dispose) 
            where RT : struct, HasCancel<RT> =>
            use<RT, Void, Unit, Unit, OUT, R>(ma, dispose).ToProducer();

        /// <summary>
        /// Lift am IO monad into the `Proxy` monad transformer
        /// </summary>
        [Pure, MethodImpl(mops)]
        public static Producer<RT, OUT, R> use<RT, OUT, R>(Aff<RT, R> ma, Func<R, Unit> dispose) 
            where RT : struct, HasCancel<RT> =>
            use<RT, Void, Unit, Unit, OUT, R>(ma, dispose).ToProducer();

        /// <summary>
        /// Lift am IO monad into the `Proxy` monad transformer
        /// </summary>
        [Pure, MethodImpl(mops)]
        public static Producer<RT, OUT, R> use<RT, OUT, R>(Eff<RT, R> ma, Func<R, Unit> dispose) 
            where RT : struct, HasCancel<RT> =>
            use<RT, Void, Unit, Unit, OUT, R>(ma, dispose).ToProducer();        

        /// <summary>
        /// Release a previously used resource
        /// </summary>
        [Pure, MethodImpl(mops)]
        public static Producer<RT, OUT, Unit> release<RT, OUT, R>(R dispose) 
            where RT : struct, HasCancel<RT> =>
            Proxy.release<RT, Void, Unit, Unit, OUT, R>(dispose).ToProducer();
 
        /// <summary>
        /// Folds values coming down-stream, when the predicate returns true the folded value is yielded 
        /// </summary>
        /// <param name="Initial">Initial state</param>
        /// <param name="Fold">Fold operation</param>
        /// <param name="UntilValue">Predicate</param>
        /// <returns>A pipe that folds</returns>
        public static Producer<RT, S, Unit> FoldUntil<RT, S, A>(this Producer<RT, S, A> ma, S Initial, Func<S, A, S> Fold, Func<A, bool> UntilValue)
            where RT : struct, HasCancel<RT>
        {
            var state = Initial;
            return ma.Bind(
                x =>
                {
                    if (UntilValue(x))
                    {
                        var nstate = state;
                        state = Initial;
                        return yield<RT, S>(nstate);
                    }
                    else
                    {
                        state = Fold(state, x);
                        return Pure<RT, S, Unit>(unit);
                    }
                });
        }
 
        /// <summary>
        /// Folds values coming down-stream, when the predicate returns true the folded value is yielded 
        /// </summary>
        /// <param name="Initial">Initial state</param>
        /// <param name="Fold">Fold operation</param>
        /// <param name="UntilValue">Predicate</param>
        /// <returns>A pipe that folds</returns>
        public static Producer<RT, S, Unit> FoldWhile<RT, S, A>(this Producer<RT, S, A> ma, S Initial, Func<S, A, S> Fold, Func<A, bool> WhileValue)
            where RT : struct, HasCancel<RT>
        {
            var state = Initial;
            return ma.Bind(
                x =>
                {
                    if (WhileValue(x))
                    {
                        state = Fold(state, x);
                        return Pure<RT, S, Unit>(unit);
                    }
                    else
                    {
                        var nstate = state;
                        state = Initial;
                        return yield<RT, S>(nstate);
                    }
                });
        }
         
        /// <summary>
        /// Folds values coming down-stream, when the predicate returns true the folded value is yielded 
        /// </summary>
        /// <param name="Initial">Initial state</param>
        /// <param name="Fold">Fold operation</param>
        /// <param name="UntilValue">Predicate</param>
        /// <returns>A pipe that folds</returns>
        public static Producer<RT, S, Unit> FoldUntil<RT, S, A>(this Producer<RT, S, A> ma, S Initial, Func<S, A, S> Fold, Func<S, bool> UntilState)
            where RT : struct, HasCancel<RT>
        {
            var state = Initial;
            return ma.Bind(
                x =>
                {
                    state = Fold(state, x);
                    if (UntilState(state))
                    {
                        var nstate = state;
                        state = Initial;
                        return yield<RT, S>(nstate);
                    }
                    else
                    {
                        return Pure<RT, S, Unit>(unit);
                    }
                });
        }
 
        /// <summary>
        /// Folds values coming down-stream, when the predicate returns true the folded value is yielded 
        /// </summary>
        /// <param name="Initial">Initial state</param>
        /// <param name="Fold">Fold operation</param>
        /// <param name="UntilValue">Predicate</param>
        /// <returns>A pipe that folds</returns>
        public static Producer<RT, S, Unit> FoldWhile<RT, S, A>(this Producer<RT, S, A> ma, S Initial, Func<S, A, S> Fold, Func<S, bool> WhileState)
            where RT : struct, HasCancel<RT>
        {
            var state = Initial;
            return ma.Bind(
                x =>
                {
                    state = Fold(state, x);
                    if (WhileState(state))
                    {
                        return Pure<RT, S, Unit>(unit);
                    }
                    else
                    {
                        var nstate = state;
                        state = Initial;
                        return yield<RT, S>(nstate);
                    }
                });
        }
        
        
        /// <summary>
        /// Merge a sequence of producers into a single producer
        /// </summary>
        /// <remarks>The merged producer completes when all component producers have completed</remarks>
        /// <param name="ms">Sequence of producers to merge</param>
        /// <returns>Merged producers</returns>
        public static Producer<RT, OUT, Unit> merge<RT, OUT>(Seq<Producer<RT, OUT, Unit>> ms) where RT : struct, HasCancel<RT>
        {
            var prod = from e in lift<RT, Fin<OUT>, RT>(runtime<RT>())
                       from x in yieldAll<RT, Fin<OUT>>(go(e))
                       select unit;
            
            var pipe = from fo in Pipe.awaiting<RT, Fin<OUT>, OUT>()
                       from nx in fo.Match(Succ: Pipe.yield<RT, Fin<OUT>, OUT>, 
                                           Fail: e => Pipe.lift<RT, Fin<OUT>, OUT, Unit>(Aff<RT, Unit>.Fail(e)))
                       select nx;

            return prod | pipe;
            
            async IAsyncEnumerable<Fin<OUT>> go(RT env)
            {
                var queue   = new ConcurrentQueue<OUT>();
                var wait    = new AutoResetEvent(true);
                var running = true;
                Error failed = null;
                
                // Create a new local runtime with its own cancellation token
                var lenv = env.LocalCancel;
                    
                // If the parent cancels, we should too
                using var reg = env.CancellationToken.Register(() => lenv.CancellationTokenSource.Cancel());

                // Posts a value to the queue and triggers the merged producer's yield
                Unit post(OUT x)
                {
                    queue.Enqueue(x);
                    wait.Set();
                    return default;
                }

                // Consumer that drains any Producer
                Consumer<RT, OUT, Unit> enqueue() =>
                    from _ in Consumer.awaiting<RT, OUT>().Select(post)
                    from r in enqueue()
                    select default(Unit);

                // Safe execution of an effect that captures and logs any errors
                // We trigger the end of the whole merge operation if any error occurs
                async Task<Fin<Unit>> run(Effect<RT, Unit> m)
                {
                    try
                    {
                        var r = await m.RunEffect().Run(lenv).AsTask().ConfigureAwait(false);
                        if (r.IsFail && !r.Error.Is(Errors.Cancelled)) 
                        {
                            // Bail on all if we get any error (other than a cancellation)
                            running = false;
                            failed = failed?.Append(r.Error) ?? r.Error;
                            lenv.CancellationTokenSource.Cancel();
                            wait.Set();
                        }
                        return r;
                    }
                    catch (Exception e)
                    {
                        running = false;
                        failed = failed?.Append(e) ?? e;
                        lenv.CancellationTokenSource.Cancel();
                        wait.Set();
                        return (Error)e;
                    }
                }
                
                // Compose the enqueue Consumer with the Producer to create an Effect that can be run 
                var mmt = ms.Map(m => m | enqueue()).Map(run).ToArray();

                // When all tasks are done, we're done
                // We should NOT be awaiting this 
                #pragma warning disable CS4014             
                Task.WhenAll(mmt)
                    .Iter(_ =>
                          {
                              running = false;
                              wait.Set();
                          });
                #pragma warning restore CS4014                

                do
                {
                    await wait.WaitOneAsync(lenv.CancellationToken).ConfigureAwait(false);
                    while (queue.TryDequeue(out var item))
                    {
                        yield return item;
                    }
                }
                // Keep processing until we're cancelled or all of the Producers have stopped producing
                while (running && !lenv.CancellationToken.IsCancellationRequested);

                if (failed != null)
                {
                    yield return failed;
                }
            }
        }
        
        /// <summary>
        /// Merge an array of queues into a single producer
        /// </summary>
        /// <remarks>The merged producer completes when all component queues have completed</remarks>
        /// <param name="ms">Sequence of queues to merge</param>
        /// <returns>Queues merged into a single producer</returns>
        public static Producer<RT, OUT, Unit> merge<RT, OUT>(params Queue<RT, OUT, Unit>[] ms) 
            where RT : struct, HasCancel<RT> =>
            merge(toSeq(ms.Map(m => (Producer<RT, OUT, Unit>)m)));
 
        /// <summary>
        /// Merge an array of producers into a single producer
        /// </summary>
        /// <remarks>The merged producer completes when all component producers have completed</remarks>
        /// <param name="ms">Sequence of producers to merge</param>
        /// <returns>Merged producers</returns>
        public static Producer<RT, OUT, Unit> merge<RT, OUT>(params Producer<RT, OUT, Unit>[] ms) 
            where RT : struct, HasCancel<RT> =>
            merge(toSeq(ms));
 
        /// <summary>
        /// Merge an array of producers into a single producer
        /// </summary>
        /// <remarks>The merged producer completes when all component producers have completed</remarks>
        /// <param name="ms">Sequence of producers to merge</param>
        /// <returns>Merged producers</returns>
        public static Producer<RT, OUT, Unit> merge<RT, OUT>(params Proxy<RT, Void, Unit, Unit, OUT, Unit>[] ms) 
            where RT : struct, HasCancel<RT> =>
            merge(toSeq(ms).Map(m => m.ToProducer()));
 
        /// <summary>
        /// Merge a sequence of queues into a single producer
        /// </summary>
        /// <remarks>The merged producer completes when all component queues have completed</remarks>
        /// <param name="ms">Sequence of queues to merge</param>
        /// <returns>Queues merged into a single producer</returns>
        public static Producer<RT, OUT, Unit> merge<RT, OUT>(Seq<Queue<RT, OUT, Unit>> ms) 
            where RT : struct, HasCancel<RT> =>
            merge(ms.Map(m => (Producer<RT, OUT, Unit>)m));
 
        /// <summary>
        /// Merge a sequence of producers into a single producer
        /// </summary>
        /// <remarks>The merged producer completes when all component producers have completed</remarks>
        /// <param name="ms">Sequence of producers to merge</param>
        /// <returns>Merged producers</returns>
        public static Producer<RT, OUT, Unit> merge<RT, OUT>(Seq<Proxy<RT, Void, Unit, Unit, OUT, Unit>> ms) 
            where RT : struct, HasCancel<RT> =>
            merge(ms.Map(m => m.ToProducer()));
    }
}
