using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt.Common;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

public partial class IO
{
    /// <summary>
    /// Put the IO into a failure state
    /// </summary>
    /// <param name="value">Error value</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>IO in a failed state.  Always yields an error.</returns>
    public static IO<A> fail<A>(Error value) =>
        IO<A>.fail(value);
    
    /// <summary>
    /// Put the IO into a failure state
    /// </summary>
    /// <param name="value">Error value</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>IO in a failed state.  Always yields an error.</returns>
    public static IO<A> fail<A>(string value) =>
        IO<A>.fail(Error.New(value));
    
    /// <summary>
    /// Lift an action into the IO monad
    /// </summary>
    /// <param name="f">Action to lift</param>
    public static IO<Unit> lift(Action f) =>
        lift(() =>
             {
                 f();
                 return unit;
             });

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
    public static K<M, A> local<M, A>(K<M, A> ma) 
        where M : Monad<M> =>
        ma.LocalIO();
    
    public static IO<A> lift<A>(Either<Error, A> ma) =>
        ma switch
        {
            Either.Right<Error, A> (var r) => IO<A>.pure(r),
            Either.Left<Error, A> (var l)  => IO<A>.fail(l),
            _                              => IO<A>.fail(Errors.Bottom)
        };
    
    public static IO<A> lift<A>(Fin<A> ma) =>
        lift(ma.ToEither());
    
    public static IO<A> lift<A>(Func<A> f) => 
        IO<A>.Lift(f);
    
    public static IO<A> lift<A>(Func<EnvIO, A> f) => 
        IO<A>.Lift(f);
    
    public static IO<A> lift<A>(Func<Fin<A>> f) => 
        IO<A>.Lift(() => f().ThrowIfFail());
    
    public static IO<A> lift<A>(Func<EnvIO, Fin<A>> f) => 
        IO<A>.Lift(e => f(e).ThrowIfFail());
    
    public static IO<A> lift<A>(Func<Either<Error, A>> f) => 
        IO<A>.Lift(() => f().ToFin().ThrowIfFail());
    
    public static IO<A> lift<A>(Func<EnvIO, Either<Error, A>> f) => 
        IO<A>.Lift(e => f(e).ToFin().ThrowIfFail());

    public static IO<A> liftAsync<A>(Func<Task<A>> f) => 
        IO<A>.LiftAsync(f);

    public static IO<A> liftAsync<A>(Func<EnvIO, Task<A>> f) => 
        IO<A>.LiftAsync(f);

    public static readonly IO<EnvIO> env = 
        lift(e => e);
    
    public static readonly IO<CancellationToken> token = 
        lift(e => e.Token);
    
    public static readonly IO<CancellationTokenSource> source = 
        lift(e => e.Source);
    
    public static readonly IO<Option<SynchronizationContext>> syncContext = 
        lift(e => Optional(e.SyncContext));
    
    public static IO<B> bind<A, B>(K<IO, A> ma, Func<A, K<IO, B>> f) =>
        ma.As().Bind(f);

    public static IO<B> map<A, B>(Func<A, B> f, K<IO, A> ma) => 
        ma.As().Map(f);

    public static IO<B> map<A, B>(Func<A, B> f, IO<A> ma) => 
        ma.Map(f);

    public static IO<B> apply<A, B>(K<IO, Func<A, B>> mf, K<IO, A> ma) => 
        mf.As().Bind(ma.As().Map);

    public static IO<B> action<A, B>(K<IO, A> ma, K<IO, B> mb) =>
        ma.As().Bind(_ => mb);

    public static IO<A> empty<A>() =>
        IO<A>.Empty;

    public static IO<A> or<A>(K<IO, A> ma, K<IO, A> mb) => 
        ma.As() | mb.As();

    /// <summary>
    /// Queue this IO operation to run on the thread-pool. 
    /// </summary>
    /// <param name="timeout">Maximum time that the forked IO operation can run for. `None` for no timeout.</param>
    /// <returns>Returns a `ForkIO` data-structure that contains two IO effects that can be used to either cancel
    /// the forked IO operation or to await the result of it.
    /// </returns>
    [Pure]
    [MethodImpl(Opt.Default)]
    public static K<M, B> mapIO<M, A, B>(K<M, A> ma, Func<IO<A>, IO<B>> f)
        where M : Monad<M> =>
        M.MapIO(ma, f);    
    
    /// <summary>
    /// Queue this IO operation to run on the thread-pool. 
    /// </summary>
    /// <param name="timeout">Maximum time that the forked IO operation can run for. `None` for no timeout.</param>
    /// <returns>Returns a `ForkIO` data-structure that contains two IO effects that can be used to either cancel
    /// the forked IO operation or to await the result of it.
    /// </returns>
    [Pure]
    [MethodImpl(Opt.Default)]
    public static IO<ForkIO<A>> fork<A>(K<IO, A> ma, Option<TimeSpan> timeout = default) =>
        ma.As().Fork(timeout);

    /// <summary>
    /// Queue this IO operation to run on the thread-pool. 
    /// </summary>
    /// <param name="timeout">Maximum time that the forked IO operation can run for. `None` for no timeout.</param>
    /// <returns>Returns a `ForkIO` data-structure that contains two IO effects that can be used to either cancel
    /// the forked IO operation or to await the result of it.
    /// </returns>
    [Pure]
    [MethodImpl(Opt.Default)]
    public static K<M, ForkIO<A>> fork<M, A>(K<M, A> ma, Option<TimeSpan> timeout = default)
        where M : Monad<M> =>
        mapIO(ma, mio => fork(mio , timeout));

    /// <summary>
    /// Yield the thread for the specified milliseconds or until cancelled.
    /// </summary>
    /// <param name="milliseconds">Amount of time to yield for</param>
    /// <returns>Unit</returns>
    [Pure]
    [MethodImpl(Opt.Default)]
    public static IO<Unit> yield(double milliseconds) =>
        IO<Unit>.Lift(env => yieldFor(new Duration(milliseconds), env.Token));

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Internal
    //
    
    /// <summary>
    /// Yields the thread for the `Duration` specified allowing for concurrency
    /// on the current thread without having to use async/await
    /// </summary>
    internal static Unit yieldFor(Duration d, CancellationToken token)
    {
        if (d == Duration.Zero) return default;
        var      start = TimeProvider.System.GetTimestamp();
        var      span  = (TimeSpan)d;
        SpinWait sw    = default;
        do
        {
            if (token.IsCancellationRequested) throw new TaskCanceledException();
            if (TimeProvider.System.GetElapsedTime(start) >= span) return default;
            sw.SpinOnce();
        } while (true);
    }
}
