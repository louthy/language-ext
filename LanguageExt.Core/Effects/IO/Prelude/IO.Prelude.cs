using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt.Common;
using LanguageExt.DSL;
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
        IO.lift<Unit>(
            e =>
            {
                e.Source.Cancel();
                throw new OperationCanceledException();
            });
    
    /// <summary>
    /// Always yields a `Unit` value
    /// </summary>
    public static readonly IO<Unit> unitIO = 
        IO.pure<Unit>(default);
    
    /// <summary>
    /// Yields the IO environment
    /// </summary>
    public static readonly IO<EnvIO> envIO = 
        IO.lift<EnvIO>(e => e);

    /// <summary>
    /// Tail call 
    /// </summary>
    /// <param name="tailIO"></param>
    /// <typeparam name="A"></typeparam>
    /// <returns></returns>
    public static IO<A> tail<A>(IO<A> tailIO) =>
        new IOTail<A>(tailIO);

    /// <summary>
    /// Wraps this computation in a local-environment that ignores any cancellation-token cancellation requests.
    /// </summary>
    /// <returns>An uninterruptible computation</returns>
    public static IO<A> uninterruptible<A>(IO<A> ma) =>
        ma.Uninterruptible();

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
    /// Awaits all 
    /// </summary>
    /// <param name="forks">IO operations to await</param>
    /// <returns>Sequence of results</returns>
    public static IO<Seq<A>> awaitAll<A>(params ForkIO<A>[] forks) =>
        awaitAll(forks.ToSeqUnsafe());

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
    /// Awaits all 
    /// </summary>
    /// <param name="forks">IO operations to await</param>
    /// <returns>Sequence of results</returns>
    public static IO<Seq<A>> awaitAll<A>(Seq<IO<ForkIO<A>>> forks) =>
        IO.liftAsync(async eio =>
                     {
                         var forks1 = forks.Map(mf => mf.Run(eio));
                         var result = await Task.WhenAll(forks1.Map(f => f.Await.RunAsync(eio).AsTask()));
                         return result.ToSeqUnsafe();
                     });

    /// <summary>
    /// Awaits all 
    /// </summary>
    /// <param name="forks">IO operations to await</param>
    /// <returns>Sequence of results</returns>
    public static IO<Seq<A>> awaitAll<A>(Seq<ForkIO<A>> forks) =>
        IO.liftAsync(async eio =>
                     {
                         var result = await Task.WhenAll(forks.Map(f => f.Await.RunAsync(eio).AsTask()));
                         return result.ToSeqUnsafe();
                     });

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
    public static IO<A> awaitAny<A>(params ForkIO<A>[] forks) =>
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
                         if(eio.Token.IsCancellationRequested) throw new OperationCanceledException();
                         using var lenv = eio.LocalCancel;
                         var result = await await Task.WhenAny(ms.Map(io => io.RunAsync(lenv).AsTask()));
                         await lenv.Source.CancelAsync();
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
    public static IO<A> awaitAny<A>(Seq<ForkIO<A>> forks) =>
        IO.liftAsync(async eio =>
                     {
                         if(eio.Token.IsCancellationRequested) throw new OperationCanceledException();
                         using var lenv = eio.LocalCancel;
                         var result = await await Task.WhenAny(forks.Map(f => f.Await.RunAsync(eio).AsTask()));
                         await lenv.Source.CancelAsync();
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
    public static IO<A> awaitAny<A>(Seq<IO<ForkIO<A>>> forks) =>
        IO.liftAsync(async eio =>
                     {
                         if(eio.Token.IsCancellationRequested) throw new OperationCanceledException();
                         using var lenv   = eio.LocalCancel;
                         var       forks1 = forks.Map(mf => mf.Run(lenv));
                         var       result = await await Task.WhenAny(forks1.Map(f => f.Await.RunAsync(eio).AsTask()));
                         await lenv.Source.CancelAsync();
                         return result;
                     });
    
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
    public static IO<A> repeat<A>(IO<A> ma) =>
        ma.Repeat();

    /// <summary>
    /// Keeps repeating the computation until the scheduler expires  
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
    public static IO<A> repeatWhile<A>(IO<A> ma, Func<A, bool> predicate) => 
        ma.As().RepeatWhile(predicate);

    /// <summary>
    /// Keeps repeating the computation until the scheduler expires, or the predicate returns false
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
    public static IO<A> repeatUntil<A>(
        K<IO, A> ma,
        Func<A, bool> predicate) =>
        ma.As().RepeatUntil(predicate);

    /// <summary>
    /// Keeps repeating the computation until the scheduler expires, or the predicate returns true
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
    public static IO<A> retry<A>(K<IO, A> ma) => 
        ma.As().Retry();

    /// <summary>
    /// Keeps retrying the computation until the scheduler expires  
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
    public static IO<A> retryWhile<A>(
        K<IO, A> ma,
        Func<Error, bool> predicate) => 
        ma.As().RetryWhile(predicate);

    /// <summary>
    /// Keeps retrying the computation until the scheduler expires, or the predicate returns false
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
    public static IO<A> retryUntil<A>(
        K<IO, A> ma,
        Func<Error, bool> predicate) => 
        ma.As().RetryUntil(predicate);

    /// <summary>
    /// Keeps retrying the computation until the scheduler expires, or the predicate returns true
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
