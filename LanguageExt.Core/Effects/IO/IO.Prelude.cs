using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt.Common;
using LanguageExt.Traits;

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
        new (e =>
             {
                 e.Source.Cancel();
                 throw new TaskCanceledException();
             });
    
    /// <summary>
    /// Always yields a `Unit` value
    /// </summary>
    public static readonly IO<Unit> unitIO = 
        IO<Unit>.pure(default);
    
    /// <summary>
    /// Yields the IO environment
    /// </summary>
    public static readonly IO<EnvIO> envIO = 
        IO<EnvIO>.Lift(e => e);
    
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
        ma.Fork(timeout);

    /// <summary>
    /// Timeout operation if it takes too long
    /// </summary>
    [Pure]
    [MethodImpl(Opt.Default)]
    public static K<M, A> timeout<M, A>(TimeSpan timeout, K<M, A> ma) 
        where M : Monad<M> =>
         ma.Timeout(timeout);
    
    /// <summary>
    /// Keeps repeating the computation   
    /// </summary>
    /// <param name="ma">Computation to repeat</param>
    /// <typeparam name="A">Computation bound value type</typeparam>
    /// <returns>The result of the last invocation of `ma`</returns>
    public static K<M, A> repeat<M, A>(K<M, A> ma)
        where M : Monad<M> =>
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
        ma.Repeat(schedule);

    /// <summary>
    /// Keeps repeating the computation until the predicate returns false
    /// </summary>
    /// <param name="ma">Computation to repeat</param>
    /// <typeparam name="A">Computation bound value type</typeparam>
    /// <returns>The result of the last invocation of `ma`</returns>
    public static K<M, A> repeatWhile<M, A>(K<M, A> ma, Func<A, bool> predicate) 
        where M : Monad<M> =>
        ma.RepeatWhile(predicate);

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
        ma.RepeatWhile(schedule, predicate);

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
        ma.RepeatUntil(predicate);

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
        ma.RepeatUntil(schedule, predicate);

    /// <summary>
    /// Keeps retrying the computation   
    /// </summary>
    /// <param name="ma">Computation to retry</param>
    /// <typeparam name="A">Computation bound value type</typeparam>
    /// <returns>The result of the last invocation of ma</returns>
    public static K<M, A> retry<M, A>(K<M, A> ma) 
        where M : Monad<M> =>
        ma.Retry();

    /// <summary>
    /// Keeps retrying the computation, until the scheduler expires  
    /// </summary>
    /// <param name="schedule">Scheduler strategy for retrying</param>
    /// <param name="ma">Computation to retry</param>
    /// <typeparam name="A">Computation bound value type</typeparam>
    /// <returns>The result of the last invocation of ma</returns>
    public static K<M, A> retry<M, A>(Schedule schedule, K<M, A> ma) 
        where M : Monad<M> =>
        ma.Retry(schedule);

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
        ma.RetryWhile(predicate);

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
        ma.RetryWhile(schedule, predicate);

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
        ma.RetryUntil(predicate);

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
        ma.RetryUntil(schedule, predicate);

    /// <summary>
    /// Yield the thread for the specified milliseconds or until cancelled.
    /// </summary>
    /// <param name="milliseconds">Amount of time to yield for</param>
    /// <returns>Unit</returns>
    public static IO<Unit> yield(double milliseconds) =>
        IO.yield(milliseconds);
}
