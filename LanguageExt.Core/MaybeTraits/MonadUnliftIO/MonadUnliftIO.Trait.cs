using System;
using LanguageExt.Common;

namespace LanguageExt.Traits;

public static partial class Maybe
{
    /// <summary>
    /// Monad that is either the IO monad or a transformer with the IO monad in its stack
    /// </summary>
    /// <typeparam name="M">Self-referring trait</typeparam>
    public interface MonadUnliftIO<M> : Maybe.MonadIO<M>
        where M : MonadUnliftIO<M>, Traits.Monad<M>
    {
        /// <summary>
        /// Extract the IO monad from within the M monad (usually as part of a monad-transformer stack).
        /// </summary>
        /// <remarks>
        /// IMPLEMENTATION REQUIRED: If this method isn't overloaded in this monad
        /// or any monad in the stack on the way to the inner-monad, then it will throw
        /// an exception.
        ///
        /// This isn't ideal, it appears to be the only way to achieve this
        /// kind of functionality in C# without resorting to magic. 
        /// </remarks>
        /// <exception cref="ExceptionalException">If this method isn't overloaded in
        /// the inner monad or any monad in the stack on the way to the inner-monad,
        /// then it will throw an exception.</exception>
        public static virtual K<M, IO<A>> ToIOMaybe<A>(K<M, A> ma) =>
            throw new ExceptionalException(Errors.ToIONotSupported);

        /// <summary>
        /// Extract the IO monad from within the `M` monad (usually as part of a monad-transformer stack).  Then perform
        /// a mapping operation on the IO action before lifting the IO back into the `M` monad.
        /// </summary>
        public static virtual K<M, B> MapIOMaybe<A, B>(K<M, A> ma, Func<IO<A>, IO<B>> f) =>
            M.ToIOMaybe(ma).Bind(io => M.LiftIOMaybe(f(io)));

        /// <summary>
        /// Queue this IO operation to run on the thread-pool. 
        /// </summary>
        /// <param name="timeout">Maximum time that the forked IO operation can run for. `None` for no timeout.</param>
        /// <returns>Returns a `ForkIO` data-structure that contains two IO effects that can be used to either cancel
        /// the forked IO operation or to await the result of it.
        /// </returns>
        public static virtual K<M, ForkIO<A>> ForkIOMaybe<A>(K<M, A> ma, Option<TimeSpan> timeout = default) =>
            M.MapIOMaybe(ma, io => io.Fork(timeout));

        /// <summary>
        /// Await a forked operation
        /// </summary>
        public static virtual K<M, A> AwaitMaybe<A>(K<M, ForkIO<A>> ma) =>
            M.MapIOMaybe(ma, io => io.Bind(f => f.Await));

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
        public static virtual K<M, A> LocalIOMaybe<A>(K<M, A> ma) =>
            M.MapIOMaybe(ma, io => io.Local());

        /// <summary>
        /// Make this IO computation run on the `SynchronizationContext` that was captured at the start
        /// of the IO chain (i.e. the one embedded within the `EnvIO` environment that is passed through
        /// all IO computations)
        /// </summary>
        public static virtual K<M, A> PostIOMaybe<A>(K<M, A> ma) =>
            M.MapIOMaybe(ma, io => io.Post());

        /// <summary>
        /// Timeout operation if it takes too long
        /// </summary>
        public static virtual K<M, A> TimeoutIOMaybe<A>(K<M, A> ma, TimeSpan timeout) =>
            M.MapIOMaybe(ma, io => io.Timeout(timeout));

        /// <summary>
        /// The IO monad tracks resources automatically; this creates a local resource environment
        /// to run this computation in.  Once the computation is completed, any resources acquired
        /// are automatically released.  Imagine this as the ultimate `using` statement.
        /// </summary>
        public static virtual K<M, A> BracketIOMaybe<A>(K<M, A> ma) =>
            M.MapIOMaybe(ma, io => io.Bracket());

        /// <summary>
        /// When acquiring, using, and releasing various resources, it can be quite convenient to write a function to manage
        /// the acquisition and releasing, taking a function of the acquired value that specifies an action to be performed
        /// in between.
        /// </summary>
        /// <param name="Acq">Resource acquisition</param>
        /// <param name="Use">Function to use the acquired resource</param>
        /// <param name="Fin">Function to invoke to release the resource</param>
        public static virtual K<M, C> BracketIOMaybe<A, B, C>(
            K<M, A> Acq,
            Func<A, IO<C>> Use,
            Func<A, IO<B>> Fin) =>
            M.MapIOMaybe(Acq, io => io.Bracket(Use, Fin));

        /// <summary>
        /// When acquiring, using, and releasing various resources, it can be quite convenient to write a function to manage
        /// the acquisition and releasing, taking a function of the acquired value that specifies an action to be performed
        /// in between.
        /// </summary>
        /// <param name="Acq">Resource acquisition</param>
        /// <param name="Use">Function to use the acquired resource</param>
        /// <param name="Catch">Function to run to handle any exceptions</param>
        /// <param name="Fin">Function to invoke to release the resource</param>
        public static virtual K<M, C> BracketIOMaybe<A, B, C>(
            K<M, A> Acq,
            Func<A, IO<C>> Use,
            Func<Error, IO<C>> Catch,
            Func<A, IO<B>> Fin) =>
            M.MapIOMaybe(Acq, io => io.Bracket(Use, Catch, Fin));

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
        public static virtual K<M, A> RepeatIOMaybe<A>(K<M, A> ma) =>
            M.MapIOMaybe(ma, io => io.Repeat());

        /// <summary>
        /// Keeps repeating the computation until the scheduler expires, or an error occurs  
        /// </summary>
        /// <remarks>
        /// Any resources acquired within a repeated IO computation will automatically be released.  This also means you can't
        /// acquire resources and return them from within a repeated computation.
        /// </remarks>
        /// <param name="schedule">Scheduler strategy for repeating</param>
        /// <returns>The result of the last invocation</returns>
        public static virtual K<M, A> RepeatIOMaybe<A>(
            K<M, A> ma,
            Schedule schedule) =>
            M.MapIOMaybe(ma, io => io.Repeat(schedule));

        /// <summary>
        /// Keeps repeating the computation until the predicate returns false, or an error occurs 
        /// </summary>
        /// <remarks>
        /// Any resources acquired within a repeated IO computation will automatically be released.  This also means you can't
        /// acquire resources and return them from within a repeated computation.
        /// </remarks>
        /// <param name="predicate">Keep repeating while this predicate returns `true` for each computed value</param>
        /// <returns>The result of the last invocation</returns>
        public static virtual K<M, A> RepeatWhileIOMaybe<A>(
            K<M, A> ma,
            Func<A, bool> predicate) =>
            M.MapIOMaybe(ma, io => io.RepeatWhile(predicate));

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
        public static virtual K<M, A> RepeatWhileIOMaybe<A>(
            K<M, A> ma,
            Schedule schedule,
            Func<A, bool> predicate) =>
            M.MapIOMaybe(ma, io => io.RepeatWhile(schedule, predicate));

        /// <summary>
        /// Keeps repeating the computation until the predicate returns true, or an error occurs
        /// </summary>
        /// <remarks>
        /// Any resources acquired within a repeated IO computation will automatically be released.  This also means you can't
        /// acquire resources and return them from within a repeated computation.
        /// </remarks>
        /// <param name="predicate">Keep repeating until this predicate returns `true` for each computed value</param>
        /// <returns>The result of the last invocation</returns>
        public static virtual K<M, A> RepeatUntilIOMaybe<A>(
            K<M, A> ma,
            Func<A, bool> predicate) =>
            M.MapIOMaybe(ma, io => io.RepeatUntil(predicate));

        /// <summary>
        /// Keeps repeating the computation until the scheduler expires, or the predicate returns true, or an error occurs
        /// </summary>
        /// <remarks>
        /// Any resources acquired within a repeated IO computation will automatically be released.  This also means you can't
        /// acquire resources and return them from within a repeated computation.
        /// </remarks>
        /// <param name="schedule">Scheduler strategy for repeating</param>
        /// <param name="predicate">Keep repeating until this predicate returns `true` for each computed value</param>
        /// <returns>The result of the last invocation</returns>
        public static virtual K<M, A> RepeatUntilIOMaybe<A>(
            K<M, A> ma,
            Schedule schedule,
            Func<A, bool> predicate) =>
            M.MapIOMaybe(ma, io => io.RepeatUntil(schedule, predicate));

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
        public static virtual K<M, A> RetryIOMaybe<A>(K<M, A> ma) =>
            M.MapIOMaybe(ma, io => io.Retry());

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
        public static virtual K<M, A> RetryIOMaybe<A>(
            K<M, A> ma,
            Schedule schedule) =>
            M.MapIOMaybe(ma, io => io.Retry(schedule));

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
        public static virtual K<M, A> RetryWhileIOMaybe<A>(
            K<M, A> ma,
            Func<Error, bool> predicate) =>
            M.MapIOMaybe(ma, io => io.RetryWhile(predicate));

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
        public static virtual K<M, A> RetryWhileIOMaybe<A>(
            K<M, A> ma,
            Schedule schedule,
            Func<Error, bool> predicate) =>
            M.MapIOMaybe(ma, io => io.RetryWhile(schedule, predicate));

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
        public static virtual K<M, A> RetryUntilIOMaybe<A>(
            K<M, A> ma,
            Func<Error, bool> predicate) =>
            M.MapIOMaybe(ma, io => io.RetryUntil(predicate));

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
        public static virtual K<M, A> RetryUntilIOMaybe<A>(
            K<M, A> ma,
            Schedule schedule,
            Func<Error, bool> predicate) =>
            M.MapIOMaybe(ma, io => io.RetryUntil(schedule, predicate));

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // 
        //  Folding
        //

        public static virtual K<M, S> FoldIOMaybe<S, A>(
            K<M, A> ma,
            Schedule schedule,
            S initialState,
            Func<S, A, S> folder) =>
            M.MapIOMaybe(ma, io => io.Fold(schedule, initialState, folder));

        public static virtual K<M, S> FoldIOMaybe<S, A>(
            K<M, A> ma,
            S initialState,
            Func<S, A, S> folder) =>
            M.MapIOMaybe(ma, io => io.Fold(initialState, folder));

        public static virtual K<M, S> FoldWhileIOMaybe<S, A>(
            K<M, A> ma,
            Schedule schedule,
            S initialState,
            Func<S, A, S> folder,
            Func<S, bool> stateIs) =>
            M.MapIOMaybe(ma, io => io.FoldWhile(schedule, initialState, folder, stateIs));

        public static virtual K<M, S> FoldWhileIOMaybe<S, A>(
            K<M, A> ma,
            S initialState,
            Func<S, A, S> folder,
            Func<S, bool> stateIs) =>
            M.MapIOMaybe(ma, io => io.FoldWhile(initialState, folder, stateIs));

        public static virtual K<M, S> FoldWhileIOMaybe<S, A>(
            K<M, A> ma,
            Schedule schedule,
            S initialState,
            Func<S, A, S> folder,
            Func<A, bool> valueIs) =>
            M.MapIOMaybe(ma, io => io.FoldWhile(schedule, initialState, folder, valueIs));

        public static virtual K<M, S> FoldWhileIOMaybe<S, A>(
            K<M, A> ma,
            S initialState,
            Func<S, A, S> folder,
            Func<A, bool> valueIs) =>
            M.MapIOMaybe(ma, io => io.FoldWhile(initialState, folder, valueIs));

        public static virtual K<M, S> FoldWhileIOMaybe<S, A>(
            K<M, A> ma,
            Schedule schedule,
            S initialState,
            Func<S, A, S> folder,
            Func<(S State, A Value), bool> predicate) =>
            M.MapIOMaybe(ma, io => io.FoldWhile(schedule, initialState, folder, predicate));

        public static virtual K<M, S> FoldWhileIOMaybe<S, A>(
            K<M, A> ma,
            S initialState,
            Func<S, A, S> folder,
            Func<(S State, A Value), bool> predicate) =>
            M.MapIOMaybe(ma, io => io.FoldWhile(initialState, folder, predicate));

        public static virtual K<M, S> FoldUntilIOMaybe<S, A>(
            K<M, A> ma,
            Schedule schedule,
            S initialState,
            Func<S, A, S> folder,
            Func<S, bool> stateIs) =>
            M.MapIOMaybe(ma, io => io.FoldUntil(schedule, initialState, folder, stateIs));

        public static virtual K<M, S> FoldUntilIOMaybe<S, A>(
            K<M, A> ma,
            S initialState,
            Func<S, A, S> folder,
            Func<S, bool> stateIs) =>
            M.MapIOMaybe(ma, io => io.FoldUntil(initialState, folder, stateIs));

        public static virtual K<M, S> FoldUntilIOMaybe<S, A>(
            K<M, A> ma,
            Schedule schedule,
            S initialState,
            Func<S, A, S> folder,
            Func<A, bool> valueIs) =>
            M.MapIOMaybe(ma, io => io.FoldUntil(schedule, initialState, folder, valueIs));

        public static virtual K<M, S> FoldUntilIOMaybe<S, A>(
            K<M, A> ma,
            S initialState,
            Func<S, A, S> folder,
            Func<A, bool> valueIs) =>
            M.MapIOMaybe(ma, io => io.FoldUntil(initialState, folder, valueIs));

        public static virtual K<M, S> FoldUntilIOMaybe<S, A>(
            K<M, A> ma,
            S initialState,
            Func<S, A, S> folder,
            Func<(S State, A Value), bool> predicate) =>
            M.MapIOMaybe(ma, io => io.FoldUntil(initialState, folder, predicate));

        public static virtual K<M, S> FoldUntilIOMaybe<S, A>(
            K<M, A> ma,
            Schedule schedule,
            S initialState,
            Func<S, A, S> folder,
            Func<(S State, A Value), bool> predicate) =>
            M.MapIOMaybe(ma, io => io.FoldUntil(schedule, initialState, folder, predicate));
    }
}
