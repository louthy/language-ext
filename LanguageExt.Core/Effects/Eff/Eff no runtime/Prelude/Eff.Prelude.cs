using System;
using LanguageExt.Common;
using System.Threading.Tasks;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Threading;
using LanguageExt.Effects;

namespace LanguageExt;

public static partial class Prelude
{
    /// <summary>
    /// Effect that always returns `unit`.
    /// </summary>
    public static readonly Eff<Unit> unitEff = Pure(unit);
    
    /// <summary>
    /// Timeout operation if it takes too long
    /// </summary>
    [Pure]
    [MethodImpl(Opt.Default)]
    public static Eff<A> timeout<A>(TimeSpan timeout, Eff<A> ma) =>
        ma.MapIO(io => io.Timeout(timeout));
    
    /// <summary>
    /// Construct an successful effect with a pure value
    /// </summary>
    /// <param name="value">Pure value to construct the monad with</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Synchronous IO monad that captures the pure value</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<A> SuccessEff<A>(A value) => 
        LanguageExt.Eff<A>.Pure(value);

    /// <summary>
    /// Construct a failed effect
    /// </summary>
    /// <param name="error">Error that represents the failure</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Synchronous IO monad that captures the failure</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<A> FailEff<A>(Error error) => 
        LanguageExt.Eff<A>.Fail(error);    
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Runtime helpers
    //

    /// <summary>
    /// Make the runtime into the bound value
    /// </summary>
    [Pure]
    internal static Eff<MinRT> runtimeMinRT =>
        LanguageExt.Eff<MinRT>.Lift(rt => rt);

    /// <summary>
    /// Create a new cancellation context and run the provided Aff in that context
    /// </summary>
    /// <param name="ma">Operation to run in the next context</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>An asynchronous effect that captures the operation running in context</returns>
    public static Eff<A> localCancel<A>(Eff<A> ma) =>
        localCancel(ma.effect).As();

    /// <summary>
    /// Cancellation token
    /// </summary>
    /// <returns>CancellationToken</returns>
    public static Eff<CancellationToken> cancelTokenEff =>
        IO.token;
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Monadic join
    //

    /// <summary>
    /// Monadic join operator 
    /// </summary>
    /// <remarks>
    /// Collapses a nested IO monad so there is no nesting.
    /// </remarks>
    /// <param name="mma">Nest IO monad to flatten</param>
    /// <typeparam name="RT">Runtime</typeparam>
    /// <typeparam name="E">Error type</typeparam>
    /// <typeparam name="A">Bound value</typeparam>
    /// <returns>Flattened IO monad</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<A> flatten<A>(Eff<Eff<A>> mma) =>
        mma.Flatten(); 
            
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Lifting
    //

    /// <summary>
    /// Lift a synchronous effect into the IO monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<Unit> liftEff(Action action) =>
        LanguageExt.Eff<Unit>.Lift(() => { action(); return unit; });

    /// <summary>
    /// Lift a synchronous effect into the IO monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<A> liftEff<A>(Func<A> f) =>
        LanguageExt.Eff<A>.Lift(f);
    
    /// <summary>
    /// Lift a synchronous effect into the IO monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<A> liftEff<A>(Func<Either<Error, A>> f) =>
        LanguageExt.Eff<A>.Lift(f);

    /// <summary>
    /// Lift a synchronous effect into the IO monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<A> liftEff<A>(Func<Fin<A>> f) =>
        LanguageExt.Eff<A>.Lift(f);

    /// <summary>
    /// Lift a synchronous effect into the IO monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<A> liftEff<A>(Func<MinRT, Either<Error, A>> f) =>
        LanguageExt.Eff<A>.Lift(f);

    /// <summary>
    /// Lift a synchronous effect into the IO monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<A> liftEff<A>(Func<MinRT, A> f) =>
        LanguageExt.Eff<A>.Lift(f);

    /// <summary>
    /// Lift a synchronous effect into the IO monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<A> liftEff<A>(Func<MinRT, Fin<A>> f) =>
        LanguageExt.Eff<A>.Lift(f);

    /// <summary>
    /// Lift a asynchronous effect into the IO monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<A> liftEff<A>(Func<MinRT, Task<A>> f) =>
        LanguageExt.Eff<A>.LiftIO(f);

    /// <summary>
    /// Lift a asynchronous effect into the IO monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<A> liftEff<A>(Func<MinRT, Task<Fin<A>>> f) =>
        LanguageExt.Eff<A>.LiftIO(f);

    /// <summary>
    /// Lift a asynchronous effect into the IO monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<A> liftEff<A>(Func<Task<A>> f) =>
        LanguageExt.Eff<A>.LiftIO(f);

    /// <summary>
    /// Lift a asynchronous effect into the IO monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<A> liftEff<A>(Func<Task<Fin<A>>> f) =>
        LanguageExt.Eff<A>.LiftIO(f);
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Map and map-left
    //

    /// <summary>
    /// Maps the IO monad if it's in a success state
    /// </summary>
    /// <param name="f">Function to map the success value with</param>
    /// <returns>Mapped IO monad</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<B> map<A, B>(Eff<A> ma, Func<A, B> f) =>
        ma.Map(f);

    /// <summary>
    /// Maps the IO monad if it's in a success state
    /// </summary>
    /// <param name="f">Function to map the success value with</param>
    /// <returns>Mapped IO monad</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<A> mapFail<A>(Eff<A> ma, Func<Error, Error> f) =>
        ma.MapFail(f);

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Bi-map
    //

    /// <summary>
    /// Mapping of either the Success state or the Failure state depending on what
    /// state this IO monad is in.  
    /// </summary>
    /// <param name="Succ">Mapping to use if the IO monad is in a success state</param>
    /// <param name="Fail">Mapping to use if the IO monad is in a failure state</param>
    /// <returns>Mapped IO monad</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<B> bimap<A, B>(Eff<A> ma, Func<A, B> Succ, Func<Error, Error> Fail) =>
        ma.BiMap(Succ, Fail);

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Matching
    //

    /// <summary>
    /// Pattern match the success or failure values and collapse them down to a success value
    /// </summary>
    /// <param name="Succ">Success value mapping</param>
    /// <param name="Fail">Failure value mapping</param>
    /// <returns>IO in a success state</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<B> match<A, B>(Eff<A> ma, Func<A, B> Succ, Func<Error, B> Fail) =>
        ma.Match(Succ, Fail);

    /// <summary>
    /// Map the failure to a success value
    /// </summary>
    /// <param name="f">Function to map the fail value</param>
    /// <returns>IO in a success state</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<A> ifFail<A>(Eff<A> ma, Func<Error, A> Fail) =>
        ma.IfFail(Fail);

    /// <summary>
    /// Map the failure to a new IO effect
    /// </summary>
    /// <param name="f">Function to map the fail value</param>
    /// <returns>IO that encapsulates that IfFail</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<A> ifFailEff<A>(Eff<A> ma, Func<Error, Eff<A>> Fail) =>
        ma.IfFailEff(Fail);
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Filter
    //

    /// <summary>
    /// Only allow values through the effect if the predicate returns `true` for the bound value
    /// </summary>
    /// <param name="predicate">Predicate to apply to the bound value></param>
    /// <returns>Filtered IO</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<A> filter<A>(Eff<A> ma, Func<A, bool> predicate) =>
        ma.Filter(predicate);
}
