using System;
using System.Threading;
using LanguageExt.Common;
using LanguageExt.DSL;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class Deriving
{
    public static partial class Maybe
    {
        /// <summary>
        /// Derived `MonadIO` implementation
        /// </summary>
        /// <typeparam name="Supertype">Super-type wrapper around the subtype</typeparam>
        /// <typeparam name="Subtype">The subtype that the supertype type 'wraps'</typeparam>
        public interface MonadIO<Supertype, Subtype> :
            Monad<Supertype, Subtype>,
            MonadIO<Supertype>
            where Subtype : Traits.Maybe.MonadIO<Subtype>, Monad<Subtype>
            where Supertype : MonadIO<Supertype, Subtype>, Monad<Supertype>
        {
            /// <summary>
            /// Lift an IO operation into the `Self` monad
            /// </summary>
            /// <param name="ma">IO structure to lift</param>
            /// <typeparam name="A">Bound value type</typeparam>
            /// <returns>Monad with an `IO` structure lifted into it</returns>
            /// <exception cref="ExceptionalException">If this method isn't overloaded in
            /// the inner monad or any monad in the stack on the way to the inner monad
            /// then it will throw an exception.</exception>
            static K<Supertype, A> Traits.Maybe.MonadIO<Supertype>.LiftIO<A>(K<IO, A> ma) =>
                Supertype.CoTransform(Subtype.LiftIO(ma));

            /// <summary>
            /// Lift an IO operation into the `Self` monad
            /// </summary>
            /// <param name="ma">IO structure to lift</param>
            /// <typeparam name="A">Bound value type</typeparam>
            /// <returns>Monad with an `IO` structure lifted into it</returns>
            /// <exception cref="ExceptionalException">If this method isn't overloaded in
            /// the inner monad or any monad in the stack on the way to the inner monad
            /// then it will throw an exception.</exception>
            static K<Supertype, A> Traits.Maybe.MonadIO<Supertype>.LiftIO<A>(IO<A> ma) =>
                Supertype.CoTransform(Subtype.LiftIO(ma));

            /// <summary>
            /// Extract the inner `IO` monad from the `Self` structure provided
            /// </summary>
            /// <param name="ma">`Self` structure to extract the `IO` monad from</param>
            /// <typeparam name="A">Bound value type</typeparam>
            /// <returns>`Self` structure with the `IO` structure as the bound value</returns>
            static K<Supertype, IO<A>> Traits.Maybe.MonadIO<Supertype>.ToIO<A>(K<Supertype, A> ma) =>
                Supertype.CoTransform(Subtype.ToIO(Supertype.Transform(ma)));

            /// <summary>
            /// Map the inner `IO` monad within the `Self` structure provided
            /// </summary>
            /// <param name="ma">`Self` structure to extract the `IO` monad from</param>
            /// <param name="f">`IO` structure mapping function</param>
            /// <typeparam name="A">Input bound value type</typeparam>
            /// <typeparam name="B">Output bound value type</typeparam>
            /// <returns>`Self` structure that has had its inner `IO` monad mapped</returns>
            /// <exception cref="ExceptionalException">If this method isn't overloaded in
            /// the inner monad or any monad in the stack on the way to the inner monad
            /// then it will throw an exception.</exception>
            static K<Supertype, B> Traits.Maybe.MonadIO<Supertype>.MapIO<A, B>(K<Supertype, A> ma, Func<IO<A>, IO<B>> f) =>
                Supertype.CoTransform(Subtype.MapIO(Supertype.Transform(ma), f));

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
            static K<Supertype, A> MonadIO<Supertype>.LocalIO<A>(K<Supertype, A> ma) =>
                ma.MapIO(io => io.Local());

            /// <summary>
            /// Make this IO computation run on the `SynchronizationContext` that was captured at the start
            /// of the IO chain (i.e. the one embedded within the `EnvIO` environment that is passed through
            /// all IO computations)
            /// </summary>
            static K<Supertype, A> MonadIO<Supertype>.PostIO<A>(K<Supertype, A> ma) =>
                ma.MapIO(io => io.Post());

            /// <summary>
            /// Await a forked operation
            /// </summary>
            static K<Supertype, A> MonadIO<Supertype>.Await<A>(K<Supertype, ForkIO<A>> ma) =>
                ma.MapIO(io => io.Bind(f => f.Await));

            /// <summary>
            /// Queue this IO operation to run on the thread-pool. 
            /// </summary>
            /// <param name="timeout">Maximum time that the forked IO operation can run for. `None` for no timeout.</param>
            /// <returns>Returns a `ForkIO` data-structure that contains two IO effects that can be used to either cancel
            /// the forked IO operation or to await the result of it.
            /// </returns>
            static K<Supertype, ForkIO<A>> Traits.Maybe.MonadIO<Supertype>.ForkIO<A>(
                K<Supertype, A> ma,
                Option<TimeSpan> timeout) =>
                Supertype.CoTransform(Subtype.ForkIO(Supertype.Transform(ma), timeout));

            /// <summary>
            /// Timeout operation if it takes too long
            /// </summary>
            static K<Supertype, A> MonadIO<Supertype>.TimeoutIO<A>(K<Supertype, A> ma, TimeSpan timeout) =>
                ma.MapIO(io => io.Timeout(timeout));

            /// <summary>
            /// The IO monad tracks resources automatically, this creates a local resource environment
            /// to run this computation in.  Once the computation has completed any resources acquired
            /// are automatically released.  Imagine this as the ultimate `using` statement.
            /// </summary>
            static K<Supertype, A> MonadIO<Supertype>.BracketIO<A>(K<Supertype, A> ma) =>
                ma.MapIO(io => io.Bracket());

            /// <summary>
            /// When acquiring, using, and releasing various resources, it can be quite convenient to write a function to manage
            /// the acquisition and releasing, taking a function of the acquired value that specifies an action to be performed
            /// in between.
            /// </summary>
            /// <param name="Acq">Resource acquisition</param>
            /// <param name="Use">Function to use the acquired resource</param>
            /// <param name="Fin">Function to invoke to release the resource</param>
            static K<Supertype, C> MonadIO<Supertype>.BracketIO<A, B, C>(
                K<Supertype, A> Acq,
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
            static K<Supertype, C> MonadIO<Supertype>.BracketIO<A, B, C>(
                K<Supertype, A> Acq,
                Func<A, IO<C>> Use,
                Func<Error, IO<C>> Catch,
                Func<A, IO<B>> Fin) =>
                Acq.MapIO(io => io.Bracket(Use, Catch, Fin));

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
            static K<Supertype, A> MonadIO<Supertype>.RepeatIO<A>(K<Supertype, A> ma) =>
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
            static K<Supertype, A> MonadIO<Supertype>.RepeatIO<A>(
                K<Supertype, A> ma,
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
            static K<Supertype, A> MonadIO<Supertype>.RepeatWhileIO<A>(
                K<Supertype, A> ma,
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
            static K<Supertype, A> MonadIO<Supertype>.RepeatWhileIO<A>(
                K<Supertype, A> ma,
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
            static K<Supertype, A> MonadIO<Supertype>.RepeatUntilIO<A>(
                K<Supertype, A> ma,
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
            static K<Supertype, A> MonadIO<Supertype>.RepeatUntilIO<A>(
                K<Supertype, A> ma,
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
            static K<Supertype, A> MonadIO<Supertype>.RetryIO<A>(K<Supertype, A> ma) =>
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
            static K<Supertype, A> MonadIO<Supertype>.RetryIO<A>(
                K<Supertype, A> ma,
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
            static K<Supertype, A> MonadIO<Supertype>.RetryWhileIO<A>(
                K<Supertype, A> ma,
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
            static K<Supertype, A> MonadIO<Supertype>.RetryWhileIO<A>(
                K<Supertype, A> ma,
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
            static K<Supertype, A> MonadIO<Supertype>.RetryUntilIO<A>(
                K<Supertype, A> ma,
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
            static K<Supertype, A> MonadIO<Supertype>.RetryUntilIO<A>(
                K<Supertype, A> ma,
                Schedule schedule,
                Func<Error, bool> predicate) =>
                ma.MapIO(io => io.RetryUntil(schedule, predicate));

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // 
            //  Folding
            //

            static K<Supertype, S> MonadIO<Supertype>.FoldIO<S, A>(
                K<Supertype, A> ma,
                Schedule schedule,
                S initialState,
                Func<S, A, S> folder) =>
                ma.MapIO(io => io.Fold(schedule, initialState, folder));

            static K<Supertype, S> MonadIO<Supertype>.FoldIO<S, A>(
                K<Supertype, A> ma,
                S initialState,
                Func<S, A, S> folder) =>
                ma.MapIO(io => io.Fold(initialState, folder));

            static K<Supertype, S> MonadIO<Supertype>.FoldWhileIO<S, A>(
                K<Supertype, A> ma,
                Schedule schedule,
                S initialState,
                Func<S, A, S> folder,
                Func<S, bool> stateIs) =>
                ma.MapIO(io => io.FoldWhile(schedule, initialState, folder, stateIs));

            static K<Supertype, S> MonadIO<Supertype>.FoldWhileIO<S, A>(
                K<Supertype, A> ma,
                S initialState,
                Func<S, A, S> folder,
                Func<S, bool> stateIs) =>
                ma.MapIO(io => io.FoldWhile(initialState, folder, stateIs));

            static K<Supertype, S> MonadIO<Supertype>.FoldWhileIO<S, A>(
                K<Supertype, A> ma,
                Schedule schedule,
                S initialState,
                Func<S, A, S> folder,
                Func<A, bool> valueIs) =>
                ma.MapIO(io => io.FoldWhile(schedule, initialState, folder, valueIs));

            static K<Supertype, S> MonadIO<Supertype>.FoldWhileIO<S, A>(
                K<Supertype, A> ma,
                S initialState,
                Func<S, A, S> folder,
                Func<A, bool> valueIs) =>
                ma.MapIO(io => io.FoldWhile(initialState, folder, valueIs));

            static K<Supertype, S> MonadIO<Supertype>.FoldWhileIO<S, A>(
                K<Supertype, A> ma,
                Schedule schedule,
                S initialState,
                Func<S, A, S> folder,
                Func<(S State, A Value), bool> predicate) =>
                ma.MapIO(io => io.FoldWhile(schedule, initialState, folder, predicate));

            static K<Supertype, S> MonadIO<Supertype>.FoldWhileIO<S, A>(
                K<Supertype, A> ma,
                S initialState,
                Func<S, A, S> folder,
                Func<(S State, A Value), bool> predicate) =>
                ma.MapIO(io => io.FoldWhile(initialState, folder, predicate));

            static K<Supertype, S> MonadIO<Supertype>.FoldUntilIO<S, A>(
                K<Supertype, A> ma,
                Schedule schedule,
                S initialState,
                Func<S, A, S> folder,
                Func<S, bool> stateIs) =>
                ma.MapIO(io => io.FoldUntil(schedule, initialState, folder, stateIs));

            static K<Supertype, S> MonadIO<Supertype>.FoldUntilIO<S, A>(
                K<Supertype, A> ma,
                S initialState,
                Func<S, A, S> folder,
                Func<S, bool> stateIs) =>
                ma.MapIO(io => io.FoldUntil(initialState, folder, stateIs));

            static K<Supertype, S> MonadIO<Supertype>.FoldUntilIO<S, A>(
                K<Supertype, A> ma,
                Schedule schedule,
                S initialState,
                Func<S, A, S> folder,
                Func<A, bool> valueIs) =>
                ma.MapIO(io => io.FoldUntil(schedule, initialState, folder, valueIs));

            static K<Supertype, S> MonadIO<Supertype>.FoldUntilIO<S, A>(
                K<Supertype, A> ma,
                S initialState,
                Func<S, A, S> folder,
                Func<A, bool> valueIs) =>
                ma.MapIO(io => io.FoldUntil(initialState, folder, valueIs));

            static K<Supertype, S> MonadIO<Supertype>.FoldUntilIO<S, A>(
                K<Supertype, A> ma,
                S initialState,
                Func<S, A, S> folder,
                Func<(S State, A Value), bool> predicate) =>
                ma.MapIO(io => io.FoldUntil(initialState, folder, predicate));

            static K<Supertype, S> MonadIO<Supertype>.FoldUntilIO<S, A>(
                K<Supertype, A> ma,
                Schedule schedule,
                S initialState,
                Func<S, A, S> folder,
                Func<(S State, A Value), bool> predicate) =>
                ma.MapIO(io => io.FoldUntil(schedule, initialState, folder, predicate));

            static K<Supertype, C> MonadIO<Supertype>.SelectMany<A, B, C>(
                K<Supertype, A> ma, Func<A, IO<B>> bind, Func<A, B, C> project) =>
                ma.Bind(x => bind(x) switch
                             {
                                 IOTail<B> tail when typeof(B) == typeof(C) => (IO<C>)(object)tail,
                                 IOTail<B> => throw new NotSupportedException(
                                                  "Tail calls can't transform in the `select`"),
                                 var mb => mb.Map(y => project(x, y))
                             });

            static K<Supertype, C> MonadIO<Supertype>.SelectMany<A, B, C>(
                IO<A> ma, Func<A, K<Supertype, B>> bind, Func<A, B, C> project) =>
                Supertype.SelectMany(Supertype.LiftIO(ma), bind, project);

            static K<Supertype, EnvIO> MonadIO<Supertype>.EnvIO =>
                Supertype.LiftIO(IO.env);

            static K<Supertype, CancellationToken> MonadIO<Supertype>.Token =>
                Supertype.LiftIO(IO.token);

            static K<Supertype, CancellationTokenSource> MonadIO<Supertype>.TokenSource =>
                Supertype.LiftIO(IO.source);

            static K<Supertype, Option<SynchronizationContext>> MonadIO<Supertype>.SyncContext =>
                Supertype.LiftIO(IO.syncContext);
        }
    }
}
