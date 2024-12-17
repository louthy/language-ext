using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt;

/// <summary>
/// A value of type `IO` is a computation which, when performed, does some I/O before returning
/// a value of type `A`.
///
/// There is really only one way you should _"perform"_ an I/O action: bind it to `Main` in your
/// program:  When your program is run, the I/O will be performed. It shouldn't be possible to
/// perform I/O from an arbitrary function, unless that function is itself in the `IO` monad and
/// called at some point, directly or indirectly, from `Main`.
///
/// Obviously, as this is C#, the above restrictions are for you to enforce. It would be reasonable
/// to relax that approach and have I/O invoked from, say, web-request handlers - or any other 'edges'
/// of your application.
/// 
/// `IO` is a monad, so `IO` actions can be combined using either the LINQ-notation or the `bind` 
/// operations from the `Monad` class.
/// </summary>
/// <param name="runIO">The lifted thunk that is the IO operation</param>
/// <typeparam name="A">Bound value</typeparam>
public abstract record IO<A> : 
    Fallible<IO<A>, IO, Error, A>,
    Monoid<IO<A>>
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Construction
    //
    
    public static IO<A> Pure(A value) => 
        new IOPure<A>(value);
    
    public static IO<A> Fail(Error value) => 
        new IOFail<A>(value);

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  General
    //
    
    public static IO<A> Empty { get; } = 
        new IOFail<A>(Errors.None);

    internal abstract bool IsAsync { get; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Lifting
    //
    
    public static IO<A> Lift(Func<A> f) =>
        new IOSync<A>(_ => IOResponse.Complete(f()));

    public static IO<A> Lift(Func<IOResponse<A>> f) =>
        new IOSync<A>(_ => f());

    public static IO<A> Lift(Func<EnvIO, A> f) =>
        new IOSync<A>(e => IOResponse.Complete(f(e)));

    public static IO<A> Lift(Func<EnvIO, IOResponse<A>> f) =>
        new IOSync<A>(f);

    public static IO<A> LiftAsync(Func<Task<IOResponse<A>>> f) =>
        new IOAsync<A>(async _ => await f());

    public static IO<A> LiftAsync(Func<EnvIO, Task<IOResponse<A>>> f) =>
        new IOAsync<A>(f);

    public static IO<A> LiftAsync(Func<Task<A>> f) =>
        new IOAsync<A>(async _ => IOResponse.Complete(await f()));

    public static IO<A> LiftAsync(Func<EnvIO, Task<A>> f) =>
        new IOAsync<A>(async env => IOResponse.Complete(await f(env)));

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Functor
    //

    public abstract IO<B> Map<B>(Func<A, B> f);

    public IO<B> Map<B>(B value) =>
        Map(_ => value);

    public IO<A> MapFail(Func<Error, Error> f) => 
        this.Catch(f).As();

    public IO<B> BiMap<B>(Func<A, B> Succ, Func<Error, Error> Fail) =>
        Map(Succ).Catch(Fail).As();

    public IO<B> Match<B>(Func<A, B> Succ, Func<Error, B> Fail) =>
        Map(Succ).Catch(Fail).As();

    public IO<A> IfFail(Func<Error, A> Fail) =>
        this.Catch(Fail).As();

    public IO<A> IfFail(A Fail) =>
        this.Catch(Fail).As();
    
    public IO<A> IfFail(Func<Error, IO<A>> Fail) =>
        this.CatchIO(Fail).As();

    public IO<A> IfFail(IO<A> Fail) =>
        this.Catch(Fail).As();
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // 
    //  Folding
    //

    public IO<S> Fold<S>(
        Schedule schedule,
        S initialState,
        Func<S, A, S> folder) =>
        FoldUntil(schedule, initialState, folder, predicate: _ => false);

    public IO<S> Fold<S>(
        S initialState,
        Func<S, A, S> folder) =>
        FoldUntil(Schedule.Forever, initialState, folder, predicate: _ => false);

    public IO<S> FoldWhile<S>(
        Schedule schedule,
        S initialState,
        Func<S, A, S> folder,
        Func<S, bool> stateIs) =>
        FoldUntil(schedule, initialState, folder, Prelude.not(stateIs));

    public IO<S> FoldWhile<S>(
        S initialState,
        Func<S, A, S> folder,
        Func<S, bool> stateIs) =>
        FoldUntil(Schedule.Forever, initialState, folder, Prelude.not(stateIs));
    
    public IO<S> FoldWhile<S>(
        Schedule schedule,
        S initialState,
        Func<S, A, S> folder,
        Func<A, bool> valueIs) =>
        FoldUntil(schedule, initialState, folder, Prelude.not(valueIs));

    public IO<S> FoldWhile<S>(
        S initialState,
        Func<S, A, S> folder,
        Func<A, bool> valueIs) =>
        FoldUntil(Schedule.Forever, initialState, folder, Prelude.not(valueIs));
    
    public IO<S> FoldWhile<S>(
        Schedule schedule,
        S initialState,
        Func<S, A, S> folder,
        Func<(S State, A Value), bool> predicate) =>
        FoldUntil(schedule, initialState, folder, Prelude.not(predicate));

    public IO<S> FoldWhile<S>(
        S initialState,
        Func<S, A, S> folder,
        Func<(S State, A Value), bool> predicate) =>
        FoldUntil(Schedule.Forever, initialState, folder, Prelude.not(predicate));
    
    public IO<S> FoldUntil<S>(
        Schedule schedule,
        S initialState,
        Func<S, A, S> folder,
        Func<S, bool> stateIs) =>
        FoldUntil(schedule, initialState, folder, p => stateIs(p.State));
    
    public IO<S> FoldUntil<S>(
        S initialState,
        Func<S, A, S> folder,
        Func<S, bool> stateIs) =>
        FoldUntil(Schedule.Forever, initialState, folder, p => stateIs(p.State));
    
    public IO<S> FoldUntil<S>(
        Schedule schedule,
        S initialState,
        Func<S, A, S> folder,
        Func<A, bool> valueIs) =>
        FoldUntil(schedule, initialState, folder, p => valueIs(p.Value));
    
    public IO<S> FoldUntil<S>(
        S initialState,
        Func<S, A, S> folder,
        Func<A, bool> valueIs) =>
        FoldUntil(Schedule.Forever, initialState, folder, p => valueIs(p.Value));
    
    public IO<S> FoldUntil<S>(
        S initialState,
        Func<S, A, S> folder,
        Func<(S State, A Value), bool> predicate) =>
        FoldUntil(Schedule.Forever, initialState, folder, predicate);

    public abstract IO<S> FoldUntil<S>(
        Schedule schedule,
        S initialState,
        Func<S, A, S> folder,
        Func<(S State, A Value), bool> predicate);
    
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Cross thread-context posting
    //
 
    /// <summary>
    /// Make this IO computation run on the `SynchronizationContext` that was captured at the start
    /// of the IO chain (i.e. the one embedded within the `EnvIO` environment that is passed through
    /// all IO computations)
    /// </summary>
    public abstract IO<A> Post();
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Monad
    //

    public abstract IO<B> Bind<B>(Func<A, IO<B>> f);

    public IO<B> Bind<B>(Func<A, K<IO, B>> f) =>
        Bind(x => f(x).As());

    public IO<B> Bind<B>(Func<A, Pure<B>> f) =>
        Map(x => f(x).Value);

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  LINQ
    //
    
    public IO<B> Select<B>(Func<A, B> f) =>
        Map(f);

    public IO<C> SelectMany<B, C>(Func<A, IO<B>> bind, Func<A, B, C> project) =>
        Bind(x => bind(x).Map(y => project(x, y)));

    public IO<C> SelectMany<B, C>(Func<A, K<IO, B>> bind, Func<A, B, C> project) =>
        Bind(x => bind(x).Map(y => project(x, y)));

    public IO<C> SelectMany<B, C>(Func<A, Pure<B>> bind, Func<A, B, C> project) =>
        Bind(x => bind(x).Map(y => project(x, y)));

    public OptionT<M, C> SelectMany<M, B, C>(Func<A, OptionT<M, B>> bind, Func<A, B, C> project)
        where M : Monad<M>, Alternative<M> =>
        OptionT<M, A>.LiftIO(this).SelectMany(bind, project);

    public TryT<M, C> SelectMany<M, B, C>(Func<A, TryT<M, B>> bind, Func<A, B, C> project)
        where M : Monad<M>, Alternative<M> =>
        TryT<M, A>.LiftIO(this).SelectMany(bind, project);

    public EitherT<L, M, C> SelectMany<L, M, B, C>(Func<A, EitherT<L, M, B>> bind, Func<A, B, C> project)
        where M : Monad<M>, Alternative<M> =>
        EitherT<L, M, A>.LiftIO(this).SelectMany(bind, project);

    public FinT<M, C> SelectMany<M, B, C>(Func<A, FinT<M, B>> bind, Func<A, B, C> project)
        where M : Monad<M>, Alternative<M> =>
        FinT<M, A>.LiftIO(this).SelectMany(bind, project);

    public ValidationT<F, M, C> SelectMany<F, M, B, C>(Func<A, ValidationT<F, M, B>> bind, Func<A, B, C> project)
        where F : Monoid<F>
        where M : Monad<M>, Alternative<M> =>
        ValidationT<F, M, A>.LiftIO(this).SelectMany(bind, project);

    public ReaderT<Env, M, C> SelectMany<Env, M, B, C>(Func<A, ReaderT<Env, M, B>> bind, Func<A, B, C> project)
        where M : Monad<M>, Alternative<M> =>
        ReaderT<Env, M, A>.LiftIO(this).SelectMany(bind, project);

    public StateT<S, M, C> SelectMany<S, M, B, C>(Func<A, StateT<S, M, B>> bind, Func<A, B, C> project)
        where M : Monad<M>, Alternative<M> =>
        StateT<S, M, A>.LiftIO(this).SelectMany(bind, project);

    public Eff<C> SelectMany<B, C>(Func<A, Eff<B>> bind, Func<A, B, C> project) =>
        Eff<A>.LiftIO(this).SelectMany(bind, project);

    public Eff<RT, C> SelectMany<RT, B, C>(Func<A, Eff<RT, B>> bind, Func<A, B, C> project) =>
        Eff<RT, A>.LiftIO(this).SelectMany(bind, project);

    public IO<C> SelectMany<C>(Func<A, Guard<Error, Unit>> bind, Func<A, Unit, C> project) =>
        SelectMany(a => bind(a).ToIO(), project);

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Operators
    //
    
    public static IO<A> operator |(IO<A> lhs, IO<A> rhs) =>
        lhs.Choose(rhs).As();

    public static IO<A> operator |(IO<A> lhs, K<IO, A> rhs) => 
        lhs.Choose(rhs).As();

    public static IO<A> operator |(K<IO, A> lhs, IO<A> rhs) => 
        lhs.Choose(rhs).As();

    public static IO<A> operator |(IO<A> lhs, Pure<A> rhs) =>
        lhs.Choose(rhs.ToIO()).As();

    public static IO<A> operator |(IO<A> lhs, Fail<Error> rhs) =>
        lhs.Catch(rhs).As();

    public static IO<A> operator |(IO<A> lhs, CatchM<Error, IO, A> rhs) =>
        lhs.Catch(rhs).As();

    public static IO<A> operator |(IO<A> lhs, A rhs) =>
        lhs.Choose(IO.pure(rhs)).As();

    /// <summary>
    /// Sequentially compose two actions, discarding any value produced by the first, like sequencing operators (such
    /// as the semicolon) in C#.
    /// </summary>
    /// <param name="lhs">First action to run</param>
    /// <param name="rhs">Second action to run</param>
    /// <returns>Result of the second action</returns>
    public static IO<A> operator >> (IO<A> lhs, IO<A> rhs) =>
        lhs.Bind(_ => rhs);
    
    /// <summary>
    /// Sequentially compose two actions, discarding any value produced by the first, like sequencing operators (such
    /// as the semicolon) in C#.
    /// </summary>
    /// <param name="lhs">First action to run</param>
    /// <param name="rhs">Second action to run</param>
    /// <returns>Result of the second action</returns>
    public static IO<A> operator >> (IO<A> lhs, K<IO, A> rhs) =>
        lhs.Bind(_ => rhs);
    
    /// <summary>
    /// Sequentially compose two actions.  The second action is a unit returning action, so the result of the
    /// first action is propagated. 
    /// </summary>
    /// <param name="lhs">First action to run</param>
    /// <param name="rhs">Second action to run</param>
    /// <returns>Result of the first action</returns>
    public static IO<A> operator >> (IO<A> lhs, IO<Unit> rhs) =>
        lhs.Bind(x => rhs.Map(_ => x));
    
    /// <summary>
    /// Sequentially compose two actions.  The second action is a unit returning action, so the result of the
    /// first action is propagated. 
    /// </summary>
    /// <param name="lhs">First action to run</param>
    /// <param name="rhs">Second action to run</param>
    /// <returns>Result of the first action</returns>
    public static IO<A> operator >> (IO<A> lhs, K<IO, Unit> rhs) =>
        lhs.Bind(x => rhs.Map(_ => x));

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
    public abstract IO<A> Bracket();

    /// <summary>
    /// When acquiring, using, and releasing various resources, it can be quite convenient to write a function to manage
    /// the acquisition and releasing, taking a function of the acquired value that specifies an action to be performed
    /// in between.
    /// </summary>
    /// <param name="Use">Function to use the acquired resource</param>
    /// <param name="Fin">Function to invoke to release the resource</param>
    public IO<C> Bracket<B, C>(Func<A, IO<C>> Use, Func<A, IO<B>> Fin) =>
        Bracket(Use, IO<C>.Fail, Fin);

    /// <summary>
    /// When acquiring, using, and releasing various resources, it can be quite convenient to write a function to manage
    /// the acquisition and releasing, taking a function of the acquired value that specifies an action to be performed
    /// in between.
    /// </summary>
    /// <param name="Use">Function to use the acquired resource</param>
    /// <param name="Catch">Function to run to handle any exceptions</param>
    /// <param name="Fin">Function to invoke to release the resource</param>
    public abstract IO<C> Bracket<B, C>(Func<A, IO<C>> Use, Func<Error, IO<C>> Catch, Func<A, IO<B>> Fin);

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Conversion
    //
    
    public static implicit operator IO<A>(Pure<A> ma) =>
        Pure(ma.Value);

    public static implicit operator IO<A>(Error error) =>
        Lift(_ => error.Throw<IOResponse<A>>());

    public static implicit operator IO<A>(Fail<Error> ma) =>
        Lift(() => ma.Value.Throw<A>());

    public static implicit operator IO<A>(Fail<Exception> ma) =>
        Lift(() => ma.Value.Rethrow<A>());

    public static implicit operator IO<A>(Lift<EnvIO, A> ma) =>
        Lift(ma.Function);

    public static implicit operator IO<A>(Lift<A> ma) =>
        Lift(ma.Function);

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Parallel
    //

    /// <summary>
    /// Applies a time limit to the IO computation.  If exceeded an exception is thrown.  
    /// </summary>
    /// <param name="timeout">Timeout</param>
    /// <returns>Result of the operation or throws if the time limit exceeded.</returns>
    public IO<A> Timeout(TimeSpan timeout) =>
        Fork(timeout).Await().As();

    /// <summary>
    /// Create a local cancellation environment
    /// </summary>
    public abstract IO<A> Local();    
    
    /// <summary>
    /// Queues the specified work to run on the thread pool  
    /// </summary>
    /// <remarks>
    /// Any resources acquired within a forked IO computation will automatically be released upon the forked
    /// computation's completion (successful or otherwise).  Resources acquired in the parent thread will be
    /// available to the forked thread, and can be released from there, but they are shared resources at that
    /// point and should be treated with care.
    /// </remarks>
    /// <param name="timeout">Optional timeout</param>
    /// <returns>`Fork` record that contains members for cancellation and optional awaiting</returns>
    public abstract IO<ForkIO<A>> Fork(Option<TimeSpan> timeout = default);

    /// <summary>
    /// Run the `IO` monad to get its result
    /// </summary>
    /// <remarks>
    /// This forks the operation to run on a new task that is then awaited.
    /// 
    /// Any lifted asynchronous operations will yield to the thread-scheduler, allowing other queued
    /// operations to run concurrently.  So, even though this call isn't awaitable it still plays
    /// nicely and doesn't block the thread.
    /// </remarks>
    /// <remarks>
    /// NOTE: An exception will always be thrown if the IO operation fails.  Lift this monad into
    /// other error handling monads to leverage more declarative error handling. 
    /// </remarks>
    /// <returns>Result of the IO operation</returns>
    /// <exception cref="TaskCanceledException">Throws if the operation is cancelled</exception>
    /// <exception cref="BottomException">Throws if any lifted task fails without a value `Exception` value.</exception>
    public abstract ValueTask<A> RunAsync(EnvIO? envIO = null);

    /// <summary>
    /// Run the `IO` monad to get its result
    /// </summary>
    /// <remarks>
    /// Any lifted asynchronous operations will yield to the thread-scheduler, allowing other queued
    /// operations to run concurrently.  So, even though this call isn't awaitable it still plays
    /// nicely and doesn't block the thread.
    /// </remarks>
    /// <remarks>
    /// NOTE: An exception will always be thrown if the IO operation fails.  Lift this monad into
    /// other error handling monads to leverage more declarative error handling. 
    /// </remarks>
    /// <param name="env">IO environment</param>
    /// <returns>Result of the IO operation</returns>
    /// <exception cref="TaskCanceledException">Throws if the operation is cancelled</exception>
    /// <exception cref="BottomException">Throws if any lifted task fails without a value `Exception` value.</exception>
    public abstract A Run(EnvIO? envIO = null);
    
    /// <summary>
    /// Run the `IO` monad to get its result.  Differs from `Run` in that it catches any exceptions and turns
    /// them into a `Fin〈A〉` result. 
    /// </summary>
    public FinT<IO, A> Try() =>
        new (Map(Fin<A>.Succ).Catch(Fin<A>.Fail));
    
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
    public IO<A> Repeat() =>
        RepeatUntil(_ => false);

    /// <summary>
    /// Keeps repeating the computation, until the scheduler expires, or an error occurs  
    /// </summary>
    /// <remarks>
    /// Any resources acquired within a repeated IO computation will automatically be released.  This also means you can't
    /// acquire resources and return them from within a repeated computation.
    /// </remarks>
    /// <param name="schedule">Scheduler strategy for repeating</param>
    /// <returns>The result of the last invocation</returns>
    public IO<A> Repeat(Schedule schedule) =>
        RepeatUntil(schedule, _ => false);

    /// <summary>
    /// Keeps repeating the computation until the predicate returns false, or an error occurs 
    /// </summary>
    /// <remarks>
    /// Any resources acquired within a repeated IO computation will automatically be released.  This also means you can't
    /// acquire resources and return them from within a repeated computation.
    /// </remarks>
    /// <param name="predicate">Keep repeating while this predicate returns `true` for each computed value</param>
    /// <returns>The result of the last invocation</returns>
    public IO<A> RepeatWhile(Func<A, bool> predicate) => 
        RepeatUntil(Prelude.not(predicate));

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
    public IO<A> RepeatWhile(
        Schedule schedule,
        Func<A, bool> predicate) =>
        RepeatUntil(schedule, Prelude.not(predicate));

    /// <summary>
    /// Keeps repeating the computation until the predicate returns true, or an error occurs
    /// </summary>
    /// <remarks>
    /// Any resources acquired within a repeated IO computation will automatically be released.  This also means you can't
    /// acquire resources and return them from within a repeated computation.
    /// </remarks>
    /// <param name="predicate">Keep repeating until this predicate returns `true` for each computed value</param>
    /// <returns>The result of the last invocation</returns>
    public abstract IO<A> RepeatUntil(
        Func<A, bool> predicate);

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
    public abstract IO<A> RepeatUntil(
        Schedule schedule,
        Func<A, bool> predicate);
    
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
    public IO<A> Retry() =>
        RetryUntil(_ => false);

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
    public IO<A> Retry(Schedule schedule) =>
        RetryUntil(schedule, _ => false);

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
    public IO<A> RetryWhile(Func<Error, bool> predicate) => 
        RetryUntil(Prelude.not(predicate));

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
    public IO<A> RetryWhile(
        Schedule schedule,
        Func<Error, bool> predicate) =>
        RetryUntil(schedule, Prelude.not(predicate));

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
    public abstract IO<A> RetryUntil(
        Func<Error, bool> predicate);

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
    public abstract IO<A> RetryUntil(
        Schedule schedule,
        Func<Error, bool> predicate);

    /// <summary>
    /// Catches any error thrown by invoking this IO computation, passes it through a predicate,
    /// and if that returns true, returns the result of invoking the Fail function, otherwise
    /// this is returned.
    /// </summary>
    /// <param name="Predicate">Predicate</param>
    /// <param name="Fail">Fail functions</param>
    public abstract IO<A> Catch(Func<Error, bool> Predicate, Func<Error, K<IO, A>> Fail);

    /// <summary>
    /// Monoid combine
    /// </summary>
    /// <param name="rhs">Alternative</param>
    /// <returns>This if computation runs without error.  `rhs` otherwise</returns>
    public IO<A> Combine(IO<A> rhs) =>
        Catch(_ => true, _ => rhs);
    
    public override string ToString() => 
        "IO";
}
