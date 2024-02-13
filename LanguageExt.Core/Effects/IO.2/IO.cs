using System;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt.Common;
using LanguageExt.HKT;
using LanguageExt.TypeClasses;

namespace LanguageExt;

/// <summary>
/// A value of type `IO` a is a computation which, when performed, does some I/O before returning
/// a value of type `A`.
///
/// There is really only one way you should _"perform"_ an I/O action: bind it to `Main` in your
/// program:  When your program is run, the I/O will be performed. It shouldn't be possible to
/// perform I/O from an arbitrary function, unless that function is itself in the `IO` monad and
/// called at some point, directly or indirectly, from `Main`.
///
/// Obviously, as this is C#, the above restrictions are for you to enforce. It would be reasonable
/// to relax that approach and have IO invoked from, say, web-request handlers - or any other 'edges'
/// of your application.
/// 
/// `IO` is a monad, so `IO` actions can be combined using either the LINQ-notation or the `bind` 
/// operations from the `Monad` class.
/// </summary>
/// <param name="runIO">The lifted thunk that is the IO operation</param>
/// <typeparam name="A">Bound value</typeparam>
public record IO<A>(Func<EnvIO, ValueTask<A>> runIO) : K<IO, A>, Monoid<IO<A>>
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Construction
    //
    
    public static IO<A> Pure(A value) => 
        new(_ => ValueTask.FromResult(value));

    public static readonly IO<A> Empty =
        new(_ => throw new BottomException());

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Lifting
    //
    
    public static IO<A> Lift(Func<A> f) =>
        new(_ => ValueTask.FromResult(f()));

    public static IO<A> Lift(Func<EnvIO, A> f) =>
        new(e => ValueTask.FromResult(f(e)));

    public static IO<A> LiftIO(Func<ValueTask<A>> f) =>
        new(_ => f());

    public static IO<A> LiftIO(Func<EnvIO, ValueTask<A>> f) =>
        new(f);

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Functor
    //

    public IO<B> Map<B>(Func<A, B> f) => 
        new(e => ValueTask.FromResult(f(Run(e))));

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Monad
    //

    public IO<B> Bind<B>(Func<A, IO<B>> f) =>
        new(e => ValueTask.FromResult(f(Run(e)).Run(e)));

    public IO<B> Bind<B>(Func<A, K<IO, B>> f) =>
        new(e => ValueTask.FromResult(f(Run(e)).As().Run(e)));

    public IO<B> Bind<B>(Func<A, Pure<B>> f) =>
        new(e => ValueTask.FromResult(f(Run(e)).Value));

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  LINQ
    //
    
    public IO<B> Select<B>(Func<A, B> f) =>
        Map(f);

    public IO<C> SelectMany<B, C>(Func<A, IO<B>> bind, Func<A, B, C> project) =>
        Bind(x => bind(x).Map(y => project(x, y)));

    public IO<C> SelectMany<B, C>(Func<A, Pure<B>> bind, Func<A, B, C> project) =>
        Bind(x => bind(x).Map(y => project(x, y)));

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Fail coalescing
    //

    public IO<A> Or(IO<A> mb) =>
        new(e =>
            {
                try
                {
                    return ValueTask.FromResult(Run(e));
                }
                catch
                {
                    return ValueTask.FromResult(mb.Run(e));
                }
            });
    
    public static IO<A> operator |(IO<A> ma, IO<A> mb) =>
        ma.Or(mb);

    public static IO<A> operator |(IO<A> ma, Pure<A> mb) =>
        ma.Or(mb);

    public static IO<A> operator |(IO<A> ma, Fail<Error> mb) =>
        ma.Or(mb);

    public static IO<A> operator |(IO<A> ma, Fail<Exception> mb) =>
        ma.Or(mb);

    public static IO<A> operator |(IO<A> ma, Error mb) =>
        ma.Or(mb);

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Trait implementaion
    //
    
    static IO<A> Semigroup<IO<A>>.Append(IO<A> x, IO<A> y) => 
        x | y;

    static IO<A> Monoid<IO<A>>.Empty =>
        Empty;

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Conversion
    //
    
    public static implicit operator IO<A>(Pure<A> ma) =>
        Pure(ma.Value);

    public static implicit operator IO<A>(Error error) =>
        Lift(error.Throw<A>);

    public static implicit operator IO<A>(Fail<Error> ma) =>
        Lift(() => ma.Value.Throw<A>());

    public static implicit operator IO<A>(Fail<Exception> ma) =>
        Lift(() => ma.Value.Rethrow<A>());

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Parallel
    //
    
    /// <summary>
    /// Queues the specified work to run on the thread pool  
    /// </summary>
    /// <param name="ma">Computation to fork</param>
    /// <param name="timeout">Optional timeout</param>
    /// <returns>`Fork` record that contains members for cancellation and optional awaiting</returns>
    public IO<ForkIO<A>> Fork(Option<TimeSpan> timeout = default) =>
        IO<ForkIO<A>>.Lift(
            env =>
            {
                // Create a new local token-source with its own cancellation token
                var tsrc = timeout.IsSome
                               ? new CancellationTokenSource((TimeSpan)timeout)
                               : new CancellationTokenSource();
                var token = tsrc.Token;

                // If the parent cancels, we should too
                var reg = env.Token.Register(() => tsrc.Cancel());

                // Run the transducer asynchronously
                var cleanup = new CleanUp(tsrc, reg);

                var task = Task.Run(
                    async () =>
                    {
                        try
                        {
                            return await runIO(new EnvIO(token, tsrc, env.SyncContext));
                        }
                        finally
                        {
                            cleanup.Dispose();
                        }
                    }, token);

                return new ForkIO<A>(
                        IO<Unit>.Lift(() => { tsrc.Cancel(); return default; }),
                        LiftIO(async _ => await task.ConfigureAwait(false)));
            });

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
    /// <returns>Result of the IO operation</returns>
    /// <exception cref="TaskCanceledException">Throws if the operation is cancelled</exception>
    /// <exception cref="BottomException">Throws if any lifted task fails without a value `Exception` value.</exception>
    public A Run() =>
        Run(EnvIO.New());

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
    public A Run(EnvIO env)
    {
        var token = env.Token;
        if (token.IsCancellationRequested) throw new TaskCanceledException();

        // Launch the task
        var t = runIO(env);

        // Spin waiting for the task to complete or be cancelled
        SpinWait sw = default;
        while (!t.IsCompleted && !token.IsCancellationRequested)
        {
            sw.SpinOnce();
        }

        if (t.IsCanceled || token.IsCancellationRequested)
        {
            throw new TaskCanceledException();
        }

        if (t.IsFaulted)
        {
            return t.AsTask().Exception is Exception e
                       ? e.Rethrow<A>()
                       : throw new BottomException();
        }

        return t.Result;
    }
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Internal
    //
    
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
