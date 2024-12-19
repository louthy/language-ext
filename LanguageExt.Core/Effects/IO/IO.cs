using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt.Common;
using LanguageExt.Traits;
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

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Lifting
    //

    public static IO<A> Lift(Func<EnvIO, A> f) =>
        new IOLiftSync<A, A>(f, Pure);
    
    public static IO<A> LiftAsync(Func<EnvIO, Task<A>> f) => 
        new IOLiftAsync<A, A>(f, Pure);
    
    public static IO<A> Lift(Func<A> f) =>
        Lift(_ => f());

    public static IO<A> LiftAsync(Func<Task<A>> f) =>
        LiftAsync(_ => f());

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Functor
    //

    public abstract IO<B> Map<B>(Func<A, B> f);

    public IO<B> ApplyBack<B>(K<IO, Func<A, B>> f) =>
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

    public IO<S> FoldUntil<S>(
        Schedule schedule,
        S initialState,
        Func<S, A, S> folder,
        Func<(S State, A Value), bool> predicate) =>
        IO<S>.LiftAsync(async envIO =>
                        {
                            if (envIO.Token.IsCancellationRequested) throw new TaskCanceledException();
                            var r     = await RunAsync(envIO);
                            var state = folder(initialState, r);
                            if (predicate((state, r))) return state;

                            var token = envIO.Token;
                            foreach (var delay in schedule.Run())
                            {
                                await IO.yieldFor(delay, token);
                                r = await RunAsync(envIO);
                                state = folder(state, r);
                                if (predicate((state, r))) return state;
                            }
                            return state;
                        });
    
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Cross thread-context posting
    //
 
    /// <summary>
    /// Make this IO computation run on the `SynchronizationContext` that was captured at the start
    /// of the IO chain (i.e. the one embedded within the `EnvIO` environment that is passed through
    /// all IO computations)
    /// </summary>
    public IO<A> Post() =>
        LiftAsync(async env =>
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

    public IO<B> Bind<B>(Func<A, IO<B>> f) =>
        Bind(x => f(x).Kind());

    public IO<B> Bind<B>(Func<A, Pure<B>> f) =>
        Map(x => f(x).Value);

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  LINQ
    //
    
    public IO<B> Select<B>(Func<A, B> f) =>
        Map(f);

    public IO<C> SelectMany<B, C>(Func<A, IO<B>> bind, Func<A, B, C> project) =>
        Bind(x => bind(x) switch
                  {
                      IOTail<B> tail when typeof(B) == typeof(C) => (IO<C>)(object)tail,
                      IOTail<B> => throw new NotSupportedException("Tail calls can't transform in the `select`"),
                      var mb => mb.Map(y => project(x, y)).Kind()
                  });

    public IO<C> SelectMany<B, C>(Func<A, K<IO, B>> bind, Func<A, B, C> project) =>
        Bind(x => bind(x) switch
                  {
                      IOTail<B> tail when typeof(B) == typeof(C) => (IO<C>)(object)tail,
                      IOTail<B> => throw new NotSupportedException("Tail calls can't transform in the `select`"),
                      var mb => mb.Map(y => project(x, y))
                  });
        
        //Bind(x => bind(x).Map(y => project(x, y)));

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
    public IO<A> Bracket() =>
        LiftAsync(async env =>
                  {
                      using var lenv = env.LocalResources;
                      return await RunAsync(lenv);
                  });

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
    public IO<C> Bracket<B, C>(Func<A, IO<C>> Use, Func<Error, IO<C>> Catch, Func<A, IO<B>> Fin) =>
        IO<C>.LiftAsync(async env =>
                        {
                            var x = await RunAsync(env);
                            try
                            {
                                return await Use(x).RunAsync(env);
                            }
                            catch (Exception e)
                            {
                                return await Catch(e).RunAsync(env);
                            }
                            finally
                            {
                                (await Fin(x).RunAsync(env))?.Ignore();
                            }
                        });    

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Conversion
    //
    
    public static implicit operator IO<A>(Pure<A> ma) =>
        Pure(ma.Value);

    public static implicit operator IO<A>(Error error) =>
        Lift(_ => error.Throw<A>());

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
    public IO<A> Local() =>
        LiftAsync(async env =>
                  {
                      if (env.Token.IsCancellationRequested) throw new TaskCanceledException();

                      // Create a new local token-source with its own cancellation token
                      using var tsrc = new CancellationTokenSource();
                      var       tok  = tsrc.Token;

                      // If the parent cancels, we should too
                      await using var reg = env.Token.Register(() => tsrc.Cancel());

                      var env1 = EnvIO.New(env.Resources, tok, tsrc, env.SyncContext);
                      return await RunAsync(env1);
                  });    
    
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
    public IO<ForkIO<A>> Fork(Option<TimeSpan> timeout = default) =>
        IO<ForkIO<A>>.Lift(
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
                        IO<Unit>.Lift(() => {
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
                        LiftAsync(e => AwaitAsync(task, e, token, tsrc)));
            });
    
    
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
    public IO<A> RepeatUntil(Func<A, bool> predicate) =>
        LiftAsync(async env =>
                  {
                      if (env.Token.IsCancellationRequested) throw new TaskCanceledException();
                      var lenv = env.LocalResources;
                      try
                      {
                          while (!env.Token.IsCancellationRequested)
                          {
                              var result = await RunAsync(lenv);

                              // free any resources acquired during a repeat
                              await lenv.Resources.ReleaseAll().RunAsync(env);

                              if (predicate(result)) return result;
                          }

                          throw new TaskCanceledException();
                      }
                      finally
                      {
                          // free any resources acquired during a repeat
                          await lenv.Resources.ReleaseAll().RunAsync(env);
                      }
                  });

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
    public IO<A> RepeatUntil(
        Schedule schedule,
        Func<A, bool> predicate) =>
        LiftAsync(async env =>
                  {
                      if (env.Token.IsCancellationRequested) throw new TaskCanceledException();
                      var token = env.Token;
                      var lenv  = env.LocalResources;
                      try
                      {
                          var result = await RunAsync(lenv);

                          // free any resources acquired during a repeat
                          await lenv.Resources.ReleaseAll().RunAsync(env);

                          if (predicate(result)) return result;

                          foreach (var delay in schedule.Run())
                          {
                              await Task.Delay((TimeSpan)delay, token);
                              result = await RunAsync(lenv);

                              // free any resources acquired during a repeat
                              await lenv.Resources.ReleaseAll().RunAsync(env);

                              if (predicate(result)) return result;
                          }

                          return result;
                      }
                      finally
                      {
                          // free any resources acquired during a repeat
                          await lenv.Resources.ReleaseAll().RunAsync(env);
                      }
                  });      
    
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
    public IO<A> RetryUntil(Func<Error, bool> predicate) =>
        LiftAsync(async env =>
                  {
                      if (env.Token.IsCancellationRequested) throw new TaskCanceledException();
                      var lenv = env.LocalResources;

                      while (!env.Token.IsCancellationRequested)
                      {
                          try
                          {
                              var r = await RunAsync(lenv);

                              // Any resources that were acquired should be propagated through to the `env`
                              env.Resources.Merge(lenv.Resources);

                              return r;
                          }
                          catch (Exception e)
                          {
                              // Any resources created whilst trying should be removed for the retry
                              await lenv.Resources.ReleaseAll().RunAsync(env);

                              if (predicate(Error.New(e))) throw;
                          }
                      }

                      throw new TaskCanceledException();
                  });

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
    public IO<A> RetryUntil(Schedule schedule, Func<Error, bool> predicate) =>
        LiftAsync(async env =>
                  {
                      if (env.Token.IsCancellationRequested) throw new TaskCanceledException();
                      var        token = env.Token;
                      Exception? lastError;
                      var        lenv = env.LocalResources;
                      try
                      {
                          var r = await RunAsync(lenv);

                          // Any resources that were acquired should be propagated through to the `env`
                          env.Resources.Merge(lenv.Resources);

                          return r;
                      }
                      catch (Exception e)
                      {
                          // Any resources created whilst trying should be removed for the retry
                          await lenv.Resources.ReleaseAll().RunAsync(env);

                          if (predicate(Error.New(e))) throw;
                          lastError = e;
                      }

                      foreach (var delay in schedule.Run())
                      {
                          await Task.Delay((TimeSpan)delay, token);
                          try
                          {
                              var r = await RunAsync(lenv);

                              // Any resources that were acquired should be propagated through to the `env`
                              env.Resources.Merge(lenv.Resources);

                              return r;
                          }
                          catch (Exception e)
                          {
                              // Any resources created whilst trying should be removed for the retry
                              await lenv.Resources.ReleaseAll().RunAsync(env);

                              if (predicate(Error.New(e))) throw;
                              lastError = e;
                          }
                      }

                      return lastError.Rethrow<A>();
                  });        

    /// <summary>
    /// Catches any error thrown by invoking this IO computation, passes it through a predicate,
    /// and if that returns true, returns the result of invoking the Fail function, otherwise
    /// this is returned.
    /// </summary>
    /// <param name="Predicate">Predicate</param>
    /// <param name="Fail">Fail functions</param>
    public IO<A> Catch(Func<Error, bool> Predicate, Func<Error, K<IO, A>> Fail) =>
        new IOCatch<A, A>(this, Predicate, Fail, IO.pure);

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
    /// <exception cref="BottomException">Throws if any lifted task fails without a value `Exception` value.</exception>
    public A Run(EnvIO? envIO = null) =>
        RunAsync(envIO).GetAwaiter().GetResult();

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
    public async ValueTask<A> RunAsync(EnvIO? envIO = null)
    {
        if (envIO?.Token.IsCancellationRequested ?? false) throw new TaskCanceledException();
        var envRequiresDisposal = envIO is null;
        envIO ??= EnvIO.New();
        var ma      = this;
        var catches = Seq<Func<Exception, IO<A>>>(); 

        try
        {
            while (!envIO.Token.IsCancellationRequested)
            {
                try
                {
                    switch (ma)
                    {
                        case InvokeAsync<A> op:
                            return await op.Invoke(envIO);
                        
                        case InvokeSync<A> op:
                            return op.Invoke(envIO);

                        case InvokeSyncIO<A> op:
                            ma = op.Invoke(envIO);
                            break;
                        case InvokeAsyncIO<A> op:
                            ma = await op.Invoke(envIO);
                            break;

                        case IOCatch<A> @catch:
                            var handler = @catch.MakeHandler();
                            catches = handler.Cons(catches);
                            ma = @catch.MakeOperation();
                            break;
                        
                        case IOCatchPop<A> pop:
                            catches = catches.Tail;
                            ma = pop.Next;
                            break;
                        
                        case IOTail<A> tail:
                            ma = tail.Tail;
                            break;

                        default:
                            throw new InvalidOperationException("We shouldn't be here!");
                    }
                }
                catch (Exception e)
                {
                    if (catches.IsEmpty) throw;
                    var handler = catches[0];
                    catches = catches.Tail;
                    ma = handler(e);
                }
            }

            throw new TaskCanceledException();
        }
        finally
        {
            if (envRequiresDisposal) envIO.Dispose();
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
}
