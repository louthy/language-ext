using System;
using LanguageExt.DSL;
using System.Threading;
using LanguageExt.Common;
using LanguageExt.Traits;
using System.Threading.Tasks;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;

namespace LanguageExt;

public partial class IO
{
    /// <summary>
    /// Lift a pure value into an IO computation
    /// </summary>
    /// <param name="value">value</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>IO in a success state.  Always yields the lifted value.</returns>
    [Pure]
    public static IO<A> pure<A>(A value) =>
        new IOPure<A>(value);
    
    /// <summary>
    /// Lift a pure value into an IO computation
    /// </summary>
    /// <param name="value">value</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>IO in a success state.  Always yields the lifted value.</returns>
    [Pure]
    internal static IO<A> pureAsync<A>(Task<A> value) =>
        new IOPureAsync<A>(new ValueTask<A>(value));
    
    /// <summary>
    /// Lift a pure value into an IO computation
    /// </summary>
    /// <param name="value">value</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>IO in a success state.  Always yields the lifted value.</returns>
    [Pure]
    internal static IO<A> pureVAsync<A>(ValueTask<A> value) =>
        new IOPureAsync<A>(value);
    
    /// <summary>
    /// Put the IO into a failure state
    /// </summary>
    /// <param name="value">Error value</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>IO in a failed state.  Always yields an error.</returns>
    [Pure]
    public static IO<A> fail<A>(Error value) =>
        new IOFail<A>(value);
    
    /// <summary>
    /// Put the IO into a failure state
    /// </summary>
    /// <param name="value">Error value</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>IO in a failed state.  Always yields an error.</returns>
    [Pure]
    public static IO<A> fail<A>(string value) =>
        fail<A>(Error.New(value));
    
    /// <summary>
    /// Lift an action into the IO monad
    /// </summary>
    /// <param name="f">Action to lift</param>
    [Pure]
    public static IO<Unit> lift(Action f) =>
        lift(() =>
             {
                 f();
                 return unit;
             });
    
    [Pure]
    public static IO<A> lift<A>(Either<Error, A> ma) =>
        ma switch
        {
            Either<Error, A>.Right (var r) => pure(r),
            Either<Error, A>.Left (var l)  => fail<A>(l),
            _                              => fail<A>(Errors.Bottom)
        };
    
    [Pure]
    public static IO<A> lift<A>(Fin<A> ma) =>
        lift(ma.ToEither());
    
    [Pure]
    public static IO<A> lift<A>(Func<A> f) => 
        new IOLiftSync<A, A>(_ => f(), pure);
    
    [Pure]
    public static IO<A> lift<A>(Func<EnvIO, A> f) => 
        new IOLiftSync<A, A>(f, pure);
    
    [Pure]
    public static IO<A> lift<A>(Func<Fin<A>> f) => 
        lift(() => f().ThrowIfFail());
    
    [Pure]
    public static IO<A> lift<A>(Func<EnvIO, Fin<A>> f) => 
        lift(e => f(e).ThrowIfFail());
    
    [Pure]
    public static IO<A> lift<A>(Func<Either<Error, A>> f) => 
        lift(() => f().ToFin().ThrowIfFail());
    
    [Pure]
    public static IO<A> lift<A>(Func<EnvIO, Either<Error, A>> f) => 
        lift(e => f(e).ToFin().ThrowIfFail());

    [Pure]
    public static IO<A> liftAsync<A>(Func<Task<A>> f) => 
        new IOLiftAsync<A, A>(_ => f(), pure);

    [Pure]
    public static IO<A> liftAsync<A>(Func<EnvIO, Task<A>> f) => 
        new IOLiftAsync<A, A>(f, pure);

    [Pure]
    public static IO<A> liftVAsync<A>(Func<ValueTask<A>> f) => 
        new IOLiftVAsync<A, A>(_ => f(), pure);

    [Pure]
    public static IO<A> liftVAsync<A>(Func<EnvIO, ValueTask<A>> f) => 
        new IOLiftVAsync<A, A>(f, pure);

    /// <summary>
    /// Wraps this computation in a local-environment that ignores any cancellation-token cancellation requests.
    /// </summary>
    /// <returns>An uninterruptible computation</returns>
    [Pure]
    public static IO<A> uninterruptible<A>(IO<A> ma) =>
        ma.Uninterruptible();

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
    [Pure]
    public static IO<A> local<A>(K<IO, A> ma) => 
        ma.As().Local();

    public static readonly IO<EnvIO> env = 
        lift(e => e);
    
    public static readonly IO<CancellationToken> token = 
        new IOToken<CancellationToken>(pure);
    
    public static readonly IO<CancellationTokenSource> source = 
        lift(e => e.Source);
    
    public static readonly IO<Option<SynchronizationContext>> syncContext = 
        lift(e => Optional(e.SyncContext));

    [Pure]
    public static IO<A> empty<A>() =>
        IO<A>.Empty;

    [Pure]
    public static IO<A> combine<A>(K<IO, A> ma, K<IO, A> mb) => 
        ma.As() | mb.As();

    /// <summary>
    /// Yield the thread for the specified duration or until cancelled.
    /// </summary>
    /// <param name="duration">Amount of time to yield for</param>
    /// <returns>Unit</returns>
    [Pure]
    public static IO<Unit> yieldFor(Duration duration) =>
        Math.Abs(duration.Milliseconds) < 0.00000001
            ? unitIO
            : liftAsync(e => yieldFor(duration, e.Token));

    /// <summary>
    /// Yield the thread for the specified duration or until cancelled.
    /// </summary>
    /// <param name="timeSpan">Amount of time to yield for</param>
    /// <returns>Unit</returns>
    [Pure]
    public static IO<Unit> yieldFor(TimeSpan timeSpan) =>
        Math.Abs(timeSpan.TotalMilliseconds) < 0.00000001
            ? unitIO
            : liftAsync(e => yieldFor(timeSpan, e.Token));

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Internal
    //
    
    /// <summary>
    /// Yields the thread for the `Duration` specified allowing for concurrency
    /// on the current thread 
    /// </summary>
    internal static async Task<Unit> yieldFor(Duration d, CancellationToken token)
    {
        await Task.Delay((TimeSpan)d, token);
        return unit;
    }
}
