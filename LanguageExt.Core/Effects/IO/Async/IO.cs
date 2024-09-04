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
record IOAsync<A>(Func<EnvIO, Task<IOResponse<A>>> runIO) : IO<A>
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Functor
    //

    public override IO<B> Map<B>(Func<A, B> f) =>
        new IOAsync<B>(
            async e =>
            {
                if (e.Token.IsCancellationRequested) throw new TaskCanceledException();
                return IOResponse.Complete(f(await RunAsync(e)));
            });

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // 
    //  Folding
    //

    public override IO<S> FoldUntil<S>(
        Schedule schedule,
        S initialState,
        Func<S, A, S> folder,
        Func<(S State, A Value), bool> predicate) =>
        new IOAsync<S>(async envIO =>
            {
                if (envIO.Token.IsCancellationRequested) throw new TaskCanceledException();
                var r     = await RunAsync(envIO);
                var state = folder(initialState, r);
                if (predicate((state, r))) return IOResponse.Complete(state);

                var token = envIO.Token;
                foreach (var delay in schedule.Run())
                {
                    IO.yieldFor(delay, token);
                    r     = await RunAsync(envIO);
                    state = folder(state, r);
                    if (predicate((state, r))) return IOResponse.Complete(state);
                }

                return IOResponse.Complete(state);
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
    public override IO<A> Post() =>
        new IOAsync<A>(async env =>
            {
                if (env.Token.IsCancellationRequested) throw new TaskCanceledException();
                if (env.SyncContext is null) return await runIO(env);

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
                return IOResponse.Complete(value!);
            });
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Monad
    //

    public override IO<B> Bind<B>(Func<A, IO<B>> f) =>
        new IOAsync<B>(async e =>
            {
                if (e.Token.IsCancellationRequested) throw new TaskCanceledException();
                return IOResponse.Recurse(f(await RunAsync(e)));
            });

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
    public override IO<A> Bracket() =>
        new IOAsync<A>(async env =>
            {
                using var lenv = env.LocalResources;
                return IOResponse.Complete(await RunAsync(lenv));
            });

    /// <summary>
    /// When acquiring, using, and releasing various resources, it can be quite convenient to write a function to manage
    /// the acquisition and releasing, taking a function of the acquired value that specifies an action to be performed
    /// in between.
    /// </summary>
    /// <param name="Use">Function to use the acquired resource</param>
    /// <param name="Catch">Function to run to handle any exceptions</param>
    /// <param name="Finally">Function to invoke to release the resource</param>
    public override IO<C> Bracket<B, C>(Func<A, IO<C>> Use, Func<Error, IO<C>> Catch, Func<A, IO<B>> Finally) =>
        new IOAsync<C>(async env =>
            {
                var x = await RunAsync(env);
                try
                {
                    return IOResponse.Complete(await Use(x).RunAsync(env));
                }
                catch (Exception e)
                {
                    return IOResponse.Complete(await Catch(e).RunAsync(env));
                }
                finally
                {
                    (await Finally(x).RunAsync(env))?.Ignore();
                }
            });

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Parallel
    //

    /// <summary>
    /// Create a local cancellation environment
    /// </summary>
    public override IO<A> Local() =>
        new IOAsync<A>(async env =>
            {
                if (env.Token.IsCancellationRequested) throw new TaskCanceledException();

                // Create a new local token-source with its own cancellation token
                using var tsrc = new CancellationTokenSource();
                var       tok  = tsrc.Token;

                // If the parent cancels, we should too
                await using var reg = env.Token.Register(() => tsrc.Cancel());

                var env1 = EnvIO.New(env.Resources, tok, tsrc, env.SyncContext);
                return IOResponse.Complete(await RunAsync(env1));
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
    public override IO<ForkIO<A>> Fork(Option<TimeSpan> timeout = default) =>
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

                // Run the transducer asynchronously
                var cleanup = new CleanUp(tsrc, reg);

                var parentResources = env.Resources;

                var task = Task.Factory.StartNew(
                    () =>
                    {
                        var forkedResources = new Resources(parentResources);
                        try
                        {
                            var t = runIO(EnvIO.New(forkedResources, token, tsrc, env.SyncContext));
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
                                          return IOResponse.Complete<Unit>(default); 
                                      }),
                        LiftAsync(e => AwaitAsync(task, e, token, tsrc)));
            });

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
    public override async ValueTask<A> RunAsync(EnvIO? env = null)
    {
        env ??= EnvIO.New();
        var ma = this;
        while (!env.Token.IsCancellationRequested)
        {
            switch (await ma.runIO(env))
            {
                case CompleteIO<A> (var x):
                    return x;

                case RecurseIO<A>(IOPure<A> io):
                    return io.Value;

                case RecurseIO<A>(IOSync<A> io):
                    return await io.RunAsync(env);

                case RecurseIO<A>(IOAsync<A> io):
                    ma = io;
                    break;
            }
        }
        throw new TaskCanceledException();
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
    /// <exception cref="BottomException">Throws if any lifted task fails without a value `Exception` value.</exception>
    public override A Run(EnvIO? env = null) =>
        RunAsync(env).GetAwaiter().GetResult();

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Repeating the effect
    //

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
    public override IO<A> RepeatUntil(
        Schedule schedule,
        Func<A, bool> predicate) =>
        new IOAsync<A>(async env =>
            {
                if (env.Token.IsCancellationRequested) throw new TaskCanceledException();
                var token = env.Token;
                var lenv  = env.LocalResources;
                try
                {
                    var result = await RunAsync(lenv);
                    
                    // free any resources acquired during a repeat
                    await lenv.Resources.ReleaseAll().RunAsync(env);
                    
                    if (predicate(result)) return IOResponse.Complete(result);

                    foreach (var delay in schedule.Run())
                    {
                        await Task.Delay((TimeSpan)delay, token);
                        result = await RunAsync(lenv);
                        
                        // free any resources acquired during a repeat
                        await lenv.Resources.ReleaseAll().RunAsync(env);
                        
                        if (predicate(result)) return IOResponse.Complete(result);
                    }

                    return IOResponse.Complete(result);
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
    public override IO<A> RepeatUntil(Func<A, bool> predicate) =>
        new IOAsync<A>(async env =>
            {
                if (env.Token.IsCancellationRequested) throw new TaskCanceledException();
                var lenv = env.LocalResources;
                try
                {
                    while(!env.Token.IsCancellationRequested)
                    {
                        var result = await RunAsync(lenv);
                        
                        // free any resources acquired during a repeat
                        await lenv.Resources.ReleaseAll().RunAsync(env);
                        
                        if (predicate(result)) return IOResponse.Complete(result);
                    }
                    throw new TaskCanceledException();
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
    /// This variant will keep retrying until the predicate returns `true` for the error generated at each iteration;
    /// or, until the schedule expires; at which point the last raised error will be thrown.
    /// </remarks>
    /// <remarks>
    /// Any resources acquired within a retrying IO computation will automatically be released *if* the operation fails.
    /// So, successive retries will not grow the acquired resources on each retry iteration.  Any successful operation that
    /// acquires resources will have them tracked in the usual way. 
    /// </remarks>
    public override IO<A> RetryUntil(
        Schedule schedule,
        Func<Error, bool> predicate) =>
        new IOAsync<A>(async env =>
            {
                if (env.Token.IsCancellationRequested) throw new TaskCanceledException();
                var token     = env.Token;
                Exception? lastError;
                var lenv      = env.LocalResources;
                try
                {
                    var r = await RunAsync(lenv);
                    
                    // Any resources that were acquired should be propagated through to the `env`
                    env.Resources.Merge(lenv.Resources);

                    return IOResponse.Complete(r);
                }
                catch (Exception e)
                {
                    // Any resources created whilst trying should be removed for the retry
                    await lenv.Resources.ReleaseAll().RunAsync(env);
                    
                    if (predicate(Error.New(e))) throw;
                    lastError = e;
                }
                
                foreach(var delay in schedule.Run())
                {
                    await Task.Delay((TimeSpan)delay, token);
                    try
                    {
                        var r = await RunAsync(lenv);
                        
                        // Any resources that were acquired should be propagated through to the `env`
                        env.Resources.Merge(lenv.Resources);

                        return IOResponse.Complete(r);
                    }
                    catch (Exception e)
                    {
                        // Any resources created whilst trying should be removed for the retry
                        await lenv.Resources.ReleaseAll().RunAsync(env);
                        
                        if (predicate(Error.New(e))) throw;
                        lastError = e;
                    }
                }
                return lastError.Rethrow<IOResponse<A>>();
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
    public override IO<A> RetryUntil(Func<Error, bool> predicate) =>
        new IOAsync<A>(async env =>
            {
                if (env.Token.IsCancellationRequested) throw new TaskCanceledException();
                var lenv = env.LocalResources;

                while(!env.Token.IsCancellationRequested)
                {
                    try
                    {
                        var r = await RunAsync(lenv);
                        
                        // Any resources that were acquired should be propagated through to the `env`
                        env.Resources.Merge(lenv.Resources);

                        return IOResponse.Complete(r);
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
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Internal
    //
    
    async Task<IOResponse<A>> AwaitAsync(Task<IOResponse<A>> t, EnvIO envIO, CancellationToken token, CancellationTokenSource source)
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

    /// <summary>
    /// Catches any error thrown by invoking this IO computation, passes it through a predicate,
    /// and if that returns true, returns the result of invoking the Fail function, otherwise
    /// this is returned.
    /// </summary>
    /// <param name="Predicate">Predicate</param>
    /// <param name="Fail">Fail functions</param>
    public override IO<A> Catch(Func<Error, bool> Predicate, Func<Error, K<IO, A>> Fail) =>
        new IOAsync<A>(async env =>
                      {
                          if (env.Token.IsCancellationRequested) throw new TaskCanceledException();
                          var lenv = env.LocalResources; 
                          try
                          {
                              var r = await RunAsync(lenv);
                              env.Resources.Merge(lenv.Resources);
                              return IOResponse.Complete(r);
                          }
                          catch(Exception ex)
                          {
                              await lenv.Resources.ReleaseAll().RunAsync(env);
                              var err = Error.New(ex);
                              if (Predicate(err)) return IOResponse.Complete(await Fail(err).As().RunAsync(env));
                              throw;
                          }
                      });

    public override string ToString() => 
        "IOAsync";
}
