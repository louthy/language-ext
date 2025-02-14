#pragma warning disable LX_StreamT

using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using LanguageExt.Common;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

public static partial class IOExtensions
{
    /// <summary>
    /// Convert the kind version of the `IO` monad to an `IO` monad.
    /// </summary>
    /// <remarks>
    /// This is a simple cast operation which is just a bit more elegant
    /// than manually casting.
    /// </remarks>
    /// <param name="ma"></param>
    /// <typeparam name="A"></typeparam>
    /// <returns></returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IO<A> As<A>(this K<IO, A> ma) =>
        (IO<A>)ma;

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static A Run<A>(this K<IO, A> ma, EnvIO? envIO = null) =>
        ma.As().Run(envIO);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Fin<A> RunSafe<A>(this K<IO, A> ma, EnvIO? envIO = null) =>
        ma.As().Try().Run().Run(envIO);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueTask<A> RunAsync<A>(this K<IO, A> ma, EnvIO? envIO = null) =>
        ma.As().RunAsync(envIO);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueTask<Fin<A>> RunSafeAsync<A>(this K<IO, A> ma, EnvIO? envIO = null) =>
        ma.As().Try().Run().RunAsync(envIO);
    
    /// <summary>
    /// Get the outer task and wrap it up in a new IO within the IO
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IO<A> Flatten<A>(this Task<IO<A>> tma) =>
        IO.liftAsync(async () => await tma.ConfigureAwait(false))
          .Flatten();

    /// <summary>
    /// Unwrap the inner IO to flatten the structure
    /// </summary>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IO<A> Flatten<A>(this IO<IO<A>> mma) =>
        mma.Bind(x => x);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IO<C> SelectMany<A, B, C>(this K<IO, A> ma, Func<A, K<IO, B>> bind, Func<A, B, C> project) =>
        ma.As().SelectMany(bind, project);

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
    public static K<M, A> LocalIO<M, A>(this K<M, A> ma) 
        where M : Monad<M> =>
        M.LocalIO(ma);

    /// <summary>
    /// Make this IO computation run on the `SynchronizationContext` that was captured at the start
    /// of the IO chain (i.e. the one embedded within the `EnvIO` environment that is passed through
    /// all IO computations)
    /// </summary>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static K<M, A> PostIO<M, A>(this K<M, A> ma)
        where M : Monad<M> =>
        M.PostIO(ma);

    /// <summary>
    /// Await a forked operation
    /// </summary>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static K<M, A> Await<M, A>(this K<M, ForkIO<A>> ma)
        where M : Monad<M> =>
        M.Await(ma);

    /// <summary>
    /// Queue this IO operation to run on the thread-pool. 
    /// </summary>
    /// <param name="timeout">Maximum time that the forked IO operation can run for. `None` for no timeout.</param>
    /// <returns>Returns a `ForkIO` data-structure that contains two IO effects that can be used to either cancel
    /// the forked IO operation or to await the result of it.
    /// </returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static K<M, ForkIO<A>> ForkIO<M, A>(this K<M, A> ma, Option<TimeSpan> timeout = default)
        where M : Monad<M> =>
        M.ForkIO(ma, timeout);

    /// <summary>
    /// Queue this IO operation to run on the thread-pool. 
    /// </summary>
    /// <param name="timeout">Maximum time that the forked IO operation can run for. `None` for no timeout.</param>
    /// <returns>Returns a `ForkIO` data-structure that contains two IO effects that can be used to either cancel
    /// the forked IO operation or to await the result of it.
    /// </returns>
    [Pure]
    [MethodImpl(Opt.Default)]
    public static K<M, ForkIO<Option<A>>> ForkIO<M, A>(this StreamT<M, A> ma, Option<TimeSpan> timeout = default)
        where M : Monad<M> =>
        ma.Run()
          .Map(oht => oht.Map(ht => ht.Item1))
          .ForkIO(timeout);

    /// <summary>
    /// Timeout operation if it takes too long
    /// </summary>
    [Pure]
    [MethodImpl(Opt.Default)]
    public static K<M, A> TimeoutIO<M, A>(this K<M, A> ma, TimeSpan timeout)
        where M : Monad<M>, MonadIO<M> =>
        M.TimeoutIO(ma, timeout);
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Brackets
    //

    /// <summary>
    /// The IO monad tracks resources automatically, this creates a local resource environment
    /// to run this computation in.  Once the computation has completed any resources acquired
    /// are automatically released.  Imagine this as the ultimate `using` statement.
    /// </summary>
    [Pure]
    [MethodImpl(Opt.Default)]
    public static K<M, A> BracketIO<M, A>(this K<M, A> ma)
        where M : Monad<M> =>
        M.BracketIO(ma);

    /// <summary>
    /// When acquiring, using, and releasing various resources, it can be quite convenient to write a function to manage
    /// the acquisition and releasing, taking a function of the acquired value that specifies an action to be performed
    /// in between.
    /// </summary>
    /// <param name="acq">Resource acquisition</param>
    /// <param name="Use">Function to use the acquired resource</param>
    /// <param name="Fin">Function to invoke to release the resource</param>
    public static K<M, C> BracketIO<M, A, B, C>(
        this K<M, A> acq, 
        Func<A, IO<C>> Use, 
        Func<A, IO<B>> Fin) 
        where M : Monad<M> =>
        M.BracketIO(acq, Use, Fin);

    /// <summary>
    /// When acquiring, using, and releasing various resources, it can be quite convenient to write a function to manage
    /// the acquisition and releasing, taking a function of the acquired value that specifies an action to be performed
    /// in between.
    /// </summary>
    /// <param name="acq">Resource acquisition</param>
    /// <param name="Use">Function to use the acquired resource</param>
    /// <param name="Catch">Function to run to handle any exceptions</param>
    /// <param name="Fin">Function to invoke to release the resource</param>
    public static K<M, C> BracketIO<M, A, B, C>(
        this K<M, A> acq,
        Func<A, IO<C>> Use,
        Func<Error, IO<C>> Catch,
        Func<A, IO<B>> Fin)
        where M : Monad<M> =>
        M.BracketIO(acq, Use, Catch, Fin);

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
    public static K<M, A> RepeatIO<M, A>(this K<M, A> ma)
        where M : Monad<M> =>
        M.RepeatIO(ma);

    /// <summary>
    /// Keeps repeating the computation, until the scheduler expires, or an error occurs  
    /// </summary>
    /// <remarks>
    /// Any resources acquired within a repeated IO computation will automatically be released.  This also means you can't
    /// acquire resources and return them from within a repeated computation.
    /// </remarks>
    /// <param name="schedule">Scheduler strategy for repeating</param>
    /// <returns>The result of the last invocation</returns>
    public static K<M, A> RepeatIO<M, A>(
        this K<M, A> ma, 
        Schedule schedule)
        where M : Monad<M> =>
        M.RepeatIO(ma, schedule);

    /// <summary>
    /// Keeps repeating the computation until the predicate returns false, or an error occurs 
    /// </summary>
    /// <remarks>
    /// Any resources acquired within a repeated IO computation will automatically be released.  This also means you can't
    /// acquire resources and return them from within a repeated computation.
    /// </remarks>
    /// <param name="predicate">Keep repeating while this predicate returns `true` for each computed value</param>
    /// <returns>The result of the last invocation</returns>
    public static K<M, A> RepeatWhileIO<M, A>(
        this K<M, A> ma, 
        Func<A, bool> predicate) 
        where M : Monad<M> =>
        M.RepeatWhileIO(ma, predicate);

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
    public static K<M, A> RepeatWhileIO<M, A>(
        this K<M, A> ma, 
        Schedule schedule,
        Func<A, bool> predicate)
        where M : Monad<M> =>
        M.RepeatWhileIO(ma, schedule, predicate);

    /// <summary>
    /// Keeps repeating the computation until the predicate returns true, or an error occurs
    /// </summary>
    /// <remarks>
    /// Any resources acquired within a repeated IO computation will automatically be released.  This also means you can't
    /// acquire resources and return them from within a repeated computation.
    /// </remarks>
    /// <param name="predicate">Keep repeating until this predicate returns `true` for each computed value</param>
    /// <returns>The result of the last invocation</returns>
    public static K<M, A> RepeatUntilIO<M, A>(
        this K<M, A> ma, 
        Func<A, bool> predicate)
        where M : Monad<M> =>
        M.RepeatUntilIO(ma, predicate);

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
    public static K<M, A> RepeatUntilIO<M, A>(
        this K<M, A> ma, 
        Schedule schedule,
        Func<A, bool> predicate)
        where M : Monad<M> =>
        M.RepeatUntilIO(ma, schedule, predicate);

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
    public static K<M, A> RetryIO<M, A>(this K<M, A> ma) 
        where M : Monad<M> =>
        M.RetryIO(ma);

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
    public static K<M, A> RetryIO<M, A>(
        this K<M, A> ma,
        Schedule schedule) 
        where M : Monad<M> =>
        M.RetryIO(ma, schedule);

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
    public static K<M, A> RetryWhileIO<M, A>(
        this K<M, A> ma,
        Func<Error, bool> predicate)  
        where M : Monad<M> =>
        M.RetryWhileIO(ma, predicate);

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
    public static K<M, A> RetryWhileIO<M, A>(
        this K<M, A> ma,
        Schedule schedule,
        Func<Error, bool> predicate) 
        where M : Monad<M> =>
        M.RetryWhileIO(ma, schedule, predicate);

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
    public static K<M, A> RetryUntilIO<M, A>(
        this K<M, A> ma,
        Func<Error, bool> predicate)
        where M : Monad<M> =>
        M.RetryUntilIO(ma, predicate);

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
    public static K<M, A> RetryUntilIO<M, A>(
        this K<M, A> ma,
        Schedule schedule,
        Func<Error, bool> predicate) 
        where M : Monad<M> =>
        M.RetryUntilIO(ma, schedule, predicate);
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // 
    //  Folding
    //

    public static K<M, S> FoldIO<S, M, A>(
        this K<M, A> ma,
        Schedule schedule,
        S initialState,
        Func<S, A, S> folder)
        where M : Monad<M> =>
        M.FoldIO(ma, schedule, initialState, folder);

    public static K<M, S> FoldIO<S, M, A>(
        this K<M, A> ma,
        S initialState,
        Func<S, A, S> folder)
        where M : Monad<M> =>
        M.FoldIO(ma, initialState, folder);

    public static K<M, S> FoldWhileIO<S, M, A>(
        this K<M, A> ma,
        Schedule schedule,
        S initialState,
        Func<S, A, S> folder,
        Func<S, bool> stateIs)
        where M : Monad<M> =>
        M.FoldWhileIO(ma, schedule, initialState, folder, stateIs);

    public static K<M, S> FoldWhileIO<S, M, A>(
        this K<M, A> ma,
        S initialState,
        Func<S, A, S> folder,
        Func<S, bool> stateIs)
        where M : Monad<M> =>
        M.FoldWhileIO(ma, initialState, folder, stateIs);

    public static K<M, S> FoldWhileIO<S, M, A>(
        this K<M, A> ma,
        Schedule schedule,
        S initialState,
        Func<S, A, S> folder,
        Func<A, bool> valueIs)
        where M : Monad<M> =>
        M.FoldWhileIO(ma, schedule, initialState, folder, valueIs);

    public static K<M, S> FoldWhileIO<S, M, A>(
        this K<M, A> ma,
        S initialState,
        Func<S, A, S> folder,
        Func<A, bool> valueIs)
        where M : Monad<M> =>
        M.FoldWhileIO(ma, initialState, folder, valueIs);
    
    public static K<M, S> FoldWhileIO<S, M, A>(
        this K<M, A> ma,
        Schedule schedule,
        S initialState,
        Func<S, A, S> folder,
        Func<(S State, A Value), bool> predicate)
        where M : Monad<M> =>
        M.FoldWhileIO(ma, schedule, initialState, folder, predicate);

    public static K<M, S> FoldWhileIO<S, M, A>(
        this K<M, A> ma,
        S initialState,
        Func<S, A, S> folder,
        Func<(S State, A Value), bool> predicate)
        where M : Monad<M> =>
        M.FoldWhileIO(ma, initialState, folder, predicate);
    
    public static K<M, S> FoldUntilIO<S, M, A>(
        this K<M, A> ma,
        Schedule schedule,
        S initialState,
        Func<S, A, S> folder,
        Func<S, bool> stateIs)
        where M : Monad<M> =>
        M.FoldUntilIO(ma, schedule, initialState, folder, stateIs);
    
    public static K<M, S> FoldUntilIO<S, M, A>(
        this K<M, A> ma,
        S initialState,
        Func<S, A, S> folder,
        Func<S, bool> stateIs)
        where M : Monad<M> =>
        M.FoldUntilIO(ma, initialState, folder, stateIs);
    
    public static K<M, S> FoldUntilIO<S, M, A>(
        this K<M, A> ma,
        Schedule schedule,
        S initialState,
        Func<S, A, S> folder,
        Func<A, bool> valueIs)
        where M : Monad<M> =>
        M.FoldUntilIO(ma, schedule, initialState, folder, valueIs);
    
    public static K<M, S> FoldUntilIO<S, M, A>(
        this K<M, A> ma,
        S initialState,
        Func<S, A, S> folder,
        Func<A, bool> valueIs)
        where M : Monad<M> =>
        M.FoldUntilIO(ma, initialState, folder, valueIs);
    
    public static K<M, S> FoldUntilIO<S, M, A>(
        this K<M, A> ma,
        S initialState,
        Func<S, A, S> folder,
        Func<(S State, A Value), bool> predicate)
        where M : Monad<M> =>
        M.FoldUntilIO(ma, initialState, folder, predicate);

    public static K<M, S> FoldUntilIO<S, M, A>(
        this K<M, A> ma,
        Schedule schedule,
        S initialState,
        Func<S, A, S> folder,
        Func<(S State, A Value), bool> predicate)
        where M : Monad<M> =>
        M.FoldUntilIO(ma, schedule, initialState, folder, predicate);
}
