#pragma warning disable LX_StreamT

using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt.Common;
using LanguageExt.Traits;
using LanguageExt.UnsafeValueAccess;

namespace LanguageExt;

public static partial class Prelude
{
    /// <summary>
    /// Access the cancellation-token from the IO environment
    /// </summary>
    /// <returns>CancellationToken</returns>
    public static readonly IO<CancellationToken> cancelToken =
        IO.lift(e => e.Token);

    /// <summary>
    /// Request a cancellation of the IO expression
    /// </summary>
    public static readonly IO<Unit> cancel =
        IO<Unit>.Lift(
            e =>
            {
                e.Source.Cancel();
                throw new TaskCanceledException();
            });
    
    /// <summary>
    /// Always yields a `Unit` value
    /// </summary>
    public static readonly IO<Unit> unitIO = 
        IO<Unit>.Pure(default);
    
    /// <summary>
    /// Yields the IO environment
    /// </summary>
    public static readonly IO<EnvIO> envIO = 
        IO<EnvIO>.Lift(e => e);

    /// <summary>
    /// Tail call 
    /// </summary>
    /// <param name="tailIO"></param>
    /// <typeparam name="A"></typeparam>
    /// <returns></returns>
    public static IO<A> tail<A>(IO<A> tailIO) =>
        new IOTail<A>(tailIO);
    
    /// <summary>
    /// Make this IO computation run on the `SynchronizationContext` that was captured at the start
    /// of the IO chain (i.e. the one embedded within the `EnvIO` environment that is passed through
    /// all IO computations)
    /// </summary>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static K<M, A> postIO<M, A>(K<M, A> ma)
        where M : Monad<M> =>
        ma.PostIO();        
    
    /// <summary>
    /// Make this IO computation run on the `SynchronizationContext` that was captured at the start
    /// of the IO chain (i.e. the one embedded within the `EnvIO` environment that is passed through
    /// all IO computations)
    /// </summary>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static K<IO, A> post<A>(K<IO, A> ma) =>
        ma.As().PostIO();        

    /// <summary>
    /// Queue this IO operation to run on the thread-pool. 
    /// </summary>
    /// <param name="timeout">Maximum time that the forked IO operation can run for. `None` for no timeout.</param>
    /// <returns>Returns a `ForkIO` data-structure that contains two IO effects that can be used to either cancel
    /// the forked IO operation or to await the result of it.
    /// </returns>
    [Pure]
    [MethodImpl(Opt.Default)]
    public static IO<ForkIO<A>> fork<A>(K<IO, A> ma, Option<TimeSpan> timeout = default) =>
        ma.ForkIO(timeout).As();

    /// <summary>
    /// Queue this IO operation to run on the thread-pool. 
    /// </summary>
    /// <param name="timeout">Maximum time that the forked IO operation can run for. `None` for no timeout.</param>
    /// <returns>Returns a `ForkIO` data-structure that contains two IO effects that can be used to either cancel
    /// the forked IO operation or to await the result of it.
    /// </returns>
    [Pure]
    [MethodImpl(Opt.Default)]
    public static K<M, ForkIO<A>> fork<M, A>(K<M, A> ma, Option<TimeSpan> timeout = default)
        where M : Monad<M> =>
        ma.ForkIO(timeout);

    /// <summary>
    /// Queue this IO operation to run on the thread-pool. 
    /// </summary>
    /// <param name="timeout">Maximum time that the forked IO operation can run for. `None` for no timeout.</param>
    /// <returns>Returns a `ForkIO` data-structure that contains two IO effects that can be used to either cancel
    /// the forked IO operation or to await the result of it.
    /// </returns>
    [Pure]
    [MethodImpl(Opt.Default)]
    public static K<M, ForkIO<Option<A>>> fork<M, A>(StreamT<M, A> ma, Option<TimeSpan> timeout = default)
        where M : Monad<M> =>
        ma.Run()
          .Map(oht => oht.Map(ht => ht.Item1))
          .ForkIO(timeout);

    /// <summary>
    /// Queue this IO operation to run on the thread-pool. 
    /// </summary>
    /// <param name="timeout">Maximum time that the forked IO operation can run for. `None` for no timeout.</param>
    /// <returns>Returns a `ForkIO` data-structure that contains two IO effects that can be used to either cancel
    /// the forked IO operation or to await the result of it.
    /// </returns>
    [Pure]
    [MethodImpl(Opt.Default)]
    public static IO<ForkIO<Option<A>>> fork<A>(StreamT<IO, A> ma, Option<TimeSpan> timeout = default) =>
        ma.Run()
          .Map(oht => oht.Map(ht => ht.Item1))
          .ForkIO(timeout)
          .As();

    /// <summary>
    /// Queue this IO operation to run on the thread-pool. 
    /// </summary>
    /// <param name="timeout">Maximum time that the forked IO operation can run for. `None` for no timeout.</param>
    /// <returns>Returns a `ForkIO` data-structure that contains two IO effects that can be used to either cancel
    /// the forked IO operation or to await the result of it.
    /// </returns>
    [Pure]
    [MethodImpl(Opt.Default)]
    public static K<M, A> awaitIO<M, A>(K<M, ForkIO<A>> ma)
        where M : Monad<M> =>
        ma.Await();

    /// <summary>
    /// Queue this IO operation to run on the thread-pool. 
    /// </summary>
    /// <param name="timeout">Maximum time that the forked IO operation can run for. `None` for no timeout.</param>
    /// <returns>Returns a `ForkIO` data-structure that contains two IO effects that can be used to either cancel
    /// the forked IO operation or to await the result of it.
    /// </returns>
    [Pure]
    [MethodImpl(Opt.Default)]
    public static IO<A> awaitIO<A>(K<IO, ForkIO<A>> ma) =>
        ma.Await().As();

    /// <summary>
    /// Yield the thread for the specified duration or until cancelled.
    /// </summary>
    /// <param name="duration">Amount of time to yield for</param>
    /// <returns>Unit</returns>
    [Pure]
    [MethodImpl(Opt.Default)]
    public static IO<Unit> yieldFor(Duration duration) =>
        IO.yieldFor(duration);

    /// <summary>
    /// Yield the thread for the specified duration or until cancelled.
    /// </summary>
    /// <param name="timeSpan">Amount of time to yield for</param>
    /// <returns>Unit</returns>
    [Pure]
    [MethodImpl(Opt.Default)]
    public static IO<Unit> yieldFor(TimeSpan timeSpan) =>
        IO.yieldFor(timeSpan);
    
    /// <summary>
    /// Awaits all operations
    /// </summary>
    /// <param name="ms">Operations to await</param>
    /// <returns>Sequence of results</returns>
    public static K<M, Seq<A>> awaitAll<M, A>(params K<M, A>[] ms)
        where M : Monad<M> =>
        awaitAll(ms.ToSeqUnsafe());
    
    /// <summary>
    /// Awaits all operations
    /// </summary>
    /// <param name="ms">Operations to await</param>
    /// <returns>Sequence of results</returns>
    public static IO<Seq<A>> awaitAll<A>(params K<IO, A>[] ms) =>
        awaitAll(ms.ToSeqUnsafe());
    
    /// <summary>
    /// Awaits all operations
    /// </summary>
    /// <param name="ms">Operations to await</param>
    /// <returns>Sequence of results</returns>
    public static IO<Seq<A>> awaitAll<A>(params IO<A>[] ms) =>
        awaitAll(ms.ToSeqUnsafe());
    
    /// <summary>
    /// Awaits all forks
    /// </summary>
    /// <param name="forks">Forks to await</param>
    /// <returns>Sequence of results</returns>
    public static K<M, Seq<A>> awaitAll<M, A>(params K<M, ForkIO<A>>[] forks)
        where M : Monad<M> =>
        awaitAll(forks.ToSeqUnsafe());

    /// <summary>
    /// Awaits all 
    /// </summary>
    /// <param name="ms">IO operations to await</param>
    /// <returns>Sequence of results</returns>
    public static IO<Seq<A>> awaitAll<A>(params IO<ForkIO<A>>[] mfs) =>
        awaitAll(mfs.ToSeqUnsafe());
    
    /// <summary>
    /// Awaits all operations
    /// </summary>
    /// <param name="ms">Operations to await</param>
    /// <returns>Sequence of results</returns>
    public static K<M, Seq<A>> awaitAll<M, A>(Seq<K<M, A>> ms)
        where M : Monad<M> =>
        ms.Traverse(f => f.ToIO())
          .Bind(awaitAll);
    
    /// <summary>
    /// Awaits all operations
    /// </summary>
    /// <param name="ms">Operations to await</param>
    /// <returns>Sequence of results</returns>
    public static IO<Seq<A>> awaitAll<A>(Seq<K<IO, A>> ms) =>
        awaitAll(ms.Map(f => f.As()));

    /// <summary>
    /// Awaits all operations
    /// </summary>
    /// <param name="ms">Operations to await</param>
    /// <returns>Sequence of results</returns>
    public static IO<Seq<A>> awaitAll<A>(Seq<IO<A>> ms) =>
        IO.liftAsync(async eio =>
                     {
                         var result = await Task.WhenAll(ms.Map(io => io.RunAsync(eio).AsTask()));
                         return result.ToSeqUnsafe();
                     });

    /// <summary>
    /// Awaits all forks
    /// </summary>
    /// <param name="forks">Forks to await</param>
    /// <returns>Sequence of results</returns>
    public static K<M, Seq<A>> awaitAll<M, A>(Seq<K<M, ForkIO<A>>> forks)
        where M : Monad<M> =>
        forks.TraverseM(f => f.Await());

    /// <summary>
    /// Awaits all 
    /// </summary>
    /// <param name="ms">IO operations to await</param>
    /// <returns>Sequence of results</returns>
    public static IO<Seq<A>> awaitAll<A>(Seq<IO<ForkIO<A>>> mfs) =>
        IO.liftAsync(async eio =>
                     {
                         var forks  = mfs.Map(mf => mf.Run(eio));
                         var result = await Task.WhenAll(forks.Map(f => f.Await.RunAsync(eio).AsTask()));
                         return result.ToSeqUnsafe();
                     });

    /// <summary>
    /// Awaits for any forks to complete
    /// </summary>
    /// <param name="forks">Forks to await</param>
    /// <returns>
    /// If we get one success, then we'll return straight away and cancel the others.
    /// If we get any errors, we'll collect them in the hope that at least one works.
    /// If we have collected as many errors as we have forks, then we'll return them all.
    /// </returns>
    public static K<M, A> awaitAny<M, A>(params K<M, A>[] forks)
        where M : Monad<M> =>
        awaitAny(forks.ToSeqUnsafe());

    /// <summary>
    /// Awaits for any IO to complete
    /// </summary>
    /// <param name="ms">IO operations to await</param>
    /// <returns>
    /// If we get one success, then we'll return straight away and cancel the others.
    /// If we get any errors, we'll collect them in the hope that at least one works.
    /// If we have collected as many errors as we have forks, then we'll return them all.
    /// </returns>
    public static IO<A> awaitAny<A>(params IO<A>[] ms) =>
        awaitAny(ms.ToSeqUnsafe());

    /// <summary>
    /// Awaits for any IO to complete
    /// </summary>
    /// <param name="ms">IO operations to await</param>
    /// <returns>
    /// If we get one success, then we'll return straight away and cancel the others.
    /// If we get any errors, we'll collect them in the hope that at least one works.
    /// If we have collected as many errors as we have forks, then we'll return them all.
    /// </returns>
    public static IO<A> awaitAny<A>(params K<IO, A>[] ms) =>
        awaitAny(ms.ToSeqUnsafe());

    /// <summary>
    /// Awaits for any forks to complete
    /// </summary>
    /// <param name="forks">Forks to await</param>
    /// <returns>
    /// If we get one success, then we'll return straight away and cancel the others.
    /// If we get any errors, we'll collect them in the hope that at least one works.
    /// If we have collected as many errors as we have forks, then we'll return them all.
    /// </returns>
    public static K<M, A> awaitAny<M, A>(params K<M, ForkIO<A>>[] forks)
        where M : Monad<M> =>
        awaitAny(forks.ToSeqUnsafe());

    /// <summary>
    /// Awaits for any forks to complete
    /// </summary>
    /// <param name="forks">Forks to await</param>
    /// <returns>
    /// If we get one success, then we'll return straight away and cancel the others.
    /// If we get any errors, we'll collect them in the hope that at least one works.
    /// If we have collected as many errors as we have forks, then we'll return them all.
    /// </returns>
    public static IO<A> awaitAny<A>(params IO<ForkIO<A>>[] forks) =>
        awaitAny(forks.ToSeqUnsafe());

    /// <summary>
    /// Awaits for any forks to complete
    /// </summary>
    /// <param name="forks">Forks to await</param>
    /// <returns>
    /// If we get one success, then we'll return straight away and cancel the others.
    /// If we get any errors, we'll collect them in the hope that at least one works.
    /// If we have collected as many errors as we have forks, then we'll return them all.
    /// </returns>
    public static K<M, A> awaitAny<M, A>(Seq<K<M, ForkIO<A>>> forks)
        where M : Monad<M> =>
        forks.Traverse(f => f.ToIO())
             .Bind(awaitAny);

    /// <summary>
    /// Awaits for operations to complete
    /// </summary>
    /// <param name="ms">Operations to await</param>
    /// <returns>
    /// If we get one success, then we'll return straight away and cancel the others.
    /// If we get any errors, we'll collect them in the hope that at least one works.
    /// If we have collected as many errors as we have forks, then we'll return them all.
    /// </returns>
    public static K<M, A> awaitAny<M, A>(Seq<K<M, A>> forks)
        where M : Monad<M> =>
        forks.Traverse(f => f.ToIO())
             .Bind(awaitAny);

    /// <summary>
    /// Awaits for any IO to complete
    /// </summary>
    /// <param name="ms">IO operations to await</param>
    /// <returns>
    /// If we get one success, then we'll return straight away and cancel the others.
    /// If we get any errors, we'll collect them in the hope that at least one works.
    /// If we have collected as many errors as we have forks, then we'll return them all.
    /// </returns>
    public static IO<A> awaitAny<A>(Seq<K<IO, A>> ms) =>
        awaitAny(ms.Map(m => m.As()));
    
    /// <summary>
    /// Awaits for any IO to complete
    /// </summary>
    /// <param name="ms">IO operations to await</param>
    /// <returns>
    /// If we get one success, then we'll return straight away and cancel the others.
    /// If we get any errors, we'll collect them in the hope that at least one works.
    /// If we have collected as many errors as we have forks, then we'll return them all.
    /// </returns>
    public static IO<A> awaitAny<A>(Seq<IO<A>> ms) =>
        IO.liftAsync(async eio => 
                     {
                         var result = await await Task.WhenAny(ms.Map(io => io.RunAsync(eio).AsTask()));
                         return result;
                     });

    /// <summary>
    /// Awaits for any forks to complete
    /// </summary>
    /// <param name="forks">Forks to await</param>
    /// <returns>
    /// If we get one success, then we'll return straight away and cancel the others.
    /// If we get any errors, we'll collect them in the hope that at least one works.
    /// If we have collected as many errors as we have forks, then we'll return them all.
    /// </returns>
    public static IO<A> awaitAny<A>(Seq<IO<ForkIO<A>>> mfs) =>
        IO.liftAsync(async eio =>
                     {
                         var forks  = mfs.Map(mf => mf.Run(eio));
                         var result = await await Task.WhenAny(forks.Map(f => f.Await.RunAsync(eio).AsTask()));
                         return result;
                     });

    /// <summary>
    /// Timeout operation if it takes too long
    /// </summary>
    [Pure]
    [MethodImpl(Opt.Default)]
    public static K<M, A> timeout<M, A>(TimeSpan timeout, K<M, A> ma)
        where M : Monad<M> =>
        ma.TimeoutIO(timeout);
    
    /// <summary>
    /// Timeout operation if it takes too long
    /// </summary>
    [Pure]
    [MethodImpl(Opt.Default)]
    public static IO<A> timeout<A>(TimeSpan timeout, K<IO, A> ma) => 
        ma.As().Timeout(timeout);
    
    /// <summary>
    /// Keeps repeating the computation   
    /// </summary>
    /// <param name="ma">Computation to repeat</param>
    /// <typeparam name="A">Computation bound value type</typeparam>
    /// <returns>The result of the last invocation of `ma`</returns>
    public static K<M, A> repeat<M, A>(K<M, A> ma)
        where M : Monad<M> =>
        ma.RepeatIO();
    
    /// <summary>
    /// Keeps repeating the computation   
    /// </summary>
    /// <param name="ma">Computation to repeat</param>
    /// <typeparam name="A">Computation bound value type</typeparam>
    /// <returns>The result of the last invocation of `ma`</returns>
    public static IO<A> repeat<A>(IO<A> ma) =>
        ma.Repeat();

    /// <summary>
    /// Keeps repeating the computation, until the scheduler expires  
    /// </summary>
    /// <param name="schedule">Scheduler strategy for repeating</param>
    /// <param name="ma">Computation to repeat</param>
    /// <typeparam name="A">Computation bound value type</typeparam>
    /// <returns>The result of the last invocation of `ma`</returns>
    public static K<M, A> repeat<M, A>(Schedule schedule, K<M, A> ma)
        where M : Monad<M> =>
        ma.RepeatIO(schedule);

    /// <summary>
    /// Keeps repeating the computation, until the scheduler expires  
    /// </summary>
    /// <param name="schedule">Scheduler strategy for repeating</param>
    /// <param name="ma">Computation to repeat</param>
    /// <typeparam name="A">Computation bound value type</typeparam>
    /// <returns>The result of the last invocation of `ma`</returns>
    public static IO<A> repeat<A>(Schedule schedule, K<IO, A> ma) =>
        ma.As().Repeat(schedule);

    /// <summary>
    /// Keeps repeating the computation until the predicate returns false
    /// </summary>
    /// <param name="ma">Computation to repeat</param>
    /// <typeparam name="A">Computation bound value type</typeparam>
    /// <returns>The result of the last invocation of `ma`</returns>
    public static K<M, A> repeatWhile<M, A>(K<M, A> ma, Func<A, bool> predicate) 
        where M : Monad<M> =>
        ma.RepeatWhileIO(predicate);

    /// <summary>
    /// Keeps repeating the computation until the predicate returns false
    /// </summary>
    /// <param name="ma">Computation to repeat</param>
    /// <typeparam name="A">Computation bound value type</typeparam>
    /// <returns>The result of the last invocation of `ma`</returns>
    public static IO<A> repeatWhile<A>(IO<A> ma, Func<A, bool> predicate) => 
        ma.As().RepeatWhile(predicate);

    /// <summary>
    /// Keeps repeating the computation, until the scheduler expires, or the predicate returns false
    /// </summary>
    /// <param name="schedule">Scheduler strategy for repeating</param>
    /// <param name="ma">Computation to repeat</param>
    /// <typeparam name="A">Computation bound value type</typeparam>
    /// <returns>The result of the last invocation of `ma`</returns>
    public static K<M, A> repeatWhile<M, A>(
        Schedule schedule,
        K<M, A> ma,
        Func<A, bool> predicate) 
        where M : Monad<M> =>
        ma.RepeatWhileIO(schedule, predicate);

    /// <summary>
    /// Keeps repeating the computation, until the scheduler expires, or the predicate returns false
    /// </summary>
    /// <param name="schedule">Scheduler strategy for repeating</param>
    /// <param name="ma">Computation to repeat</param>
    /// <typeparam name="A">Computation bound value type</typeparam>
    /// <returns>The result of the last invocation of `ma`</returns>
    public static IO<A> repeatWhile<A>(
        Schedule schedule,
        K<IO, A> ma,
        Func<A, bool> predicate) => 
        ma.As().RepeatWhile(schedule, predicate);

    /// <summary>
    /// Keeps repeating the computation until the predicate returns true
    /// </summary>
    /// <param name="ma">Computation to repeat</param>
    /// <typeparam name="A">Computation bound value type</typeparam>
    /// <returns>The result of the last invocation of `ma`</returns>
    public static K<M, A> repeatUntil<M, A>(
        K<M, A> ma,
        Func<A, bool> predicate)
        where M : Monad<M> =>
        ma.RepeatUntilIO(predicate);

    /// <summary>
    /// Keeps repeating the computation until the predicate returns true
    /// </summary>
    /// <param name="ma">Computation to repeat</param>
    /// <typeparam name="A">Computation bound value type</typeparam>
    /// <returns>The result of the last invocation of `ma`</returns>
    public static IO<A> repeatUntil<A>(
        K<IO, A> ma,
        Func<A, bool> predicate) =>
        ma.As().RepeatUntil(predicate);

    /// <summary>
    /// Keeps repeating the computation, until the scheduler expires, or the predicate returns true
    /// </summary>
    /// <param name="schedule">Scheduler strategy for repeating</param>
    /// <param name="ma">Computation to repeat</param>
    /// <typeparam name="A">Computation bound value type</typeparam>
    /// <returns>The result of the last invocation of `ma`</returns>
    public static K<M, A> repeatUntil<M, A>(
        Schedule schedule,
        K<M, A> ma,
        Func<A, bool> predicate) 
        where M : Monad<M> =>
        ma.RepeatUntilIO(schedule, predicate);

    /// <summary>
    /// Keeps repeating the computation, until the scheduler expires, or the predicate returns true
    /// </summary>
    /// <param name="schedule">Scheduler strategy for repeating</param>
    /// <param name="ma">Computation to repeat</param>
    /// <typeparam name="A">Computation bound value type</typeparam>
    /// <returns>The result of the last invocation of `ma`</returns>
    public static IO<A> repeatUntil<A>(
        Schedule schedule,
        K<IO, A> ma,
        Func<A, bool> predicate) => 
        ma.As().RepeatUntil(schedule, predicate);

    /// <summary>
    /// Keeps retrying the computation   
    /// </summary>
    /// <param name="ma">Computation to retry</param>
    /// <typeparam name="A">Computation bound value type</typeparam>
    /// <returns>The result of the last invocation of ma</returns>
    public static K<M, A> retry<M, A>(K<M, A> ma) 
        where M : Monad<M> =>
        ma.RetryIO();

    /// <summary>
    /// Keeps retrying the computation   
    /// </summary>
    /// <param name="ma">Computation to retry</param>
    /// <typeparam name="A">Computation bound value type</typeparam>
    /// <returns>The result of the last invocation of ma</returns>
    public static IO<A> retry<A>(K<IO, A> ma) => 
        ma.As().Retry();

    /// <summary>
    /// Keeps retrying the computation, until the scheduler expires  
    /// </summary>
    /// <param name="schedule">Scheduler strategy for retrying</param>
    /// <param name="ma">Computation to retry</param>
    /// <typeparam name="A">Computation bound value type</typeparam>
    /// <returns>The result of the last invocation of ma</returns>
    public static K<M, A> retry<M, A>(Schedule schedule, K<M, A> ma) 
        where M : Monad<M> =>
        ma.RetryIO(schedule);

    /// <summary>
    /// Keeps retrying the computation, until the scheduler expires  
    /// </summary>
    /// <param name="schedule">Scheduler strategy for retrying</param>
    /// <param name="ma">Computation to retry</param>
    /// <typeparam name="A">Computation bound value type</typeparam>
    /// <returns>The result of the last invocation of ma</returns>
    public static IO<A> retry<A>(Schedule schedule, K<IO, A> ma) => 
        ma.As().Retry(schedule);

    /// <summary>
    /// Keeps retrying the computation until the predicate returns false
    /// </summary>
    /// <param name="ma">Computation to retry</param>
    /// <typeparam name="A">Computation bound value type</typeparam>
    /// <returns>The result of the last invocation of ma</returns>
    public static K<M, A> retryWhile<M, A>(
        K<M, A> ma,
        Func<Error, bool> predicate) 
        where M : Monad<M> =>
        ma.RetryWhileIO(predicate);

    /// <summary>
    /// Keeps retrying the computation until the predicate returns false
    /// </summary>
    /// <param name="ma">Computation to retry</param>
    /// <typeparam name="A">Computation bound value type</typeparam>
    /// <returns>The result of the last invocation of ma</returns>
    public static IO<A> retryWhile<A>(
        K<IO, A> ma,
        Func<Error, bool> predicate) => 
        ma.As().RetryWhile(predicate);

    /// <summary>
    /// Keeps retrying the computation, until the scheduler expires, or the predicate returns false
    /// </summary>
    /// <param name="schedule">Scheduler strategy for retrying</param>
    /// <param name="ma">Computation to retry</param>
    /// <typeparam name="A">Computation bound value type</typeparam>
    /// <returns>The result of the last invocation of ma</returns>
    public static K<M, A> retryWhile<M, A>(
        Schedule schedule,
        K<M, A> ma,
        Func<Error, bool> predicate) 
        where M : Monad<M> =>
        ma.RetryWhileIO(schedule, predicate);

    /// <summary>
    /// Keeps retrying the computation, until the scheduler expires, or the predicate returns false
    /// </summary>
    /// <param name="schedule">Scheduler strategy for retrying</param>
    /// <param name="ma">Computation to retry</param>
    /// <typeparam name="A">Computation bound value type</typeparam>
    /// <returns>The result of the last invocation of ma</returns>
    public static IO<A> retryWhile<A>(
        Schedule schedule,
        K<IO, A> ma,
        Func<Error, bool> predicate) => 
        ma.As().RetryWhile(schedule, predicate);

    /// <summary>
    /// Keeps retrying the computation until the predicate returns true
    /// </summary>
    /// <param name="ma">Computation to retry</param>
    /// <typeparam name="A">Computation bound value type</typeparam>
    /// <returns>The result of the last invocation of ma</returns>
    public static K<M, A> retryUntil<M, A>(
        K<M, A> ma,
        Func<Error, bool> predicate) 
        where M : Monad<M> =>
        ma.RetryUntilIO(predicate);

    /// <summary>
    /// Keeps retrying the computation until the predicate returns true
    /// </summary>
    /// <param name="ma">Computation to retry</param>
    /// <typeparam name="A">Computation bound value type</typeparam>
    /// <returns>The result of the last invocation of ma</returns>
    public static IO<A> retryUntil<A>(
        K<IO, A> ma,
        Func<Error, bool> predicate) => 
        ma.As().RetryUntil(predicate);

    /// <summary>
    /// Keeps retrying the computation, until the scheduler expires, or the predicate returns true
    /// </summary>
    /// <param name="schedule">Scheduler strategy for retrying</param>
    /// <param name="ma">Computation to retry</param>
    /// <typeparam name="A">Computation bound value type</typeparam>
    /// <returns>The result of the last invocation of ma</returns>
    public static K<M, A> retryUntil<M, A>(
        Schedule schedule,
        K<M, A> ma,
        Func<Error, bool> predicate) 
        where M : Monad<M> =>
        ma.RetryUntilIO(schedule, predicate);

    /// <summary>
    /// Keeps retrying the computation, until the scheduler expires, or the predicate returns true
    /// </summary>
    /// <param name="schedule">Scheduler strategy for retrying</param>
    /// <param name="ma">Computation to retry</param>
    /// <typeparam name="A">Computation bound value type</typeparam>
    /// <returns>The result of the last invocation of ma</returns>
    public static IO<A> retryUntil<A>(
        Schedule schedule,
        K<IO, A> ma,
        Func<Error, bool> predicate) => 
        ma.As().RetryUntil(schedule, predicate);
   
}
