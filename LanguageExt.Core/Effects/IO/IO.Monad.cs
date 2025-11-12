using System;
using System.Collections.Generic;
using System.Linq;
using LanguageExt.Common;
using LanguageExt.DSL;
using LanguageExt.Traits;

namespace LanguageExt;

public partial class IO : 
    MonadUnliftIO<IO>,
    Final<IO>,
    Fallible<IO>,
    Alternative<IO>
{
    static K<IO, B> Applicative<IO>.Apply<A, B>(K<IO, Func<A, B>> mf, K<IO, A> ma) =>
        (mf, ma) switch
        {
            (_, IOEmpty<A>)          => IOEmpty<B>.Default,
            (IOEmpty<Func<A, B>>, _) => IOEmpty<B>.Default,
            _                        => ma.As().ApplyBack(mf.As()),
        };

    static K<IO, B> Applicative<IO>.Action<A, B>(K<IO, A> ma, K<IO, B> mb) =>
        (ma, mb) switch
        {
            (IOEmpty<A>, IOEmpty<B>) => IOEmpty<B>.Default,
            (IOEmpty<B>, var my)     => my,
            _                        => new IOAction<A, B, B>(ma, mb, pure),
        };

    static K<IO, A> Applicative<IO>.Actions<A>(IEnumerable<K<IO, A>> fas) =>
        new IOActions<A, A>(fas.Where(fa => fa is not IOEmpty<A>).GetIterator(), pure);

    static K<IO, A> Applicative<IO>.Actions<A>(IAsyncEnumerable<K<IO, A>> fas) => 
        new IOAsyncActions<A, A>(fas.FilterAsync(fa => fa is not IOEmpty<A>).GetIteratorAsync(), pure);

    static K<IO, B> Monad<IO>.Bind<A, B>(K<IO, A> ma, Func<A, K<IO, B>> f) =>
        ma is IOEmpty<A>
            ? IOEmpty<B>.Default
            : ma.As().Bind(f);

    static K<IO, B> Functor<IO>.Map<A, B>(Func<A, B> f, K<IO, A> ma) => 
        ma is IOEmpty<A>
            ? IOEmpty<B>.Default
            : ma.As().Map(f);

    static K<IO, A> Applicative<IO>.Pure<A>(A value) =>
        new IOPure<A>(value);

    static K<IO, A> Fallible<Error, IO>.Fail<A>(Error error) => 
        fail<A>(error);

    static K<IO, A> Fallible<Error, IO>.Catch<A>(
        K<IO, A> fa, 
        Func<Error, bool> Predicate,
        Func<Error, K<IO, A>> Fail) =>
        fa is IOEmpty<A>
            ? fa
            : new IOCatch<A, A>(fa, Predicate, Fail, null, pure);

    static K<IO, A> Choice<IO>.Choose<A>(K<IO, A> fa, K<IO, A> fb) =>
        fa is IOEmpty<A>
            ? fb
            : new IOCatch<A, A>(fa, _ => true, _ => fb, null, pure);

    static K<IO, A> Choice<IO>.Choose<A>(K<IO, A> fa, Func<K<IO, A>> fb) => 
        fa is IOEmpty<A>
            ? fb()
            : new IOCatch<A, A>(fa, _ => true, _ => fb(), null, pure);

    static K<IO, A> SemigroupK<IO>.Combine<A>(K<IO, A> lhs, K<IO, A> rhs) =>
        lhs.Choose(rhs);

    static K<IO, A> MonoidK<IO>.Empty<A>() =>
        empty<A>();

    static K<IO, A> MonadIO<IO>.LiftIO<A>(IO<A> ma) => 
        ma;

    static K<IO, B> MonadUnliftIO<IO>.MapIO<A, B>(K<IO, A> ma, Func<IO<A>, IO<B>> f) =>
        ma is IOEmpty<A>
            ? IOEmpty<B>.Default
            : f(ma.As());

    static K<IO, IO<A>> MonadUnliftIO<IO>.ToIO<A>(K<IO, A> ma) =>
        pure(ma.As());

    static K<IO, A> Final<IO>.Finally<X, A>(K<IO, A> fa, K<IO, X> @finally) =>
        fa is IOEmpty<A>
            ? fa
            : new IOFinal<X, A, A>(fa, @finally, pure);
    
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
    static K<IO, A> MonadUnliftIO<IO>.LocalIO<A>(K<IO, A> ma) =>
        ma is IOEmpty<A>
            ? ma
            : ma.As().Local();

    /// <summary>
    /// Make this IO computation run on the `SynchronizationContext` that was captured at the start
    /// of the IO chain (i.e. the one embedded within the `EnvIO` environment that is passed through
    /// all IO computations)
    /// </summary>
    static K<IO, A> MonadUnliftIO<IO>.PostIO<A>(K<IO, A> ma) =>
        ma is IOEmpty<A>
            ? ma
            : ma.As().Post();        

    /// <summary>
    /// Await a forked operation
    /// </summary>
    static K<IO, A> MonadUnliftIO<IO>.Await<A>(K<IO, ForkIO<A>> ma) =>
        ma is IOEmpty<A>
            ? IOEmpty<A>.Default
            : ma.As().Bind(f => f.Await);    

    /// <summary>
    /// Queue this IO operation to run on the thread-pool. 
    /// </summary>
    /// <param name="timeout">Maximum time that the forked IO operation can run for. `None` for no timeout.</param>
    /// <returns>Returns a `ForkIO` data-structure that contains two IO effects that can be used to either cancel
    /// the forked IO operation or to await the result of it.
    /// </returns>
    static K<IO, ForkIO<A>> MonadUnliftIO<IO>.ForkIO<A>(K<IO, A> ma, Option<TimeSpan> timeout) =>
        ma is IOEmpty<A>
            ? IOEmpty<ForkIO<A>>.Default
            : ma.As().Fork(timeout);

    /// <summary>
    /// Timeout operation if it takes too long
    /// </summary>
    static K<IO, A> MonadUnliftIO<IO>.TimeoutIO<A>(K<IO, A> ma, TimeSpan timeout) =>
        ma is IOEmpty<A>
            ? IOEmpty<A>.Default
            : ma.As().Timeout(timeout);

    /// <summary>
    /// The IO monad tracks resources automatically, this creates a local resource environment
    /// to run this computation in.  Once the computation has completed any resources acquired
    /// are automatically released.  Imagine this as the ultimate `using` statement.
    /// </summary>
    static K<IO, A> MonadUnliftIO<IO>.BracketIO<A>(K<IO, A> ma) =>
        ma is IOEmpty<A>
            ? IOEmpty<A>.Default
            : ma.As().Bracket();

    /// <summary>
    /// When acquiring, using, and releasing various resources, it can be quite convenient to write a function to manage
    /// the acquisition and releasing, taking a function of the acquired value that specifies an action to be performed
    /// in between.
    /// </summary>
    /// <param name="Acq">Resource acquisition</param>
    /// <param name="Use">Function to use the acquired resource</param>
    /// <param name="Fin">Function to invoke to release the resource</param>
    static K<IO, C> MonadUnliftIO<IO>.BracketIO<A, B, C>(
        K<IO, A> Acq,
        Func<A, IO<C>> Use,
        Func<A, IO<B>> Fin) =>
        Acq is IOEmpty<A>
            ? IOEmpty<C>.Default
            : Acq.As().Bracket(Use, Fin);

    /// <summary>
    /// When acquiring, using, and releasing various resources, it can be quite convenient to write a function to manage
    /// the acquisition and releasing, taking a function of the acquired value that specifies an action to be performed
    /// in between.
    /// </summary>
    /// <param name="Acq">Resource acquisition</param>
    /// <param name="Use">Function to use the acquired resource</param>
    /// <param name="Catch">Function to run to handle any exceptions</param>
    /// <param name="Fin">Function to invoke to release the resource</param>
    static K<IO, C> MonadUnliftIO<IO>.BracketIO<A, B, C>(
        K<IO, A> Acq,
        Func<A, IO<C>> Use,
        Func<Error, IO<C>> Catch,
        Func<A, IO<B>> Fin) =>
        Acq is IOEmpty<A>
            ? IOEmpty<C>.Default
            : Acq.As().Bracket(Use, Catch, Fin);

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
    static K<IO, A> MonadUnliftIO<IO>.RepeatIO<A>(K<IO, A> ma) =>
        ma is IOEmpty<A>
            ? IOEmpty<A>.Default
            : ma.As().Repeat();

    /// <summary>
    /// Keeps repeating the computation, until the scheduler expires, or an error occurs  
    /// </summary>
    /// <remarks>
    /// Any resources acquired within a repeated IO computation will automatically be released.  This also means you can't
    /// acquire resources and return them from within a repeated computation.
    /// </remarks>
    /// <param name="schedule">Scheduler strategy for repeating</param>
    /// <returns>The result of the last invocation</returns>
    static K<IO, A> MonadUnliftIO<IO>.RepeatIO<A>(
        K<IO, A> ma,
        Schedule schedule) =>
        ma is IOEmpty<A>
            ? IOEmpty<A>.Default
            : ma.As().Repeat(schedule);

    /// <summary>
    /// Keeps repeating the computation until the predicate returns false, or an error occurs 
    /// </summary>
    /// <remarks>
    /// Any resources acquired within a repeated IO computation will automatically be released.  This also means you can't
    /// acquire resources and return them from within a repeated computation.
    /// </remarks>
    /// <param name="predicate">Keep repeating while this predicate returns `true` for each computed value</param>
    /// <returns>The result of the last invocation</returns>
    static K<IO, A> MonadUnliftIO<IO>.RepeatWhileIO<A>(
        K<IO, A> ma,
        Func<A, bool> predicate) =>
        ma is IOEmpty<A>
            ? IOEmpty<A>.Default
            : ma.As().RepeatWhile(predicate);

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
    static K<IO, A> MonadUnliftIO<IO>.RepeatWhileIO<A>(
        K<IO, A> ma,
        Schedule schedule,
        Func<A, bool> predicate) =>
        ma is IOEmpty<A>
            ? IOEmpty<A>.Default
            : ma.As().RepeatWhile(schedule, predicate);

    /// <summary>
    /// Keeps repeating the computation until the predicate returns true, or an error occurs
    /// </summary>
    /// <remarks>
    /// Any resources acquired within a repeated IO computation will automatically be released.  This also means you can't
    /// acquire resources and return them from within a repeated computation.
    /// </remarks>
    /// <param name="predicate">Keep repeating until this predicate returns `true` for each computed value</param>
    /// <returns>The result of the last invocation</returns>
    static K<IO, A> MonadUnliftIO<IO>.RepeatUntilIO<A>(
        K<IO, A> ma,
        Func<A, bool> predicate) =>
        ma is IOEmpty<A>
            ? IOEmpty<A>.Default
            : ma.As().RepeatUntil(predicate);

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
    static K<IO, A> MonadUnliftIO<IO>.RepeatUntilIO<A>(
        K<IO, A> ma,
        Schedule schedule,
        Func<A, bool> predicate) =>
        ma is IOEmpty<A>
            ? IOEmpty<A>.Default
            : ma.As().RepeatUntil(schedule, predicate);

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
    static K<IO, A> MonadUnliftIO<IO>.RetryIO<A>(K<IO, A> ma) =>
        ma is IOEmpty<A>
            ? IOEmpty<A>.Default
            : ma.As().Retry();

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
    static K<IO, A> MonadUnliftIO<IO>.RetryIO<A>(
        K<IO, A> ma,
        Schedule schedule) =>
        ma is IOEmpty<A>
            ? IOEmpty<A>.Default
            : ma.As().Retry(schedule);

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
    static K<IO, A> MonadUnliftIO<IO>.RetryWhileIO<A>(
        K<IO, A> ma,
        Func<Error, bool> predicate) =>
        ma is IOEmpty<A>
            ? IOEmpty<A>.Default
            : ma.As().RetryWhile(predicate);

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
    static K<IO, A> MonadUnliftIO<IO>.RetryWhileIO<A>(
        K<IO, A> ma,
        Schedule schedule,
        Func<Error, bool> predicate) =>
        ma is IOEmpty<A>
            ? IOEmpty<A>.Default
            : ma.As().RetryWhile(schedule, predicate);

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
    static K<IO, A> MonadUnliftIO<IO>.RetryUntilIO<A>(
        K<IO, A> ma,
        Func<Error, bool> predicate) =>
        ma is IOEmpty<A>
            ? IOEmpty<A>.Default
            : ma.As().RetryUntil(predicate);

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
    static K<IO, A> MonadUnliftIO<IO>.RetryUntilIO<A>(
        K<IO, A> ma,
        Schedule schedule,
        Func<Error, bool> predicate) =>
        ma is IOEmpty<A>
            ? IOEmpty<A>.Default
            : ma.As().RetryUntil(schedule, predicate);
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // 
    //  Folding
    //

    static K<IO, S> MonadUnliftIO<IO>.FoldIO<S, A>(
        K<IO, A> ma,
        Schedule schedule,
        S initialState,
        Func<S, A, S> folder) =>
        ma is IOEmpty<A>
            ? IOEmpty<S>.Default
            : ma.As().Fold(schedule, initialState, folder);

    static K<IO, S> MonadUnliftIO<IO>.FoldIO<S, A>(
        K<IO, A> ma,
        S initialState,
        Func<S, A, S> folder) =>
        ma is IOEmpty<A>
            ? IOEmpty<S>.Default
            : ma.As().Fold(initialState, folder);

    static K<IO, S> MonadUnliftIO<IO>.FoldWhileIO<S, A>(
        K<IO, A> ma,
        Schedule schedule,
        S initialState,
        Func<S, A, S> folder,
        Func<S, bool> stateIs) =>
        ma is IOEmpty<A>
            ? IOEmpty<S>.Default
            : ma.As().FoldWhile(schedule, initialState, folder, stateIs);

    static K<IO, S> MonadUnliftIO<IO>.FoldWhileIO<S, A>(
        K<IO, A> ma,
        S initialState,
        Func<S, A, S> folder,
        Func<S, bool> stateIs) =>
        ma is IOEmpty<A>
            ? IOEmpty<S>.Default
            : ma.As().FoldWhile(initialState, folder, stateIs);

    static K<IO, S> MonadUnliftIO<IO>.FoldWhileIO<S, A>(
        K<IO, A> ma,
        Schedule schedule,
        S initialState,
        Func<S, A, S> folder,
        Func<A, bool> valueIs) =>
        ma is IOEmpty<A>
            ? IOEmpty<S>.Default
            : ma.As().FoldWhile(schedule, initialState, folder, valueIs);

    static K<IO, S> MonadUnliftIO<IO>.FoldWhileIO<S, A>(
        K<IO, A> ma,
        S initialState,
        Func<S, A, S> folder,
        Func<A, bool> valueIs) =>
        ma is IOEmpty<A>
            ? IOEmpty<S>.Default
            : ma.As().FoldWhile(initialState, folder, valueIs);
    
    static K<IO, S> MonadUnliftIO<IO>.FoldWhileIO<S, A>(
        K<IO, A> ma,
        Schedule schedule,
        S initialState,
        Func<S, A, S> folder,
        Func<(S State, A Value), bool> predicate) =>
        ma is IOEmpty<A>
            ? IOEmpty<S>.Default
            : ma.As().FoldWhile(schedule, initialState, folder, predicate);

    static K<IO, S> MonadUnliftIO<IO>.FoldWhileIO<S, A>(
        K<IO, A> ma,
        S initialState,
        Func<S, A, S> folder,
        Func<(S State, A Value), bool> predicate) =>
        ma is IOEmpty<A>
            ? IOEmpty<S>.Default
            : ma.As().FoldWhile(initialState, folder, predicate);
    
    static K<IO, S> MonadUnliftIO<IO>.FoldUntilIO<S, A>(
        K<IO, A> ma,
        Schedule schedule,
        S initialState,
        Func<S, A, S> folder,
        Func<S, bool> stateIs) =>
        ma is IOEmpty<A>
            ? IOEmpty<S>.Default
            : ma.As().FoldUntil(schedule, initialState, folder, stateIs);
    
    static K<IO, S> MonadUnliftIO<IO>.FoldUntilIO<S, A>(
        K<IO, A> ma,
        S initialState,
        Func<S, A, S> folder,
        Func<S, bool> stateIs) =>
        ma is IOEmpty<A>
            ? IOEmpty<S>.Default
            : ma.As().FoldUntil(initialState, folder, stateIs);
    
    static K<IO, S> MonadUnliftIO<IO>.FoldUntilIO<S, A>(
        K<IO, A> ma,
        Schedule schedule,
        S initialState,
        Func<S, A, S> folder,
        Func<A, bool> valueIs) =>
        ma is IOEmpty<A>
            ? IOEmpty<S>.Default
            : ma.As().FoldUntil(schedule, initialState, folder, valueIs);
    
    static K<IO, S> MonadUnliftIO<IO>.FoldUntilIO<S, A>(
        K<IO, A> ma,
        S initialState,
        Func<S, A, S> folder,
        Func<A, bool> valueIs) =>
        ma is IOEmpty<A>
            ? IOEmpty<S>.Default
            : ma.As().FoldUntil(initialState, folder, valueIs);
    
    static K<IO, S> MonadUnliftIO<IO>.FoldUntilIO<S, A>(
        K<IO, A> ma,
        S initialState,
        Func<S, A, S> folder,
        Func<(S State, A Value), bool> predicate) =>
        ma is IOEmpty<A>
            ? IOEmpty<S>.Default
            : ma.As().FoldUntil(initialState, folder, predicate);

    static K<IO, S> MonadUnliftIO<IO>.FoldUntilIO<S, A>(
        K<IO, A> ma,
        Schedule schedule,
        S initialState,
        Func<S, A, S> folder,
        Func<(S State, A Value), bool> predicate) =>
        ma is IOEmpty<A>
            ? IOEmpty<S>.Default
            : ma.As().FoldUntil(schedule, initialState, folder, predicate);       
}
