using System;
using LanguageExt.DSL;
using System.Threading;
using LanguageExt.Common;
using LanguageExt.Traits;
using System.Threading.Tasks;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using static LanguageExt.Prelude;

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
/// As this is C#, the above restrictions are for you to enforce. It would be reasonable
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
    //  General
    //
    
    public static IO<A> Empty =>
        IOEmpty<A>.Default;

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Functor
    //

    public abstract IO<B> Map<B>(Func<A, B> f);

    public virtual IO<B> ApplyBack<B>(K<IO, Func<A, B>> f) =>
        new IOApply<A, B, B>(f, this, IO.pure);
    
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

    public virtual IO<S> Fold<S>(
        Schedule schedule,
        S initialState,
        Func<S, A, S> folder) =>
        new IOFold<S, A, S>(this, schedule, initialState, folder, IO.pure);

    public virtual IO<S> Fold<S>(
        S initialState,
        Func<S, A, S> folder) =>
        new IOFold<S, A, S>(this, Schedule.Forever, initialState, folder, IO.pure);

    public virtual IO<S> FoldWhile<S>(
        Schedule schedule,
        S initialState,
        Func<S, A, S> folder,
        Func<S, bool> stateIs) =>
        new IOFoldWhile<S, A, S>(this, schedule, initialState, folder, s => stateIs(s.State), IO.pure);

    public virtual IO<S> FoldWhile<S>(
        S initialState,
        Func<S, A, S> folder,
        Func<S, bool> stateIs) =>
        new IOFoldWhile<S, A, S>(this, Schedule.Forever, initialState, folder, s => stateIs(s.State), IO.pure);
    
    public virtual IO<S> FoldWhile<S>(
        Schedule schedule,
        S initialState,
        Func<S, A, S> folder,
        Func<A, bool> valueIs) =>
        new IOFoldWhile<S, A, S>(this, schedule, initialState, folder, s => valueIs(s.Value), IO.pure);

    public virtual IO<S> FoldWhile<S>(
        S initialState,
        Func<S, A, S> folder,
        Func<A, bool> valueIs) =>
        new IOFoldWhile<S, A, S>(this, Schedule.Forever, initialState, folder, s => valueIs(s.Value), IO.pure);
    
    public virtual IO<S> FoldWhile<S>(
        Schedule schedule,
        S initialState,
        Func<S, A, S> folder,
        Func<(S State, A Value), bool> predicate) =>
        new IOFoldWhile<S, A, S>(this, schedule, initialState, folder, predicate, IO.pure);

    public virtual IO<S> FoldWhile<S>(
        S initialState,
        Func<S, A, S> folder,
        Func<(S State, A Value), bool> predicate) =>
        new IOFoldWhile<S, A, S>(this, Schedule.Forever, initialState, folder, predicate, IO.pure);

    public virtual IO<S> FoldUntil<S>(
        Schedule schedule,
        S initialState,
        Func<S, A, S> folder,
        Func<S, bool> stateIs) =>
        new IOFoldUntil<S, A, S>(this, schedule, initialState, folder, p => stateIs(p.State), IO.pure);
    
    public virtual IO<S> FoldUntil<S>(
        S initialState,
        Func<S, A, S> folder,
        Func<S, bool> stateIs) =>
        new IOFoldUntil<S, A, S>(this, Schedule.Forever, initialState, folder, p => stateIs(p.State), IO.pure);
    
    public virtual IO<S> FoldUntil<S>(
        Schedule schedule,
        S initialState,
        Func<S, A, S> folder,
        Func<A, bool> valueIs) =>
        new IOFoldUntil<S, A, S>(this, schedule, initialState, folder, p => valueIs(p.Value), IO.pure);
    
    public virtual IO<S> FoldUntil<S>(
        S initialState,
        Func<S, A, S> folder,
        Func<A, bool> valueIs) =>
        new IOFoldUntil<S, A, S>(this, Schedule.Forever, initialState, folder, p => valueIs(p.Value), IO.pure);
    
    public virtual IO<S> FoldUntil<S>(
        S initialState,
        Func<S, A, S> folder,
        Func<(S State, A Value), bool> predicate) =>
        new IOFoldUntil<S, A, S>(this, Schedule.Forever, initialState, folder, predicate, IO.pure);

    public virtual IO<S> FoldUntil<S>(
        Schedule schedule,
        S initialState,
        Func<S, A, S> folder,
        Func<(S State, A Value), bool> predicate) =>
        new IOFoldUntil<S, A, S>(this, schedule, initialState, folder, predicate, IO.pure);
    
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Cross thread-context posting
    //
 
    /// <summary>
    /// Make this IO computation run on the `SynchronizationContext` that was captured at the start
    /// of the IO chain (i.e. the one embedded within the `EnvIO` environment that is passed through
    /// all IO computations)
    /// </summary>
    public virtual IO<A> Post() =>
        IO.liftAsync(async env =>
                       {
                           if (env.Token.IsCancellationRequested) throw new TaskCanceledException();
                           if (env.SyncContext is null) return await RunAsync(env);

                           A?         value = default;
                           Exception? error = default;
                           using var  wait  = new AutoResetEvent(false);

                           env.SyncContext.Post(_ =>
                                                {
                                                    try
                                                    {
                                                        value = Run(env);
                                                    }
                                                    catch (Exception e)
                                                    {
                                                        error = e;
                                                    }
                                                    finally
                                                    {
                                                        wait.Set();
                                                    }
                                                },
                                                null);

                           await wait.WaitOneAsync(env.Token);
                           if (env.Token.IsCancellationRequested) throw new TaskCanceledException();
                           if (error is not null) error.Rethrow<A>();
                           return value!;
                       });
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Monad
    //

    public abstract IO<B> Bind<B>(Func<A, K<IO, B>> f);
    public abstract IO<B> BindAsync<B>(Func<A, ValueTask<K<IO, B>>> f);

    public IO<B> Bind<B>(Func<A, IO<B>> f) =>
        Bind(x => f(x).Kind());

    public IO<B> Bind<B>(Func<A, Pure<B>> f) =>
        Map(x => f(x).Value);

    public K<M, B> Bind<M, B>(Func<A, K<M, B>> f) 
        where M : Monad<M> =>
        M.LiftIOMaybe(this).Bind(f);

    public IO<B> BindAsync<B>(Func<A, ValueTask<IO<B>>> f) =>
        BindAsync(async x => (await f(x)).Kind());

    public K<M, B> BindAsync<M, B>(Func<A, ValueTask<K<M, B>>> f)
        where M : Monad<M> => 
        Bind(x => M.LiftIOMaybe(new IOPureAsync<K<M, B>>(f(x)))).Flatten();

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  LINQ
    //
    
    public IO<B> Select<B>(Func<A, B> f) =>
        Map(f);

    public IO<C> SelectMany<B, C>(Func<A, IO<B>> bind, Func<A, B, C> project) =>
        Bind(x => IOTail<A>.resolve(x, bind(x), project));

    public IO<C> SelectMany<B, C>(Func<A, K<IO, B>> bind, Func<A, B, C> project) =>
        Bind(x => IOTail<A>.resolve(x, bind(x).As(), project));

    public IO<C> SelectMany<B, C>(Func<A, Pure<B>> bind, Func<A, B, C> project) =>
        Bind(x => bind(x).Map(y => project(x, y)));

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

    public static IO<A> operator |(IO<A> lhs, Finally<IO> rhs) =>
        lhs.Finally(rhs.Operation);
    
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
    public IO<A> Bracket() =>
        WithEnv(env => env.LocalResources);

    /// <summary>
    /// The IO monad tracks resources automatically, this creates a local resource environment
    /// to run this computation in.  If the computation errors then the resources are automatically
    /// released.  
    /// </summary>
    /// <remarks>
    /// This differs from `Bracket` in that `Bracket` will also free resources once the computation
    /// is successfully complete.  `BracketFail` only frees resources on failure.
    /// </remarks>
    [Pure]
    [MethodImpl(Opt.Default)]
    public IO<A> BracketFail() =>
        WithEnvFail(env => env.LocalResources);

    /// <summary>
    /// When acquiring, using, and releasing various resources, it can be quite convenient to write a function to manage
    /// the acquisition and releasing, taking a function of the acquired value that specifies an action to be performed
    /// in between.
    /// </summary>
    /// <param name="Use">Function to use the acquired resource</param>
    /// <param name="Fin">Function to invoke to release the resource</param>
    public IO<C> Bracket<B, C>(Func<A, IO<C>> Use, Func<A, IO<B>> Fin) =>
        Bind(x => Use(x).Finally(Fin(x)));

    /// <summary>
    /// When acquiring, using, and releasing various resources, it can be quite convenient to write a function to manage
    /// the acquisition and releasing, taking a function of the acquired value that specifies an action to be performed
    /// in between.
    /// </summary>
    /// <param name="Use">Function to use the acquired resource</param>
    /// <param name="Catch">Function to run to handle any exceptions</param>
    /// <param name="Fin">Function to invoke to release the resource</param>
    public IO<C> Bracket<B, C>(Func<A, IO<C>> Use, Func<Error, IO<C>> Catch, Func<A, IO<B>> Fin) =>
        Bind(x => Use(x).Catch(Catch).Finally(Fin(x)));

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Conversion
    //
    
    public static implicit operator IO<A>(Pure<A> ma) =>
        IO.pure(ma.Value);

    public static implicit operator IO<A>(Error error) =>
        IO.lift(_ => error.Throw<A>());

    public static implicit operator IO<A>(Fail<Error> ma) =>
        IO.lift(() => ma.Value.Throw<A>());

    public static implicit operator IO<A>(Fail<Exception> ma) =>
        IO.lift(() => ma.Value.Rethrow<A>());

    public static implicit operator IO<A>(Lift<EnvIO, A> ma) =>
        IO.lift(ma.Function);

    public static implicit operator IO<A>(Lift<A> ma) =>
        IO.lift(ma.Function);
    
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
    /// Map the `EnvIO` value that is threaded through computation and run this `IO` operation in the newly
    /// mapped environment.       
    /// </summary>
    protected virtual IO<A> WithEnv(Func<EnvIO, EnvIO> f) =>
        new IOLocal<A, A>(f, this, IO.pure);

    /// <summary>
    /// Map the `EnvIO` value that is threaded through computation and run this `IO` operation in the newly
    /// mapped environment.    
    /// </summary>
    /// <remarks>
    /// Note, this only resets the environment upon error.  Successful operations will propagate the mapped
    /// environment.
    /// </remarks>
    protected virtual IO<A> WithEnvFail(Func<EnvIO, EnvIO> f) =>
        new IOLocalOnFailOnly<A, A>(f, this, IO.pure);

    /// <summary>
    /// Create a local cancellation environment
    /// </summary>
    public IO<A> Local() =>
        WithEnv(env => EnvIO.New(env.Resources, env.Token, null, env.SyncContext));
    
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
    public virtual IO<ForkIO<A>> Fork(Option<TimeSpan> timeout = default) =>
        IO.lift(
            env =>
            {
                if (env.Token.IsCancellationRequested) throw new TaskCanceledException();
                
                // Create a new local token-source with its own cancellation token
                var tsrc  = timeout.Match(Some: to => new CancellationTokenSource(to), 
                                          None: () => new CancellationTokenSource());
                var token = tsrc.Token;

                // If the parent cancels, we should too
                var reg = env.Token.Register(() => tsrc.Cancel());

                // Gather our resources for clean-up
                var cleanup = new CleanUp(tsrc, reg);

                var parentResources = env.Resources;

                var task = Task.Factory.StartNew(
                    () =>
                    {
                        var forkedResources = new Resources(parentResources);
                        try
                        {
                            var t = RunAsync(EnvIO.New(forkedResources, token, tsrc, env.SyncContext));
                            return t.GetAwaiter().GetResult();
                        }
                        finally
                        {
                            forkedResources.Dispose();
                            cleanup.Dispose();
                        }
                    }, TaskCreationOptions.LongRunning);

                return new ForkIO<A>(
                        IO.lift<Unit>(() => {
                                          try
                                          {
                                              tsrc.Cancel();
                                          }
                                          catch(ObjectDisposedException)
                                          {
                                              // ignore if already cancelled
                                          }
                                          return default; 
                                      }),
                        IO.liftAsync(e => AwaitAsync(task, e, token, tsrc)));
            });
    
    
    /// <summary>
    /// Run the `IO` monad to get its result.  Differs from `Run` in that it catches any exceptions and turns
    /// them into a `Fin〈A〉` result. 
    /// </summary>
    public FinT<IO, A> Try() =>
        new (Map(Fin.Succ).Catch(Fin.Fail<A>));
    
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
        RepeatUntil(not(predicate));

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
        RepeatUntil(schedule, not(predicate));

    /// <summary>
    /// Keeps repeating the computation until the predicate returns true, or an error occurs
    /// </summary>
    /// <remarks>
    /// Any resources acquired within a repeated IO computation will automatically be released.  This also means you can't
    /// acquire resources and return them from within a repeated computation.
    /// </remarks>
    /// <param name="predicate">Keep repeating until this predicate returns `true` for each computed value</param>
    /// <returns>The result of the last invocation</returns>
    public IO<A> RepeatUntil(Func<A, bool> predicate) =>
        Bracket().Bind(v => predicate(v) 
                                ? IO.pure(v) 
                                : RepeatUntil(predicate));

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
    public virtual IO<A> RepeatUntil(
        Schedule schedule,
        Func<A, bool> predicate)
    {
        return go(schedule.PrependZero.Run().GetIterator(), default);

        IO<A> go(Iterator<Duration> iter, A? value) =>
            iter switch
            {
                Iterator<Duration>.Nil =>
                    IO.pure<A>(value!),

                Iterator<Duration>.Cons(var head, var tail) =>
                    IO.yieldFor(head)
                      .Bind(_ => Bracket()
                               .Bind(v => predicate(v) 
                                              ? IO.pure(v) 
                                              : go(tail, v))),
                
                _ => throw new InvalidOperationException("Invalid iterator")
            };
    }
    
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
        RetryUntil(not(predicate));

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
        RetryUntil(schedule, not(predicate));

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
    public IO<A> RetryUntil(Func<Error, bool> predicate) =>
        BracketFail()
           .Catch(e => predicate(e)
                           ? IO.fail<A>(e)
                           : RetryUntil(predicate)).As();

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
    public virtual IO<A> RetryUntil(Schedule schedule, Func<Error, bool> predicate)
    {
        return go(schedule.PrependZero.Run().GetIterator(), Errors.None);

        IO<A> go(Iterator<Duration> iter, Error error) =>
            iter switch
            {
                Iterator<Duration>.Nil =>
                    IO.fail<A>(error),

                Iterator<Duration>.Cons(var head, var tail) =>
                    IO.yieldFor(head)
                      .Bind(_ => BracketFail()
                                   .Catch(e => predicate(e)
                                                   ? IO.fail<A>(e)
                                                   : go(tail, e))),
                     
                _ => throw new InvalidOperationException("Invalid iterator")
            };
    }

    /// <summary>
    /// Catches any error thrown by invoking this IO computation, passes it through a predicate,
    /// and if that returns true, returns the result of invoking the Fail function, otherwise
    /// this is returned.
    /// </summary>
    /// <param name="Predicate">Predicate</param>
    /// <param name="Fail">Fail functions</param>
    public virtual IO<A> Catch(Func<Error, bool> Predicate, Func<Error, K<IO, A>> Fail) =>
        new IOCatch<A, A>(this, Predicate, Fail, null, IO.pure);

    /// <summary>
    /// Run a `finally` operation after the `this` operation regardless of whether `this` succeeds or not.
    /// </summary>
    /// <param name="finally">Finally operation</param>
    /// <returns>Result of primary operation</returns>
    public virtual IO<A> Finally<X>(K<IO, X> @finally) =>
        new IOFinal<X, A, A>(this, @finally, IO.pure);
    
    /// <summary>
    /// Monoid combine
    /// </summary>
    /// <param name="rhs">Alternative</param>
    /// <returns>This if computation runs without error.  `rhs` otherwise</returns>
    public IO<A> Combine(IO<A> rhs) =>
        Catch(_ => true, _ => rhs);

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
    public A Run()
    {
        // RunAsync can run completely synchronously and without the creation of an async/await state-machine, so calling
        // it for operations that are completely synchronous has no additional overhead for us.  Therefore, calling it
        // directly here and then unpacking the `ValueTask` makes sense to reduce code duplication.

        var task = RunAsync();
        if (task.IsCompleted) return task.Result;

        // If RunAsync really had to do some asynchronous work, then make sure we use the awaiter and get its result
        return task.GetAwaiter().GetResult();
    }

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
    public A Run(EnvIO envIO)
    {
        // RunAsync can run completely synchronously and without the creation of an async/await state-machine, so calling
        // it for operations that are completely synchronous has no additional overhead for us.  Therefore, calling it
        // directly here and then unpacking the `ValueTask` makes sense to reduce code duplication.

        var task = RunAsync(envIO);
        if (task.IsCompleted) return task.Result;

        // If RunAsync really had to do some asynchronous work, then make sure we use the awaiter and get its result
        return task.GetAwaiter().GetResult();
    }

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
    public ValueTask<A> RunAsync()
    {
        var envIO = EnvIO.New();
        try
        {
            var va = RunAsync(envIO);
            if (va.IsCompleted)
            {
                envIO.Dispose();
                return va;
            }
            else
            {
                return SafeRunAsync(va, envIO);
            }
        }
        catch (Exception)
        {
            envIO.Dispose();
            throw;
        }
    }

    static async ValueTask<A> SafeRunAsync(ValueTask<A> va, EnvIO envIO)
    {
        try
        {
            return await va;
        }
        finally
        {
            envIO.Dispose();
        }
    }

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
    public ValueTask<A> RunAsync(EnvIO envIO)
    {
        if (envIO.Token.IsCancellationRequested) return ValueTask.FromCanceled<A>(envIO.Token);
        var ma     = this;
        var locals = Seq(envIO);
        var stack  = Seq(new StackItem(ex => ex.Rethrow<IO<A>>(), locals)); // we want at least one item in the stack
                                                                              // so that we benefit from the auto-clean-up   
                                                                              // of resources.
                                                                              
        while (!envIO.Token.IsCancellationRequested)
        {
            switch (ma)
            {
                case InvokeAsync<A>:
                    return ma.RunAsyncInternal(stack, locals);

                case InvokeAsyncIO<A>:
                    return ma.RunAsyncInternal(stack, locals);

                case InvokeSync<A> op:
                    try
                    {
                        return new ValueTask<A>(op.Invoke(envIO));
                    }
                    catch (Exception e)
                    {
                        if (stack.IsEmpty) throw;
                        ma = RunCatchHandler(e);
                        break;
                    }

                case InvokeSyncIO<A> op:
                    try
                    {
                        ma = op.Invoke(envIO);
                    }
                    catch (Exception e)
                    {
                        if (stack.IsEmpty) throw;
                        ma = RunCatchHandler(e);
                    }

                    break;

                case IOCatch<A> c:
                    var @catch = c.MakeHandler();
                    stack = new StackItem(@catch, locals).Cons(stack);
                    ma = c.MakeOperation();
                    break;

                case IOCatchPop<A> pop:
                    stack = stack.Tail;
                    ma = pop.Next;
                    break;
                
                case IOLocal<A> local:
                    locals = local.MapEnvIO(envIO).Cons(locals);
                    envIO = locals[0]; 
                    ma = local.MakeOperation();
                    break;
            
                case IOLocalRestore<A> restore:
                    locals = locals.Tail;
                    envIO = locals[0];
                    ma = restore.Next.As();
                    break;                    

                case IOTail<A> tail:
                    ma = tail.Tail;
                    break;

                default:
                    return ValueTask.FromException<A>(Errors.IODSLExtension);
            }
        }

        return ValueTask.FromCanceled<A>(envIO.Token);

        IO<A> RunCatchHandler(Exception e)
        {
            var oldLocals = locals;
            var top       = stack[0];
            var @catch    = top.Catch;
            locals = top.EnvIO;
            stack = stack.Tail;

            // Unwind any local environments
            for (var i = oldLocals.Count - 1; i >= locals.Count; i--)
            {
                oldLocals[i].Dispose();
            }

            // This must be the last thing because the `@catch` may rethrow,
            // so we have to have cleaned everything up.
            return @catch(e);
        }
    }

    /// <summary>
    /// This version of `RunAsync` uses the async/await machinery.  It is kept separate from the `RunAsync` version
    /// so that we can avoid creating the async/await state-machine if all operations are synchronous.
    /// </summary>
    async ValueTask<A> RunAsyncInternal(Seq<StackItem> stack, Seq<EnvIO> locals)
    {
        var ma = this;
        var envIO = locals[0];
        while (!envIO.Token.IsCancellationRequested)
        {
            switch (ma)
            {
                case InvokeAsync<A> op:
                    try
                    {
                        return await op.Invoke(envIO);
                    }
                    catch (Exception e)
                    {
                        if (stack.IsEmpty) throw;
                        ma = RunCatchHandler(e);
                        break;
                    }

                case InvokeSync<A> op:
                    try
                    {
                        return op.Invoke(envIO);
                    }
                    catch (Exception e)
                    {
                        if (stack.IsEmpty) throw;
                        ma = RunCatchHandler(e);
                        break;
                    }

                case InvokeSyncIO<A> op:
                    try
                    {
                        ma = op.Invoke(envIO);
                    }
                    catch (Exception e)
                    {
                        if (stack.IsEmpty) throw;
                        ma = RunCatchHandler(e);
                    }
                    break;

                case InvokeAsyncIO<A> op:
                    try
                    {
                        ma = await op.Invoke(envIO);
                    }
                    catch (Exception e)
                    {
                        if (stack.IsEmpty) throw;
                        ma = RunCatchHandler(e);
                    }
                    break;

                case IOCatch<A> c:
                    var @catch = c.MakeHandler();
                    stack = new StackItem(@catch, locals).Cons(stack);
                    ma = c.MakeOperation();
                    break;

                case IOCatchPop<A> pop:
                    stack = stack.Tail;
                    ma = pop.Next;
                    break;

                case IOLocal<A> local:
                    locals = local.MapEnvIO(envIO).Cons(locals);
                    envIO = locals[0]; 
                    ma = local.MakeOperation();
                    break;
                
                case IOLocalRestore<A> restore:
                    locals = locals.Tail;
                    envIO = locals[0];
                    ma = restore.Next.As();
                    break;
                
                case IOTail<A> tail:
                    ma = tail.Tail;
                    break;

                default:
                    return Errors.IODSLExtension.Throw<A>();
            }
        }

        throw new TaskCanceledException();

        IO<A> RunCatchHandler(Exception e)
        {
            var oldLocals = locals;
            var top       = stack[0];
            var @catch    = top.Catch;
            locals = top.EnvIO;
            stack = stack.Tail;

            // Unwind any local environments
            for (var i = oldLocals.Count - 1; i >= locals.Count; i--)
            {
                oldLocals[i].Dispose();
            }

            // This must be the last thing because the `@catch` may rethrow,
            // so we have to have cleaned everything up.
            return @catch(e);
        }
    }

    public override string ToString() => 
        "IO";
    
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Internal
    //
    
    async Task<A> AwaitAsync(Task<A> t, EnvIO envIO, CancellationToken token, CancellationTokenSource source)
    {
        if (envIO.Token.IsCancellationRequested) throw new TaskCanceledException();
        if (token.IsCancellationRequested) throw new TaskCanceledException();
        await using var reg = envIO.Token.Register(source.Cancel);
        return await t;        
    }
    
    record CleanUp(CancellationTokenSource Src, CancellationTokenRegistration Reg) : IDisposable
    {
        volatile int disposed;
        public void Dispose()
        {
            if (Interlocked.Exchange(ref disposed, 1) == 0)
            {
                try{Src.Dispose();} catch { /* not important */ }
                try{Reg.Dispose();} catch { /* not important */ }
            }
        }
    }
    
    readonly record struct StackItem(Func<Exception, IO<A>> Catch, Seq<EnvIO> EnvIO); 
}
