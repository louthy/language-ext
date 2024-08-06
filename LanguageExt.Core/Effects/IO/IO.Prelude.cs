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
    public static readonly IO<Unit> cancelIO = 
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
    /// Make this IO computation run on the `SynchronizationContext` that was captured at the start
    /// of the IO chain (i.e. the one embedded within the `EnvIO` environment that is passed through
    /// all IO computations)
    /// </summary>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static K<M, A> postIO<M, A>(K<M, A> ma)
        where M : Monad<M> =>
        ma.Post();        

    /// <summary>
    /// Queue this IO operation to run on the thread-pool. 
    /// </summary>
    /// <param name="timeout">Maximum time that the forked IO operation can run for. `None` for no timeout.</param>
    /// <returns>Returns a `ForkIO` data-structure that contains two IO effects that can be used to either cancel
    /// the forked IO operation or to await the result of it.
    /// </returns>
    [Pure]
    [MethodImpl(Opt.Default)]
    public static K<M, ForkIO<A>> forkIO<M, A>(K<M, A> ma, Option<TimeSpan> timeout = default)
        where M : Monad<M> =>
        ma.ForkIO(timeout);
    
    
    /// <summary>
    /// Yield the thread for the specified milliseconds or until cancelled.
    /// </summary>
    /// <param name="milliseconds">Amount of time to yield for</param>
    /// <returns>Unit</returns>
    public static IO<Unit> yieldIO(double milliseconds) =>
        IO.yield(milliseconds);

    /// <summary>
    /// Awaits all forks
    /// </summary>
    /// <param name="forks">Forks to await</param>
    /// <returns>Sequence of results</returns>
    public static K<M, Seq<A>> awaitAll<M, A>(params K<M, ForkIO<A>>[] forks)
        where M : Monad<M> =>
        awaitAll(forks.ToSeqUnsafe());

    /// <summary>
    /// Awaits all forks
    /// </summary>
    /// <param name="forks">Forks to await</param>
    /// <returns>Sequence of results</returns>
    public static K<M, Seq<A>> awaitAll<M, A>(Seq<K<M, ForkIO<A>>> forks)
        where M : Monad<M> =>
        forks.TraverseM(f => f.Await());

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
    public static K<M, A> awaitAny<M, A>(Seq<K<M, ForkIO<A>>> forks)
        where M : Monad<M> =>
        forks.Traverse(f => f.ToIO())
             .Bind(awaitAny);
    
    /// <summary>
    /// Timeout operation if it takes too long
    /// </summary>
    [Pure]
    [MethodImpl(Opt.Default)]
    public static K<M, A> timeoutIO<M, A>(TimeSpan timeout, K<M, A> ma) 
        where M : Monad<M> =>
         ma.TimeoutIO(timeout);
    
    /// <summary>
    /// Keeps repeating the computation   
    /// </summary>
    /// <param name="ma">Computation to repeat</param>
    /// <typeparam name="A">Computation bound value type</typeparam>
    /// <returns>The result of the last invocation of `ma`</returns>
    public static K<M, A> repeatIO<M, A>(K<M, A> ma)
        where M : Monad<M> =>
        ma.RepeatIO();

    /// <summary>
    /// Keeps repeating the computation, until the scheduler expires  
    /// </summary>
    /// <param name="schedule">Scheduler strategy for repeating</param>
    /// <param name="ma">Computation to repeat</param>
    /// <typeparam name="A">Computation bound value type</typeparam>
    /// <returns>The result of the last invocation of `ma`</returns>
    public static K<M, A> repeatIO<M, A>(Schedule schedule, K<M, A> ma)
        where M : Monad<M> =>
        ma.RepeatIO(schedule);

    /// <summary>
    /// Keeps repeating the computation until the predicate returns false
    /// </summary>
    /// <param name="ma">Computation to repeat</param>
    /// <typeparam name="A">Computation bound value type</typeparam>
    /// <returns>The result of the last invocation of `ma`</returns>
    public static K<M, A> repeatWhileIO<M, A>(K<M, A> ma, Func<A, bool> predicate) 
        where M : Monad<M> =>
        ma.RepeatWhileIO(predicate);

    /// <summary>
    /// Keeps repeating the computation, until the scheduler expires, or the predicate returns false
    /// </summary>
    /// <param name="schedule">Scheduler strategy for repeating</param>
    /// <param name="ma">Computation to repeat</param>
    /// <typeparam name="A">Computation bound value type</typeparam>
    /// <returns>The result of the last invocation of `ma`</returns>
    public static K<M, A> repeatWhileIO<M, A>(
        Schedule schedule,
        K<M, A> ma,
        Func<A, bool> predicate) 
        where M : Monad<M> =>
        ma.RepeatWhileIO(schedule, predicate);

    /// <summary>
    /// Keeps repeating the computation until the predicate returns true
    /// </summary>
    /// <param name="ma">Computation to repeat</param>
    /// <typeparam name="A">Computation bound value type</typeparam>
    /// <returns>The result of the last invocation of `ma`</returns>
    public static K<M, A> repeatUntilIO<M, A>(
        K<M, A> ma,
        Func<A, bool> predicate)
        where M : Monad<M> =>
        ma.RepeatUntilIO(predicate);

    /// <summary>
    /// Keeps repeating the computation, until the scheduler expires, or the predicate returns true
    /// </summary>
    /// <param name="schedule">Scheduler strategy for repeating</param>
    /// <param name="ma">Computation to repeat</param>
    /// <typeparam name="A">Computation bound value type</typeparam>
    /// <returns>The result of the last invocation of `ma`</returns>
    public static K<M, A> repeatUntilIO<M, A>(
        Schedule schedule,
        K<M, A> ma,
        Func<A, bool> predicate) 
        where M : Monad<M> =>
        ma.RepeatUntilIO(schedule, predicate);

    /// <summary>
    /// Keeps retrying the computation   
    /// </summary>
    /// <param name="ma">Computation to retry</param>
    /// <typeparam name="A">Computation bound value type</typeparam>
    /// <returns>The result of the last invocation of ma</returns>
    public static K<M, A> retryIO<M, A>(K<M, A> ma) 
        where M : Monad<M> =>
        ma.RetryIO();

    /// <summary>
    /// Keeps retrying the computation, until the scheduler expires  
    /// </summary>
    /// <param name="schedule">Scheduler strategy for retrying</param>
    /// <param name="ma">Computation to retry</param>
    /// <typeparam name="A">Computation bound value type</typeparam>
    /// <returns>The result of the last invocation of ma</returns>
    public static K<M, A> retryIO<M, A>(Schedule schedule, K<M, A> ma) 
        where M : Monad<M> =>
        ma.RetryIO(schedule);

    /// <summary>
    /// Keeps retrying the computation until the predicate returns false
    /// </summary>
    /// <param name="ma">Computation to retry</param>
    /// <typeparam name="A">Computation bound value type</typeparam>
    /// <returns>The result of the last invocation of ma</returns>
    public static K<M, A> retryWhileIO<M, A>(
        K<M, A> ma,
        Func<Error, bool> predicate) 
        where M : Monad<M> =>
        ma.RetryWhileIO(predicate);

    /// <summary>
    /// Keeps retrying the computation, until the scheduler expires, or the predicate returns false
    /// </summary>
    /// <param name="schedule">Scheduler strategy for retrying</param>
    /// <param name="ma">Computation to retry</param>
    /// <typeparam name="A">Computation bound value type</typeparam>
    /// <returns>The result of the last invocation of ma</returns>
    public static K<M, A> retryWhileIO<M, A>(
        Schedule schedule,
        K<M, A> ma,
        Func<Error, bool> predicate) 
        where M : Monad<M> =>
        ma.RetryWhileIO(schedule, predicate);

    /// <summary>
    /// Keeps retrying the computation until the predicate returns true
    /// </summary>
    /// <param name="ma">Computation to retry</param>
    /// <typeparam name="A">Computation bound value type</typeparam>
    /// <returns>The result of the last invocation of ma</returns>
    public static K<M, A> retryUntilIO<M, A>(
        K<M, A> ma,
        Func<Error, bool> predicate) 
        where M : Monad<M> =>
        ma.RetryUntilIO(predicate);

    /// <summary>
    /// Keeps retrying the computation, until the scheduler expires, or the predicate returns true
    /// </summary>
    /// <param name="schedule">Scheduler strategy for retrying</param>
    /// <param name="ma">Computation to retry</param>
    /// <typeparam name="A">Computation bound value type</typeparam>
    /// <returns>The result of the last invocation of ma</returns>
    public static K<M, A> retryUntilIO<M, A>(
        Schedule schedule,
        K<M, A> ma,
        Func<Error, bool> predicate) 
        where M : Monad<M> =>
        ma.RetryUntilIO(schedule, predicate);

    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Zipping
    //

    /// <summary>
    /// Takes two IO monads and zips their result
    /// </summary>
    /// <remarks>
    /// Asynchronous operations will run concurrently
    /// </remarks>
    /// <param name="tuple">Tuple of IO monads to run</param>
    /// <typeparam name="M">Monad trait type</typeparam>
    /// <typeparam name="A">First IO monad bound value type</typeparam>
    /// <typeparam name="B">Second IO monad bound value type</typeparam>
    /// <returns>IO monad</returns>
    public static K<M, (A First, B Second)> zipIO<M, A, B>(
        (K<M, A> First, K<M, B> Second) tuple)
        where M : Monad<M> =>
        from e1 in tuple.First.ForkIO()
        from e2 in tuple.Second.ForkIO()
        from r1 in e1.Await
        from r2 in e2.Await
        select (r1, r2);

    /// <summary>
    /// Takes two IO monads and zips their result
    /// </summary>
    /// <remarks>
    /// Asynchronous operations will run concurrently
    /// </remarks>
    /// <param name="tuple">Tuple of IO monads to run</param>
    /// <typeparam name="M">Monad trait type</typeparam>
    /// <typeparam name="A">First IO monad bound value type</typeparam>
    /// <typeparam name="B">Second IO monad bound value type</typeparam>
    /// <typeparam name="C">Third IO monad bound value type</typeparam>
    /// <returns>IO monad</returns>
    public static K<M, (A First, B Second, C Third)> zipIO<M, A, B, C>(
        (K<M, A> First, K<M, B> Second, K<M, C> Third) tuple)
        where M : Monad<M> =>
        from e1 in tuple.First.ForkIO()
        from e2 in tuple.Second.ForkIO()
        from e3 in tuple.Third.ForkIO()
        from r1 in e1.Await
        from r2 in e2.Await
        from r3 in e3.Await
        select (r1, r2, r3);

    /// <summary>
    /// Takes two IO monads and zips their result
    /// </summary>
    /// <remarks>
    /// Asynchronous operations will run concurrently
    /// </remarks>
    /// <param name="tuple">Tuple of IO monads to run</param>
    /// <typeparam name="M">Monad trait type</typeparam>
    /// <typeparam name="A">First IO monad bound value type</typeparam>
    /// <typeparam name="B">Second IO monad bound value type</typeparam>
    /// <typeparam name="C">Third IO monad bound value type</typeparam>
    /// <typeparam name="D">Fourth IO monad bound value type</typeparam>
    /// <returns>IO monad</returns>
    public static K<M, (A First, B Second, C Third, D Fourth)> zipIO<M, A, B, C, D>(
        (K<M, A> First, K<M, B> Second, K<M, C> Third, K<M, D> Fourth) tuple) 
        where M : Monad<M> =>
        from e1 in tuple.First.ForkIO()
        from e2 in tuple.Second.ForkIO()
        from e3 in tuple.Third.ForkIO()
        from e4 in tuple.Fourth.ForkIO()
        from r1 in e1.Await
        from r2 in e2.Await
        from r3 in e3.Await
        from r4 in e4.Await
        select (r1, r2, r3, r4);
    /// <summary>
    /// Takes two IO monads and zips their result
    /// </summary>
    /// <remarks>
    /// Asynchronous operations will run concurrently
    /// </remarks>
    /// <typeparam name="M">Monad trait type</typeparam>
    /// <typeparam name="A">First IO monad bound value type</typeparam>
    /// <typeparam name="B">Second IO monad bound value type</typeparam>
    /// <returns>IO monad</returns>
    public static K<M, (A First, B Second)> zipIO<M, A, B>(
        K<M, A> First, K<M, B> Second) 
        where M : Monad<M> =>
        (First, Second).ZipIO();

    /// <summary>
    /// Takes two IO monads and zips their result
    /// </summary>
    /// <remarks>
    /// Asynchronous operations will run concurrently
    /// </remarks>
    /// <typeparam name="M">Monad trait type</typeparam>
    /// <typeparam name="A">First IO monad bound value type</typeparam>
    /// <typeparam name="B">Second IO monad bound value type</typeparam>
    /// <typeparam name="C">Third IO monad bound value type</typeparam>
    /// <returns>IO monad</returns>
    public static K<M, (A First, B Second, C Third)> zipIO<M, A, B, C>(
        K<M, A> First, K<M, B> Second, K<M, C> Third) 
        where M : Monad<M> =>
        (First, Second, Third).ZipIO();
    
    /// <summary>
    /// Takes two IO monads and zips their result
    /// </summary>
    /// <remarks>
    /// Asynchronous operations will run concurrently
    /// </remarks>
    /// <typeparam name="M">Monad trait type</typeparam>
    /// <typeparam name="A">First IO monad bound value type</typeparam>
    /// <typeparam name="B">Second IO monad bound value type</typeparam>
    /// <typeparam name="C">Third IO monad bound value type</typeparam>
    /// <typeparam name="D">Fourth IO monad bound value type</typeparam>
    /// <returns>IO monad</returns>
    public static K<M, (A First, B Second, C Third, D Fourth)> zipIO<M, A, B, C, D>(
        K<M, A> First, K<M, B> Second, K<M, C> Third, K<M, D> Fourth) 
        where M : Monad<M> =>
        (First, Second, Third, Fourth).ZipIO();    

    static IO<A> awaitAny<A>(Seq<IO<ForkIO<A>>> mfs) =>
        new (eio =>
             {
                 using var wait = new AutoResetEvent(false);

                 var count     = mfs.Count;
                 var resultSet = false;
                 A?  result    = default;
                 var errors    = Error.Empty;
                 
                 // Run the IO<ForkIO> to get the ForkIO structures, this should happen immediately.
                 var forks = mfs.Map(mf => mf.Run(eio));
                 
                 // Applicative traverse await all the forks. For each completion or failure, log the result.
                 forks.Traverse(f => f.Await.Match(Succ: onComplete, Fail: onError))
                      .ForkIO()
                      .Run(eio)
                      .Ignore();
                 
                 // If we get one success, then we'll return straight away and cancel the others
                 // If we get any errors, we'll collect them in the hope that at least one works
                 // We wake up every 20ms to see if we've been cancelled
                 while (!resultSet && errors.Count < count)
                 {
                     wait.WaitOne(20);
                     if (eio.Token.IsCancellationRequested) throw new TaskCanceledException();
                 }
                 
                 return errors.IsEmpty && resultSet
                            ? result!
                            : errors.Throw<A>();

                 A onError(Error error)
                 {
                     errors += error;
                     wait.Set();
                     return error.Throw<A>();
                 }

                 A onComplete(A value)
                 {
                     result = value;
                     resultSet = true;
                     wait.Set();
                     try
                     {
                         // Try to cancel all the forks - ignore any errors thrown
                         forks.Traverse(f => f.Cancel).Run(eio).Ignore();
                     }
                     catch
                     {
                         // Ignore
                     }

                     return value;
                 }
             });
}
