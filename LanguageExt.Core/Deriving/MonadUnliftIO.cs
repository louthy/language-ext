using System;
using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class Deriving
{
    /// <summary>
    /// Derived `MonadUnliftIO` implementation
    /// </summary>
    /// <typeparam name="Supertype">Super-type wrapper around the subtype</typeparam>
    /// <typeparam name="Subtype">The subtype that the supertype type 'wraps'</typeparam>
    public interface MonadUnliftIO<Supertype, Subtype> : 
        MonadIO<Supertype, Subtype>,
        MonadUnliftIO<Supertype>
        where Subtype : MonadUnliftIO<Subtype>, MonadIO<Subtype>
        where Supertype : MonadUnliftIO<Supertype, Subtype>, Monad<Supertype>
    {
        /// <summary>
        /// Extract the inner `IO` monad from the `Self` structure provided
        /// </summary>
        /// <param name="ma">`Self` structure to extract the `IO` monad from</param>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <returns>`Self` structure with the `IO` structure as the bound value</returns>
        static K<Supertype, IO<A>> Traits.Maybe.MonadUnliftIO<Supertype>.ToIO<A>(K<Supertype, A> ma) =>
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
        /// the inner monad or any monad in the stack on the way to the inner-monad
        /// then it will throw an exception.</exception>
        static K<Supertype, B> Traits.Maybe.MonadUnliftIO<Supertype>.MapIO<A, B>(K<Supertype, A> ma, Func<IO<A>, IO<B>> f) =>
            Supertype.CoTransform(Subtype.MapIO(Supertype.Transform(ma), f));        
        
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
        static K<Supertype, A> MonadUnliftIO<Supertype>.LocalIO<A>(K<Supertype, A> ma) =>
            Supertype.CoTransform(Subtype.LocalIO(Supertype.Transform(ma)));

        /// <summary>
        /// Make this IO computation run on the `SynchronizationContext` that was captured at the start
        /// of the IO chain (i.e. the one embedded within the `EnvIO` environment that is passed through
        /// all IO computations)
        /// </summary>
        static K<Supertype, A> MonadUnliftIO<Supertype>.PostIO<A>(K<Supertype, A> ma) =>
            Supertype.CoTransform(Subtype.PostIO(Supertype.Transform(ma)));

        /// <summary>
        /// Timeout operation if it takes too long
        /// </summary>
        static K<Supertype, A> MonadUnliftIO<Supertype>.TimeoutIO<A>(K<Supertype, A> ma, TimeSpan timeout) =>
            Supertype.CoTransform(Subtype.TimeoutIO(Supertype.Transform(ma), timeout));

        /// <summary>
        /// The IO monad tracks resources automatically; this creates a local resource environment
        /// to run this computation in.  Once the computation is completed, any resources acquired
        /// are automatically released.  Imagine this as the ultimate `using` statement.
        /// </summary>
        static K<Supertype, A> MonadUnliftIO<Supertype>.BracketIO<A>(K<Supertype, A> ma) =>
            Supertype.CoTransform(Subtype.BracketIO(Supertype.Transform(ma)));

        /// <summary>
        /// When acquiring, using, and releasing various resources, it can be quite convenient to write a function to manage
        /// the acquisition and releasing, taking a function of the acquired value that specifies an action to be performed
        /// in between.
        /// </summary>
        /// <param name="Acq">Resource acquisition</param>
        /// <param name="Use">Function to use the acquired resource</param>
        /// <param name="Fin">Function to invoke to release the resource</param>
        static K<Supertype, C> MonadUnliftIO<Supertype>.BracketIO<A, B, C>(
            K<Supertype, A> Acq,
            Func<A, IO<C>> Use,
            Func<A, IO<B>> Fin) =>
            Supertype.CoTransform(Subtype.BracketIO(Supertype.Transform(Acq), Use, Fin));

        /// <summary>
        /// When acquiring, using, and releasing various resources, it can be quite convenient to write a function to manage
        /// the acquisition and releasing, taking a function of the acquired value that specifies an action to be performed
        /// in between.
        /// </summary>
        /// <param name="Acq">Resource acquisition</param>
        /// <param name="Use">Function to use the acquired resource</param>
        /// <param name="Catch">Function to run to handle any exceptions</param>
        /// <param name="Fin">Function to invoke to release the resource</param>
        static K<Supertype, C> MonadUnliftIO<Supertype>.BracketIO<A, B, C>(
            K<Supertype, A> Acq,
            Func<A, IO<C>> Use,
            Func<Error, IO<C>> Catch,
            Func<A, IO<B>> Fin) =>
            Supertype.CoTransform(Subtype.BracketIO(Supertype.Transform(Acq), Use, Catch, Fin));

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
        static K<Supertype, A> MonadUnliftIO<Supertype>.RepeatIO<A>(K<Supertype, A> ma) =>
            Supertype.CoTransform(Subtype.RepeatIO(Supertype.Transform(ma)));

        /// <summary>
        /// Keeps repeating the computation, until the scheduler expires, or an error occurs  
        /// </summary>
        /// <remarks>
        /// Any resources acquired within a repeated IO computation will automatically be released.  This also means you can't
        /// acquire resources and return them from within a repeated computation.
        /// </remarks>
        /// <param name="schedule">Scheduler strategy for repeating</param>
        /// <returns>The result of the last invocation</returns>
        static K<Supertype, A> MonadUnliftIO<Supertype>.RepeatIO<A>(
            K<Supertype, A> ma,
            Schedule schedule) =>
            Supertype.CoTransform(Subtype.RepeatIO(Supertype.Transform(ma), schedule));

        /// <summary>
        /// Keeps repeating the computation until the predicate returns false, or an error occurs 
        /// </summary>
        /// <remarks>
        /// Any resources acquired within a repeated IO computation will automatically be released.  This also means you can't
        /// acquire resources and return them from within a repeated computation.
        /// </remarks>
        /// <param name="predicate">Keep repeating while this predicate returns `true` for each computed value</param>
        /// <returns>The result of the last invocation</returns>
        static K<Supertype, A> MonadUnliftIO<Supertype>.RepeatWhileIO<A>(
            K<Supertype, A> ma,
            Func<A, bool> predicate) =>
            Supertype.CoTransform(Subtype.RepeatWhileIO(Supertype.Transform(ma), predicate));

        /// <summary>
        /// Keeps repeating the computation until the scheduler expires, or the predicate returns false, or an error occurs
        /// </summary>
        /// <remarks>
        /// Any resources acquired within a repeated IO computation will automatically be released.  This also means you can't
        /// acquire resources and return them from within a repeated computation.
        /// </remarks>
        /// <param name="schedule">Scheduler strategy for repeating</param>
        /// <param name="predicate">Keep repeating while this predicate returns `true` for each computed value</param>
        /// <returns>The result of the last invocation</returns>
        static K<Supertype, A> MonadUnliftIO<Supertype>.RepeatWhileIO<A>(
            K<Supertype, A> ma,
            Schedule schedule,
            Func<A, bool> predicate) =>
            Supertype.CoTransform(Subtype.RepeatWhileIO(Supertype.Transform(ma), schedule, predicate));

        /// <summary>
        /// Keeps repeating the computation until the predicate returns true, or an error occurs
        /// </summary>
        /// <remarks>
        /// Any resources acquired within a repeated IO computation will automatically be released.  This also means you can't
        /// acquire resources and return them from within a repeated computation.
        /// </remarks>
        /// <param name="predicate">Keep repeating until this predicate returns `true` for each computed value</param>
        /// <returns>The result of the last invocation</returns>
        static K<Supertype, A> MonadUnliftIO<Supertype>.RepeatUntilIO<A>(
            K<Supertype, A> ma,
            Func<A, bool> predicate) =>
            Supertype.CoTransform(Subtype.RepeatUntilIO(Supertype.Transform(ma), predicate));

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
        static K<Supertype, A> MonadUnliftIO<Supertype>.RepeatUntilIO<A>(
            K<Supertype, A> ma,
            Schedule schedule,
            Func<A, bool> predicate) =>
            Supertype.CoTransform(Subtype.RepeatUntilIO(Supertype.Transform(ma), schedule, predicate));

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
        static K<Supertype, A> MonadUnliftIO<Supertype>.RetryIO<A>(K<Supertype, A> ma) =>
            Supertype.CoTransform(Subtype.RetryIO(Supertype.Transform(ma)));

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
        static K<Supertype, A> MonadUnliftIO<Supertype>.RetryIO<A>(
            K<Supertype, A> ma,
            Schedule schedule) =>
            Supertype.CoTransform(Subtype.RetryIO(Supertype.Transform(ma), schedule));

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
        static K<Supertype, A> MonadUnliftIO<Supertype>.RetryWhileIO<A>(
            K<Supertype, A> ma,
            Func<Error, bool> predicate) =>
            Supertype.CoTransform(Subtype.RetryWhileIO(Supertype.Transform(ma), predicate));

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
        static K<Supertype, A> MonadUnliftIO<Supertype>.RetryWhileIO<A>(
            K<Supertype, A> ma,
            Schedule schedule,
            Func<Error, bool> predicate) =>
            Supertype.CoTransform(Subtype.RetryWhileIO(Supertype.Transform(ma), schedule, predicate));

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
        static K<Supertype, A> MonadUnliftIO<Supertype>.RetryUntilIO<A>(
            K<Supertype, A> ma,
            Func<Error, bool> predicate) =>
            Supertype.CoTransform(Subtype.RetryUntilIO(Supertype.Transform(ma), predicate));

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
        static K<Supertype, A> MonadUnliftIO<Supertype>.RetryUntilIO<A>(
            K<Supertype, A> ma,
            Schedule schedule,
            Func<Error, bool> predicate) =>
            Supertype.CoTransform(Subtype.RetryUntilIO(Supertype.Transform(ma), schedule, predicate));

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // 
        //  Folding
        //

        static K<Supertype, S> MonadUnliftIO<Supertype>.FoldIO<S, A>(
            K<Supertype, A> ma,
            Schedule schedule,
            S initialState,
            Func<S, A, S> folder) =>
            Supertype.CoTransform(Subtype.FoldIO(Supertype.Transform(ma), schedule, initialState, folder));

        static K<Supertype, S> MonadUnliftIO<Supertype>.FoldIO<S, A>(
            K<Supertype, A> ma,
            S initialState,
            Func<S, A, S> folder) =>
            Supertype.CoTransform(Subtype.FoldIO(Supertype.Transform(ma), initialState, folder));

        static K<Supertype, S> MonadUnliftIO<Supertype>.FoldWhileIO<S, A>(
            K<Supertype, A> ma,
            Schedule schedule,
            S initialState,
            Func<S, A, S> folder,
            Func<S, bool> stateIs) =>
            Supertype.CoTransform(Subtype.FoldWhileIO(Supertype.Transform(ma), schedule, initialState, folder, stateIs));

        static K<Supertype, S> MonadUnliftIO<Supertype>.FoldWhileIO<S, A>(
            K<Supertype, A> ma,
            S initialState,
            Func<S, A, S> folder,
            Func<S, bool> stateIs) =>
            Supertype.CoTransform(Subtype.FoldWhileIO(Supertype.Transform(ma), initialState, folder, stateIs));

        static K<Supertype, S> MonadUnliftIO<Supertype>.FoldWhileIO<S, A>(
            K<Supertype, A> ma,
            Schedule schedule,
            S initialState,
            Func<S, A, S> folder,
            Func<A, bool> valueIs) =>
            Supertype.CoTransform(Subtype.FoldWhileIO(Supertype.Transform(ma), schedule, initialState, folder, valueIs));

        static K<Supertype, S> MonadUnliftIO<Supertype>.FoldWhileIO<S, A>(
            K<Supertype, A> ma,
            S initialState,
            Func<S, A, S> folder,
            Func<A, bool> valueIs) =>
            Supertype.CoTransform(Subtype.FoldWhileIO(Supertype.Transform(ma), initialState, folder, valueIs));

        static K<Supertype, S> MonadUnliftIO<Supertype>.FoldWhileIO<S, A>(
            K<Supertype, A> ma,
            Schedule schedule,
            S initialState,
            Func<S, A, S> folder,
            Func<(S State, A Value), bool> predicate) =>
            Supertype.CoTransform(Subtype.FoldWhileIO(Supertype.Transform(ma), schedule, initialState, folder, predicate));

        static K<Supertype, S> MonadUnliftIO<Supertype>.FoldWhileIO<S, A>(
            K<Supertype, A> ma,
            S initialState,
            Func<S, A, S> folder,
            Func<(S State, A Value), bool> predicate) =>
            Supertype.CoTransform(Subtype.FoldWhileIO(Supertype.Transform(ma), initialState, folder, predicate));

        static K<Supertype, S> MonadUnliftIO<Supertype>.FoldUntilIO<S, A>(
            K<Supertype, A> ma,
            Schedule schedule,
            S initialState,
            Func<S, A, S> folder,
            Func<S, bool> stateIs) =>
            Supertype.CoTransform(Subtype.FoldUntilIO(Supertype.Transform(ma), schedule, initialState, folder, stateIs));

        static K<Supertype, S> MonadUnliftIO<Supertype>.FoldUntilIO<S, A>(
            K<Supertype, A> ma,
            S initialState,
            Func<S, A, S> folder,
            Func<S, bool> stateIs) =>
            Supertype.CoTransform(Subtype.FoldUntilIO(Supertype.Transform(ma), initialState, folder, stateIs));

        static K<Supertype, S> MonadUnliftIO<Supertype>.FoldUntilIO<S, A>(
            K<Supertype, A> ma,
            Schedule schedule,
            S initialState,
            Func<S, A, S> folder,
            Func<A, bool> valueIs) =>
            Supertype.CoTransform(Subtype.FoldUntilIO(Supertype.Transform(ma), schedule, initialState, folder, valueIs));

        static K<Supertype, S> MonadUnliftIO<Supertype>.FoldUntilIO<S, A>(
            K<Supertype, A> ma,
            S initialState,
            Func<S, A, S> folder,
            Func<A, bool> valueIs) =>
            Supertype.CoTransform(Subtype.FoldUntilIO(Supertype.Transform(ma), initialState, folder, valueIs));

        static K<Supertype, S> MonadUnliftIO<Supertype>.FoldUntilIO<S, A>(
            K<Supertype, A> ma,
            S initialState,
            Func<S, A, S> folder,
            Func<(S State, A Value), bool> predicate) =>
            Supertype.CoTransform(Subtype.FoldUntilIO(Supertype.Transform(ma), initialState, folder, predicate));

        static K<Supertype, S> MonadUnliftIO<Supertype>.FoldUntilIO<S, A>(
            K<Supertype, A> ma,
            Schedule schedule,
            S initialState,
            Func<S, A, S> folder,
            Func<(S State, A Value), bool> predicate) =>
            Supertype.CoTransform(Subtype.FoldUntilIO(Supertype.Transform(ma), schedule, initialState, folder, predicate));
    }
}
