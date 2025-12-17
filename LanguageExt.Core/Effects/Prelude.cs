using System;
using LanguageExt.Common;
using LanguageExt.Traits;
using System.Diagnostics.Contracts;
using LanguageExt.UnsafeValueAccess;

namespace LanguageExt;

public static partial class Prelude
{
    /// <summary>
    /// Tail call 
    /// </summary>
    /// <param name="ma"></param>
    /// <typeparam name="A"></typeparam>
    /// <returns></returns>
    [Pure]
    public static K<M, A> tailIO<M, A>(K<M, A> ma) 
        where M : MonadUnliftIO<M> =>
        ma.MapIO(tail);
    
    /// <summary>
    /// Make this computation run on the `SynchronizationContext` that was captured at the start
    /// of the IO chain (i.e. the one embedded within the `EnvIO` environment that is passed through
    /// all IO computations)
    /// </summary>
    [Pure]
    public static K<M, A> postIO<M, A>(K<M, A> ma)
        where M : MonadUnliftIO<M> =>
        M.PostIO(ma);        

    /// <summary>
    /// Queue this IO operation to run on the thread-pool. 
    /// </summary>
    /// <param name="timeout">Maximum time that the forked IO operation can run for. `None` for no timeout.</param>
    /// <returns>Returns a `ForkIO` data-structure that contains two IO effects that can be used to either cancel
    /// the forked IO operation or to await the result of it.
    /// </returns>
    [Pure]
    public static K<M, B> mapIO<M, A, B>(K<M, A> ma, Func<IO<A>, IO<B>> f)
        where M : MonadUnliftIO<M>, Monad<M> =>
        M.MapIO(ma, f);    

    /// <summary>
    /// Wraps this computation in a local-environment that ignores any cancellation-token cancellation requests.
    /// </summary>
    /// <returns>An uninterruptible computation</returns>
    [Pure]
    public static K<M, A> uninterruptible<M, A>(K<M, A> ma)
        where M : MonadUnliftIO<M> =>
        M.UninterruptibleIO(ma);
    
    /// <summary>
    /// Creates a local cancellation environment
    /// </summary>
    /// <remarks>
    /// A local cancellation environment stops other IO computations, that rely on the same
    /// environmental cancellation token, from being taken down by a regional cancellation.
    ///
    /// If an `IO.cancel` is invoked locally, then it will still create an exception that
    /// propagates upwards and so catching cancellations is still important. 
    /// </remarks>
    /// <param name="ma">Computation to run within the local context</param>
    /// <typeparam name="A">Bound value</typeparam>
    /// <returns>Result of the computation</returns>
    [Pure]
    public static K<M, A> localIO<M, A>(K<M, A> ma) 
        where M : MonadUnliftIO<M> =>
        M.LocalIO(ma);

    /// <summary>
    /// Queue this IO operation to run on the thread-pool. 
    /// </summary>
    /// <param name="timeout">Maximum time that the forked IO operation can run for. `None` for no timeout.</param>
    /// <returns>Returns a `ForkIO` data-structure that contains two IO effects that can be used to either cancel
    /// the forked IO operation or to await the result of it.
    /// </returns>
    [Pure]
    public static K<M, ForkIO<A>> fork<M, A>(K<M, A> ma, Option<TimeSpan> timeout = default)
        where M : MonadUnliftIO<M>, Monad<M> =>
        M.ForkIO(ma, timeout);

    /// <summary>
    /// Queue this IO operation to run on the thread-pool. 
    /// </summary>
    /// <param name="timeout">Maximum time that the forked IO operation can run for. `None` for no timeout.</param>
    /// <returns>Returns a `ForkIO` data-structure that contains two IO effects that can be used to either cancel
    /// the forked IO operation or to await the result of it.
    /// </returns>
    [Pure]
    public static K<M, A> awaitIO<M, A>(K<M, ForkIO<A>> ma)
        where M : MonadUnliftIO<M> =>
        M.Await(ma);
    
    /// <summary>
    /// Awaits all operations
    /// </summary>
    /// <param name="ms">Operations to await</param>
    /// <returns>Sequence of results</returns>
    [Pure]
    public static K<M, Seq<A>> awaitAll<M, A>(params K<M, A>[] ms)
        where M : MonadUnliftIO<M> =>
        awaitAll(ms.ToSeqUnsafe());
    
    /// <summary>
    /// Awaits all forks
    /// </summary>
    /// <param name="forks">Forks to await</param>
    /// <returns>Sequence of results</returns>
    [Pure]
    public static K<M, Seq<A>> awaitAll<M, A>(params K<M, ForkIO<A>>[] forks)
        where M : MonadUnliftIO<M> =>
        awaitAll(forks.ToSeqUnsafe());
    
    /// <summary>
    /// Awaits all operations
    /// </summary>
    /// <param name="ms">Operations to await</param>
    /// <returns>Sequence of results</returns>
    [Pure]
    public static K<M, Seq<A>> awaitAll<M, A>(Seq<K<M, A>> ms)
        where M : MonadUnliftIO<M> =>
        ms.Traverse(f => f.ToIO())
          .Bind(awaitAll);
    
    /// <summary>
    /// Awaits all forks
    /// </summary>
    /// <param name="forks">Forks to await</param>
    /// <returns>Sequence of results</returns>
    [Pure]
    public static K<M, Seq<A>> awaitAll<M, A>(Seq<K<M, ForkIO<A>>> forks)
        where M : MonadUnliftIO<M> =>
        forks.TraverseM(f => f.Await());

    /// <summary>
    /// Awaits for any operation to complete
    /// </summary>
    /// <param name="ms">Operations to await</param>
    /// <returns>
    /// If we get one success, then we'll return straight away and cancel the others.
    /// If we get any errors, we'll collect them in the hope that at least one works.
    /// If we have collected as many errors as we have forks, then we'll return them all.
    /// </returns>
    [Pure]
    public static K<M, A> awaitAny<M, A>(params K<M, A>[] ms)
        where M : MonadUnliftIO<M> =>
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
    [Pure]
    public static K<M, A> awaitAny<M, A>(params K<M, ForkIO<A>>[] forks)
        where M : MonadUnliftIO<M> =>
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
    [Pure]
    public static K<M, A> awaitAny<M, A>(Seq<K<M, ForkIO<A>>> forks)
        where M : MonadUnliftIO<M> =>
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
    [Pure]
    public static K<M, A> awaitAny<M, A>(Seq<K<M, A>> forks)
        where M : MonadUnliftIO<M> =>
        forks.Traverse(f => f.ToIO())
             .Bind(awaitAny);

    /// <summary>
    /// Timeout operation if it takes too long
    /// </summary>
    [Pure]
    public static K<M, A> timeout<M, A>(TimeSpan timeout, K<M, A> ma)
        where M : MonadUnliftIO<M> =>
        ma.TimeoutIO(timeout);
    
    /// <summary>
    /// Keeps repeating the computation   
    /// </summary>
    /// <param name="ma">Computation to repeat</param>
    /// <typeparam name="A">Computation bound value type</typeparam>
    /// <returns>The result of the last invocation of `ma`</returns>
    [Pure]
    public static K<M, A> repeat<M, A>(K<M, A> ma)
        where M : MonadUnliftIO<M> =>
        ma.RepeatIO();

    /// <summary>
    /// Keeps repeating the computation, until the scheduler expires  
    /// </summary>
    /// <param name="schedule">Scheduler strategy for repeating</param>
    /// <param name="ma">Computation to repeat</param>
    /// <typeparam name="A">Computation bound value type</typeparam>
    /// <returns>The result of the last invocation of `ma`</returns>
    [Pure]
    public static K<M, A> repeat<M, A>(Schedule schedule, K<M, A> ma)
        where M : MonadUnliftIO<M> =>
        ma.RepeatIO(schedule);

    /// <summary>
    /// Keeps repeating the computation until the predicate returns false
    /// </summary>
    /// <param name="ma">Computation to repeat</param>
    /// <typeparam name="A">Computation bound value type</typeparam>
    /// <returns>The result of the last invocation of `ma`</returns>
    [Pure]
    public static K<M, A> repeatWhile<M, A>(K<M, A> ma, Func<A, bool> predicate) 
        where M : MonadUnliftIO<M> =>
        ma.RepeatWhileIO(predicate);

    /// <summary>
    /// Keeps repeating the computation until the scheduler expires, or the predicate returns false
    /// </summary>
    /// <param name="schedule">Scheduler strategy for repeating</param>
    /// <param name="ma">Computation to repeat</param>
    /// <typeparam name="A">Computation bound value type</typeparam>
    /// <returns>The result of the last invocation of `ma`</returns>
    [Pure]
    public static K<M, A> repeatWhile<M, A>(
        Schedule schedule,
        K<M, A> ma,
        Func<A, bool> predicate) 
        where M : MonadUnliftIO<M> =>
        ma.RepeatWhileIO(schedule, predicate);

    /// <summary>
    /// Keeps repeating the computation until the predicate returns true
    /// </summary>
    /// <param name="ma">Computation to repeat</param>
    /// <typeparam name="A">Computation bound value type</typeparam>
    /// <returns>The result of the last invocation of `ma`</returns>
    [Pure]
    public static K<M, A> repeatUntil<M, A>(
        K<M, A> ma,
        Func<A, bool> predicate)
        where M : MonadUnliftIO<M> =>
        ma.RepeatUntilIO(predicate);

    /// <summary>
    /// Keeps repeating the computation until the scheduler expires, or the predicate returns true
    /// </summary>
    /// <param name="schedule">Scheduler strategy for repeating</param>
    /// <param name="ma">Computation to repeat</param>
    /// <typeparam name="A">Computation bound value type</typeparam>
    /// <returns>The result of the last invocation of `ma`</returns>
    [Pure]
    public static K<M, A> repeatUntil<M, A>(
        Schedule schedule,
        K<M, A> ma,
        Func<A, bool> predicate) 
        where M : MonadUnliftIO<M> =>
        ma.RepeatUntilIO(schedule, predicate);

    /// <summary>
    /// Keeps retrying the computation   
    /// </summary>
    /// <param name="ma">Computation to retry</param>
    /// <typeparam name="A">Computation bound value type</typeparam>
    /// <returns>The result of the last invocation of ma</returns>
    [Pure]
    public static K<M, A> retry<M, A>(K<M, A> ma) 
        where M : MonadUnliftIO<M> =>
        ma.RetryIO();

    /// <summary>
    /// Keeps retrying the computation until the scheduler expires  
    /// </summary>
    /// <param name="schedule">Scheduler strategy for retrying</param>
    /// <param name="ma">Computation to retry</param>
    /// <typeparam name="A">Computation bound value type</typeparam>
    /// <returns>The result of the last invocation of ma</returns>
    [Pure]
    public static K<M, A> retry<M, A>(Schedule schedule, K<M, A> ma) 
        where M : MonadUnliftIO<M> =>
        ma.RetryIO(schedule);

    /// <summary>
    /// Keeps retrying the computation until the predicate returns false
    /// </summary>
    /// <param name="ma">Computation to retry</param>
    /// <typeparam name="A">Computation bound value type</typeparam>
    /// <returns>The result of the last invocation of ma</returns>
    [Pure]
    public static K<M, A> retryWhile<M, A>(
        K<M, A> ma,
        Func<Error, bool> predicate) 
        where M : MonadUnliftIO<M> =>
        ma.RetryWhileIO(predicate);

    /// <summary>
    /// Keeps retrying the computation until the scheduler expires, or the predicate returns false
    /// </summary>
    /// <param name="schedule">Scheduler strategy for retrying</param>
    /// <param name="ma">Computation to retry</param>
    /// <typeparam name="A">Computation bound value type</typeparam>
    /// <returns>The result of the last invocation of ma</returns>
    [Pure]
    public static K<M, A> retryWhile<M, A>(
        Schedule schedule,
        K<M, A> ma,
        Func<Error, bool> predicate) 
        where M : MonadUnliftIO<M> =>
        ma.RetryWhileIO(schedule, predicate);

    /// <summary>
    /// Keeps retrying the computation until the predicate returns true
    /// </summary>
    /// <param name="ma">Computation to retry</param>
    /// <typeparam name="A">Computation bound value type</typeparam>
    /// <returns>The result of the last invocation of ma</returns>
    [Pure]
    public static K<M, A> retryUntil<M, A>(
        K<M, A> ma,
        Func<Error, bool> predicate) 
        where M : MonadUnliftIO<M> =>
        ma.RetryUntilIO(predicate);

    /// <summary>
    /// Keeps retrying the computation until the scheduler expires, or the predicate returns true
    /// </summary>
    /// <param name="schedule">Scheduler strategy for retrying</param>
    /// <param name="ma">Computation to retry</param>
    /// <typeparam name="A">Computation bound value type</typeparam>
    /// <returns>The result of the last invocation of ma</returns>
    [Pure]
    public static K<M, A> retryUntil<M, A>(
        Schedule schedule,
        K<M, A> ma,
        Func<Error, bool> predicate) 
        where M : MonadUnliftIO<M> =>
        ma.RetryUntilIO(schedule, predicate);
}
