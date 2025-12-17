using System;
using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt;

public static class MonadUnliftIOExtensions
{
    /// <summary>
    /// Await a forked operation
    /// </summary>
    public static K<M, A> Await<M, A>(this K<M, ForkIO<A>> ma) 
        where M : MonadUnliftIO<M> =>
        M.Await(ma);

    /// <param name="ma">Resource acquisition</param>
    extension<M, A>(K<M, A> ma) where M : MonadUnliftIO<M>
    {
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
        /// <returns>Result of the computation</returns>
        public K<M, A> LocalIO() =>
            M.LocalIO(ma);

        /// <summary>
        /// Wraps this computation in a local-environment that ignores any cancellation-token cancellation requests.
        /// </summary>
        /// <returns>An uninterruptible computation</returns>
        public K<M, A> UninterruptibleIO() =>
            M.UninterruptibleIO(ma);

        /// <summary>
        /// Make this IO computation run on the `SynchronizationContext` that was captured at the start
        /// of the IO chain (i.e. the one embedded within the `EnvIO` environment that is passed through
        /// all IO computations)
        /// </summary>
        public K<M, A> PostIO() =>
            M.PostIO(ma);
        
        /// <summary>
        /// Timeout operation if it takes too long
        /// </summary>
        public K<M, A> TimeoutIO(TimeSpan timeout) =>
            M.TimeoutIO(ma, timeout);

        /// <summary>
        /// The IO monad tracks resources automatically, this creates a local resource environment
        /// to run this computation in.  Once the computation has completed any resources acquired
        /// are automatically released.  Imagine this as the ultimate `using` statement.
        /// </summary>
        public K<M, A> BracketIO() =>
            M.BracketIO(ma);

        /// <summary>
        /// When acquiring, using, and releasing various resources, it can be quite convenient to write a function to manage
        /// the acquisition and releasing, taking a function of the acquired value that specifies an action to be performed
        /// in between.
        /// </summary>
        /// <param name="Use">Function to use the acquired resource</param>
        /// <param name="Fin">Function to invoke to release the resource</param>
        public K<M, C> BracketIO<B, C>(Func<A, IO<C>> Use,
                                       Func<A, IO<B>> Fin) =>
            M.BracketIO(ma, Use, Fin);

        /// <summary>
        /// When acquiring, using, and releasing various resources, it can be quite convenient to write a function to manage
        /// the acquisition and releasing, taking a function of the acquired value that specifies an action to be performed
        /// in between.
        /// </summary>
        /// <param name="Use">Function to use the acquired resource</param>
        /// <param name="Catch">Function to run to handle any exceptions</param>
        /// <param name="Fin">Function to invoke to release the resource</param>
        public K<M, C> BracketIO<B, C>(Func<A, IO<C>> Use,
                                       Func<Error, IO<C>> Catch,
                                       Func<A, IO<B>> Fin) =>
            M.BracketIO(ma, Use, Catch, Fin);


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
        public K<M, A> RepeatIO() =>
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
        public K<M, A> RepeatIO(Schedule schedule) =>
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
        public K<M, A> RepeatWhileIO(Func<A, bool> predicate) =>
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
        public K<M, A> RepeatWhileIO(Schedule schedule,
                                     Func<A, bool> predicate) =>
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
        public K<M, A> RepeatUntilIO(Func<A, bool> predicate) =>
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
        public K<M, A> RepeatUntilIO(Schedule schedule,
                                     Func<A, bool> predicate) =>
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
        public K<M, A> RetryIO() =>
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
        public K<M, A> RetryIO(Schedule schedule) =>
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
        public K<M, A> RetryWhileIO(Func<Error, bool> predicate) =>
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
        public K<M, A> RetryWhileIO(Schedule schedule,
                                    Func<Error, bool> predicate) =>
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
        public K<M, A> RetryUntilIO(Func<Error, bool> predicate) =>
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
        public K<M, A> RetryUntilIO(Schedule schedule,
                                    Func<Error, bool> predicate) =>
            M.RetryUntilIO(ma, schedule, predicate);
    }
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // 
    //  Folding
    //

    public static K<M, S> FoldIO<M, S, A>(
        this K<M, A> ma,
        Schedule schedule,
        S initialState,
        Func<S, A, S> folder) 
        where M : MonadUnliftIO<M> =>
        M.FoldIO(ma, schedule, initialState, folder);

    public static K<M, S> FoldIO<M, S, A>(
        this K<M, A> ma,
        S initialState,
        Func<S, A, S> folder) 
        where M : MonadUnliftIO<M> =>
        M.FoldIO(ma, initialState, folder);

    public static K<M, S> FoldWhileIO<M, S, A>(
        this K<M, A> ma,
        Schedule schedule,
        S initialState,
        Func<S, A, S> folder,
        Func<S, bool> stateIs) 
        where M : MonadUnliftIO<M> =>
        M.FoldWhileIO(ma, schedule, initialState, folder, stateIs);

    public static K<M, S> FoldWhileIO<M, S, A>(
        this K<M, A> ma,
        S initialState,
        Func<S, A, S> folder,
        Func<S, bool> stateIs) 
        where M : MonadUnliftIO<M> =>
        M.FoldWhileIO(ma, initialState, folder, stateIs);

    public static K<M, S> FoldWhileIO<M, S, A>(
        this K<M, A> ma,
        Schedule schedule,
        S initialState,
        Func<S, A, S> folder,
        Func<A, bool> valueIs) 
        where M : MonadUnliftIO<M> =>
        M.FoldWhileIO(ma, schedule, initialState, folder, valueIs);

    public static K<M, S> FoldWhileIO<M, S, A>(
        this K<M, A> ma,
        S initialState,
        Func<S, A, S> folder,
        Func<A, bool> valueIs) 
        where M : MonadUnliftIO<M> =>
        M.FoldWhileIO(ma, initialState, folder, valueIs);
    
    public static K<M, S> FoldWhileIO<M, S, A>(
        this K<M, A> ma,
        Schedule schedule,
        S initialState,
        Func<S, A, S> folder,
        Func<(S State, A Value), bool> predicate) 
        where M : MonadUnliftIO<M> =>
        M.FoldWhileIO(ma, schedule, initialState, folder, predicate);

    public static K<M, S> FoldWhileIO<M, S, A>(
        this K<M, A> ma,
        S initialState,
        Func<S, A, S> folder,
        Func<(S State, A Value), bool> predicate) 
        where M : MonadUnliftIO<M> =>
        M.FoldWhileIO(ma, initialState, folder, predicate);
    
    public static K<M, S> FoldUntilIO<M, S, A>(
        this K<M, A> ma,
        Schedule schedule,
        S initialState,
        Func<S, A, S> folder,
        Func<S, bool> stateIs) 
        where M : MonadUnliftIO<M> =>
        M.FoldUntilIO(ma, schedule, initialState, folder, stateIs);
    
    public static K<M, S> FoldUntilIO<M, S, A>(
        this K<M, A> ma,
        S initialState,
        Func<S, A, S> folder,
        Func<S, bool> stateIs) 
        where M : MonadUnliftIO<M> =>
        M.FoldUntilIO(ma, initialState, folder, stateIs);
    
    public static K<M, S> FoldUntilIO<M, S, A>(
        this K<M, A> ma,
        Schedule schedule,
        S initialState,
        Func<S, A, S> folder,
        Func<A, bool> valueIs) 
        where M : MonadUnliftIO<M> =>
        M.FoldUntilIO(ma, schedule, initialState, folder, valueIs);
    
    public static K<M, S> FoldUntilIO<M, S, A>(
        this K<M, A> ma,
        S initialState,
        Func<S, A, S> folder,
        Func<A, bool> valueIs) 
        where M : MonadUnliftIO<M> =>
        M.FoldUntilIO(ma, initialState, folder, valueIs);
    
    public static K<M, S> FoldUntilIO<M, S, A>(
        this K<M, A> ma,
        S initialState,
        Func<S, A, S> folder,
        Func<(S State, A Value), bool> predicate) 
        where M : MonadUnliftIO<M> =>
        M.FoldUntilIO(ma, initialState, folder, predicate);

    public static K<M, S> FoldUntilIO<M, S, A>(
        this K<M, A> ma,
        Schedule schedule,
        S initialState,
        Func<S, A, S> folder,
        Func<(S State, A Value), bool> predicate) 
        where M : MonadUnliftIO<M> =>
        M.FoldUntilIO(ma, schedule, initialState, folder, predicate);
 }
