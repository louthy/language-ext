using System;
using System.Threading;
using LanguageExt.Common;
using LanguageExt.DSL;

namespace LanguageExt.Traits;

/// <summary>
/// Monad that is either the IO monad or a transformer with the IO monad in its stack
/// </summary>
/// <typeparam name="M">Self referring trait</typeparam>
public interface MonadIO<M> : Monad<M>
    where M : MonadIO<M>
{
    /// <summary>
    /// Creates a local cancellation environment
    /// </summary>
    /// <remarks>
    /// A local cancellation environment stops other IO computations, that rely on the same
    /// environmental cancellation token, from being taken down by a regional cancellation.
    ///
    /// If a `IO.cancel` is invoked locally then it will still create an exception that
    /// propagates upwards and so catching cancellations is still important. 
    /// </remarks>
    /// <param name="ma">Computation to run within the local context</param>
    /// <typeparam name="A">Bound value</typeparam>
    /// <returns>Result of the computation</returns>
    public static virtual K<M, A> LocalIO<A>(K<M, A> ma) =>
        ma.MapIO(io => io.Local());

    /// <summary>
    /// Make this IO computation run on the `SynchronizationContext` that was captured at the start
    /// of the IO chain (i.e. the one embedded within the `EnvIO` environment that is passed through
    /// all IO computations)
    /// </summary>
    public static virtual K<M, A> PostIO<A>(K<M, A> ma) =>
        ma.MapIO(io => io.Post());        

    /// <summary>
    /// Await a forked operation
    /// </summary>
    public static virtual K<M, A> Await<A>(K<M, ForkIO<A>> ma) =>
        ma.MapIO(io => io.Bind(f => f.Await));    

    /// <summary>
    /// Timeout operation if it takes too long
    /// </summary>
    public static virtual K<M, A> TimeoutIO<A>(K<M, A> ma, TimeSpan timeout) =>
        ma.MapIO(io => io.Timeout(timeout));

    /// <summary>
    /// The IO monad tracks resources automatically, this creates a local resource environment
    /// to run this computation in.  Once the computation has completed any resources acquired
    /// are automatically released.  Imagine this as the ultimate `using` statement.
    /// </summary>
    public static virtual K<M, A> BracketIO<A>(K<M, A> ma) =>
        ma.MapIO(io => io.Bracket());

    /// <summary>
    /// When acquiring, using, and releasing various resources, it can be quite convenient to write a function to manage
    /// the acquisition and releasing, taking a function of the acquired value that specifies an action to be performed
    /// in between.
    /// </summary>
    /// <param name="Acq">Resource acquisition</param>
    /// <param name="Use">Function to use the acquired resource</param>
    /// <param name="Fin">Function to invoke to release the resource</param>
    public static virtual K<M, C> BracketIO<A, B, C>(
        K<M, A> Acq,
        Func<A, IO<C>> Use,
        Func<A, IO<B>> Fin) =>
        Acq.MapIO(io => io.Bracket(Use, Fin));

    /// <summary>
    /// When acquiring, using, and releasing various resources, it can be quite convenient to write a function to manage
    /// the acquisition and releasing, taking a function of the acquired value that specifies an action to be performed
    /// in between.
    /// </summary>
    /// <param name="Acq">Resource acquisition</param>
    /// <param name="Use">Function to use the acquired resource</param>
    /// <param name="Catch">Function to run to handle any exceptions</param>
    /// <param name="Fin">Function to invoke to release the resource</param>
    public static virtual K<M, C> BracketIO<A, B, C>(
        K<M, A> Acq,
        Func<A, IO<C>> Use,
        Func<Error, IO<C>> Catch,
        Func<A, IO<B>> Fin) =>
        Acq.MapIO(io => io.Bracket(Use, Catch, Fin));

    /// <summary>
    /// Queue this IO operation to run on the thread-pool. 
    /// </summary>
    /// <param name="timeout">Maximum time that the forked IO operation can run for. `None` for no timeout.</param>
    /// <returns>Returns a `ForkIO` data-structure that contains two IO effects that can be used to either cancel
    /// the forked IO operation or to await the result of it.
    /// </returns>
    public static virtual K<M, ForkIO<A>> ForkIO<A>(K<M, A> ma, Option<TimeSpan> timeout = default) =>
        ma.MapIO(io => io.Fork(timeout));
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Repeating the effect
    //

    /// <summary>
    /// Keeps repeating the computation forever, or until an error occurs
    /// </summary>
    /// <remarks>
    /// Any resources acquired within a repeated IO computation will automatically be released.  This also means you can't
    /// acquire resources and return them from within a repeated computation.
    /// </remarks>
    /// <returns>The result of the last invocation</returns>
    public static virtual K<M, A> RepeatIO<A>(K<M, A> ma) =>
        ma.MapIO(io => io.Repeat());

    /// <summary>
    /// Keeps repeating the computation, until the scheduler expires, or an error occurs  
    /// </summary>
    /// <remarks>
    /// Any resources acquired within a repeated IO computation will automatically be released.  This also means you can't
    /// acquire resources and return them from within a repeated computation.
    /// </remarks>
    /// <param name="schedule">Scheduler strategy for repeating</param>
    /// <returns>The result of the last invocation</returns>
    public static virtual K<M, A> RepeatIO<A>(
        K<M, A> ma,
        Schedule schedule) =>
        ma.MapIO(io => io.Repeat(schedule));

    /// <summary>
    /// Keeps repeating the computation until the predicate returns false, or an error occurs 
    /// </summary>
    /// <remarks>
    /// Any resources acquired within a repeated IO computation will automatically be released.  This also means you can't
    /// acquire resources and return them from within a repeated computation.
    /// </remarks>
    /// <param name="predicate">Keep repeating while this predicate returns `true` for each computed value</param>
    /// <returns>The result of the last invocation</returns>
    public static virtual K<M, A> RepeatWhileIO<A>(
        K<M, A> ma,
        Func<A, bool> predicate) =>
        ma.MapIO(io => io.RepeatWhile(predicate));

    /// <summary>
    /// Keeps repeating the computation, until the scheduler expires, or the predicate returns false, or an error occurs
    /// </summary>
    /// <remarks>
    /// Any resources acquired within a repeated IO computation will automatically be released.  This also means you can't
    /// acquire resources and return them from within a repeated computation.
    /// </remarks>
    /// <param name="schedule">Scheduler strategy for repeating</param>
    /// <param name="predicate">Keep repeating while this predicate returns `true` for each computed value</param>
    /// <returns>The result of the last invocation</returns>
    public static virtual K<M, A> RepeatWhileIO<A>(
        K<M, A> ma,
        Schedule schedule,
        Func<A, bool> predicate) =>
        ma.MapIO(io => io.RepeatWhile(schedule, predicate));

    /// <summary>
    /// Keeps repeating the computation until the predicate returns true, or an error occurs
    /// </summary>
    /// <remarks>
    /// Any resources acquired within a repeated IO computation will automatically be released.  This also means you can't
    /// acquire resources and return them from within a repeated computation.
    /// </remarks>
    /// <param name="predicate">Keep repeating until this predicate returns `true` for each computed value</param>
    /// <returns>The result of the last invocation</returns>
    public static virtual K<M, A> RepeatUntilIO<A>(
        K<M, A> ma,
        Func<A, bool> predicate) =>
        ma.MapIO(io => io.RepeatUntil(predicate));

    /// <summary>
    /// Keeps repeating the computation, until the scheduler expires, or the predicate returns true, or an error occurs
    /// </summary>
    /// <remarks>
    /// Any resources acquired within a repeated IO computation will automatically be released.  This also means you can't
    /// acquire resources and return them from within a repeated computation.
    /// </remarks>
    /// <param name="schedule">Scheduler strategy for repeating</param>
    /// <param name="predicate">Keep repeating until this predicate returns `true` for each computed value</param>
    /// <returns>The result of the last invocation</returns>
    public static virtual K<M, A> RepeatUntilIO<A>(
        K<M, A> ma,
        Schedule schedule,
        Func<A, bool> predicate) =>
        ma.MapIO(io => io.RepeatUntil(schedule, predicate));

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Retrying the effect when it fails
    //

    /// <summary>
    /// Retry if the IO computation fails 
    /// </summary>
    /// <remarks>
    /// This variant will retry forever
    /// </remarks>
    /// <remarks>
    /// Any resources acquired within a retrying IO computation will automatically be released *if* the operation fails.
    /// So, successive retries will not grow the acquired resources on each retry iteration.  Any successful operation that
    /// acquires resources will have them tracked in the usual way. 
    /// </remarks>
    public static virtual K<M, A> RetryIO<A>(K<M, A> ma) =>
        ma.MapIO(io => io.Retry());

    /// <summary>
    /// Retry if the IO computation fails 
    /// </summary>
    /// <remarks>
    /// This variant will retry until the schedule expires
    /// </remarks>
    /// <remarks>
    /// Any resources acquired within a retrying IO computation will automatically be released *if* the operation fails.
    /// So, successive retries will not grow the acquired resources on each retry iteration.  Any successful operation that
    /// acquires resources will have them tracked in the usual way. 
    /// </remarks>
    public static virtual K<M, A> RetryIO<A>(
        K<M, A> ma,
        Schedule schedule) =>
        ma.MapIO(io => io.Retry(schedule));

    /// <summary>
    /// Retry if the IO computation fails 
    /// </summary>
    /// <remarks>
    /// This variant will keep retrying whilst the predicate returns `true` for the error generated at each iteration;
    /// at which point the last raised error will be thrown.
    /// </remarks>
    /// <remarks>
    /// Any resources acquired within a retrying IO computation will automatically be released *if* the operation fails.
    /// So, successive retries will not grow the acquired resources on each retry iteration.  Any successful operation that
    /// acquires resources will have them tracked in the usual way. 
    /// </remarks>
    public static virtual K<M, A> RetryWhileIO<A>(
        K<M, A> ma,
        Func<Error, bool> predicate) =>
        ma.MapIO(io => io.RetryWhile(predicate));

    /// <summary>
    /// Retry if the IO computation fails 
    /// </summary>
    /// <remarks>
    /// This variant will keep retrying whilst the predicate returns `true` for the error generated at each iteration;
    /// or, until the schedule expires; at which point the last raised error will be thrown.
    /// </remarks>
    /// <remarks>
    /// Any resources acquired within a retrying IO computation will automatically be released *if* the operation fails.
    /// So, successive retries will not grow the acquired resources on each retry iteration.  Any successful operation that
    /// acquires resources will have them tracked in the usual way. 
    /// </remarks>
    public static virtual K<M, A> RetryWhileIO<A>(
        K<M, A> ma,
        Schedule schedule,
        Func<Error, bool> predicate) =>
        ma.MapIO(io => io.RetryWhile(schedule, predicate));

    /// <summary>
    /// Retry if the IO computation fails 
    /// </summary>
    /// <remarks>
    /// This variant will keep retrying until the predicate returns `true` for the error generated at each iteration;
    /// at which point the last raised error will be thrown.
    /// </remarks>
    /// <remarks>
    /// Any resources acquired within a retrying IO computation will automatically be released *if* the operation fails.
    /// So, successive retries will not grow the acquired resources on each retry iteration.  Any successful operation that
    /// acquires resources will have them tracked in the usual way. 
    /// </remarks>
    public static virtual K<M, A> RetryUntilIO<A>(
        K<M, A> ma,
        Func<Error, bool> predicate) =>
        ma.MapIO(io => io.RetryUntil(predicate));

    /// <summary>
    /// Retry if the IO computation fails 
    /// </summary>
    /// <remarks>
    /// This variant will keep retrying until the predicate returns `true` for the error generated at each iteration;
    /// or, until the schedule expires; at which point the last raised error will be thrown.
    /// </remarks>
    /// <remarks>
    /// Any resources acquired within a retrying IO computation will automatically be released *if* the operation fails.
    /// So, successive retries will not grow the acquired resources on each retry iteration.  Any successful operation that
    /// acquires resources will have them tracked in the usual way. 
    /// </remarks>
    public static virtual K<M, A> RetryUntilIO<A>(
        K<M, A> ma,
        Schedule schedule,
        Func<Error, bool> predicate) =>
        ma.MapIO(io => io.RetryUntil(schedule, predicate));
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // 
    //  Folding
    //

    public static virtual K<M, S> FoldIO<S, A>(
        K<M, A> ma,
        Schedule schedule,
        S initialState,
        Func<S, A, S> folder) =>
        ma.MapIO(io => io.Fold(schedule, initialState, folder));

    public static virtual K<M, S> FoldIO<S, A>(
        K<M, A> ma,
        S initialState,
        Func<S, A, S> folder) =>
        ma.MapIO(io => io.Fold(initialState, folder));

    public static virtual K<M, S> FoldWhileIO<S, A>(
        K<M, A> ma,
        Schedule schedule,
        S initialState,
        Func<S, A, S> folder,
        Func<S, bool> stateIs) =>
        ma.MapIO(io => io.FoldWhile(schedule, initialState, folder, stateIs));

    public static virtual K<M, S> FoldWhileIO<S, A>(
        K<M, A> ma,
        S initialState,
        Func<S, A, S> folder,
        Func<S, bool> stateIs) =>
        ma.MapIO(io => io.FoldWhile(initialState, folder, stateIs));

    public static virtual K<M, S> FoldWhileIO<S, A>(
        K<M, A> ma,
        Schedule schedule,
        S initialState,
        Func<S, A, S> folder,
        Func<A, bool> valueIs) =>
        ma.MapIO(io => io.FoldWhile(schedule, initialState, folder, valueIs));

    public static virtual K<M, S> FoldWhileIO<S, A>(
        K<M, A> ma,
        S initialState,
        Func<S, A, S> folder,
        Func<A, bool> valueIs) =>
        ma.MapIO(io => io.FoldWhile(initialState, folder, valueIs));
    
    public static virtual K<M, S> FoldWhileIO<S, A>(
        K<M, A> ma,
        Schedule schedule,
        S initialState,
        Func<S, A, S> folder,
        Func<(S State, A Value), bool> predicate) =>
        ma.MapIO(io => io.FoldWhile(schedule, initialState, folder, predicate));

    public static virtual K<M, S> FoldWhileIO<S, A>(
        K<M, A> ma,
        S initialState,
        Func<S, A, S> folder,
        Func<(S State, A Value), bool> predicate) =>
        ma.MapIO(io => io.FoldWhile(initialState, folder, predicate));
    
    public static virtual K<M, S> FoldUntilIO<S, A>(
        K<M, A> ma,
        Schedule schedule,
        S initialState,
        Func<S, A, S> folder,
        Func<S, bool> stateIs) =>
        ma.MapIO(io => io.FoldUntil(schedule, initialState, folder, stateIs));
    
    public static virtual K<M, S> FoldUntilIO<S, A>(
        K<M, A> ma,
        S initialState,
        Func<S, A, S> folder,
        Func<S, bool> stateIs) =>
        ma.MapIO(io => io.FoldUntil(initialState, folder, stateIs));
    
    public static virtual K<M, S> FoldUntilIO<S, A>(
        K<M, A> ma,
        Schedule schedule,
        S initialState,
        Func<S, A, S> folder,
        Func<A, bool> valueIs) =>
        ma.MapIO(io => io.FoldUntil(schedule, initialState, folder, valueIs));
    
    public static virtual K<M, S> FoldUntilIO<S, A>(
        K<M, A> ma,
        S initialState,
        Func<S, A, S> folder,
        Func<A, bool> valueIs) =>
        ma.MapIO(io => io.FoldUntil(initialState, folder, valueIs));
    
    public static virtual K<M, S> FoldUntilIO<S, A>(
        K<M, A> ma,
        S initialState,
        Func<S, A, S> folder,
        Func<(S State, A Value), bool> predicate) =>
        ma.MapIO(io => io.FoldUntil(initialState, folder, predicate));

    public static virtual K<M, S> FoldUntilIO<S, A>(
        K<M, A> ma,
        Schedule schedule,
        S initialState,
        Func<S, A, S> folder,
        Func<(S State, A Value), bool> predicate) =>
        ma.MapIO(io => io.FoldUntil(schedule, initialState, folder, predicate));

    public static virtual K<M, C> SelectMany<A, B, C>(K<M, A> ma, Func<A, IO<B>> bind, Func<A, B, C> project) =>
        ma.Bind(x => bind(x) switch
                     {
                         IOTail<B> tail when typeof(B) == typeof(C) => (IO<C>)(object)tail,
                         IOTail<B> => throw new NotSupportedException("Tail calls can't transform in the `select`"),
                         var mb => mb.Map(y => project(x, y))
                     });

    public static virtual K<M, C> SelectMany<A, B, C>(IO<A> ma, Func<A, K<M, B>> bind, Func<A, B, C> project) =>
        M.SelectMany(M.LiftIO(ma), bind, project);
    
    public static virtual K<M, EnvIO> EnvIO => 
        M.LiftIO(IO.env);
    
    public static virtual K<M, CancellationToken> Token => 
        M.LiftIO(IO.token);

    public static virtual K<M, CancellationTokenSource> TokenSource =>
        M.LiftIO(IO.source);

    public static virtual K<M, Option<SynchronizationContext>> SyncContext =>
        M.LiftIO(IO.syncContext);
}
