using System;
using LanguageExt.Common;
using System.Threading.Tasks;
using LanguageExt.Effects.Traits;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class Prelude
{
    /// <summary>
    /// Construct an successful effect with a pure value
    /// </summary>
    /// <param name="value">Pure value to construct the monad with</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Synchronous IO monad that captures the pure value</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<RT, A> SuccessEff<RT, A>(A value) =>
        LanguageExt.Eff<RT, A>.Pure(value);

    /// <summary>
    /// Construct a failed effect
    /// </summary>
    /// <param name="error">Error that represents the failure</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Synchronous IO monad that captures the failure</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<RT, A> FailEff<RT, A>(Error error) =>
        LanguageExt.Eff<RT, A>.Fail(error);    
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Runtime helpers
    //

    /// <summary>
    /// Make the runtime into the bound value
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<RT, RT> runtime<RT>() =>
        liftEff<RT, RT>(rt => rt);

    /// <summary>
    /// Create a new cancellation context and run the provided Aff in that context
    /// </summary>
    /// <param name="ma">Operation to run in the next context</param>
    /// <typeparam name="RT">Runtime environment</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>An asynchronous effect that captures the operation running in context</returns>
    public static Eff<RT, A> localCancel<RT, A>(Eff<RT, A> ma) where RT : HasIO<RT> =>
        liftEff<RT, A>(
             rt =>
             {
                 var rt1 = rt.WithIO(rt.EnvIO.LocalCancel);
                 using (rt1.EnvIO.Source)
                 {
                     return ma.Run(rt1);
                 }
             });

    /// <summary>
    /// Create a new local context for the environment by mapping the outer environment and then
    /// using the result as a new context when running the IO monad provided
    /// </summary>
    /// <param name="f">Function to map the outer environment into a new one to run `ma`</param>
    /// <param name="ma">IO monad to run in the new context</param>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<OuterRT, A> localEff<OuterRT, InnerRT, A>(Func<OuterRT, InnerRT> f, Eff<InnerRT, A> ma)
        where InnerRT : HasIO<InnerRT>
        where OuterRT : HasIO<OuterRT> =>
        new((from irt in State.gets<StateT<OuterRT, ResourceT<IO>>, OuterRT, InnerRT>(f)
             let ires = ma.RunUnsafe(irt)
             select ires.Value).As());
 
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
    public static Eff<RT, A> flatten<RT, A>(Eff<RT, Eff<RT, A>> mma) =>
        mma.Bind(x => x);
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Zipping
    //

    /// <summary>
    /// Takes two IO monads and zips their result
    /// </summary>
    /// <remarks>
    /// Asynchronous operations will run concurrently
    /// </remarks>
    /// <param name="tuple">Tuple of IO monads to run</param>
    /// <typeparam name="RT">Runtime type</typeparam>
    /// <typeparam name="E">Error type</typeparam>
    /// <typeparam name="A">First IO monad bound value type</typeparam>
    /// <typeparam name="B">Second IO monad bound value type</typeparam>
    /// <returns>IO monad</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<RT, (A First, B Second)> zip<RT, A, B>(
        (Eff<RT, A> First, Eff<RT, B> Second) tuple) =>
        from e1 in tuple.First.Fork()
        from e2 in tuple.Second.Fork()
        from r1 in e1.Await
        from r2 in e2.Await
        select (r1, r2);

    /// <summary>
    /// Takes two IO monads and zips their result
    /// </summary>
    /// <remarks>
    /// Asynchronous operations will run concurrently
    /// </remarks>
    /// <param name="tuple">Tuple of IO monads to run</param>
    /// <typeparam name="RT">Runtime type</typeparam>
    /// <typeparam name="E">Error type</typeparam>
    /// <typeparam name="A">First IO monad bound value type</typeparam>
    /// <typeparam name="B">Second IO monad bound value type</typeparam>
    /// <typeparam name="C">Third IO monad bound value type</typeparam>
    /// <returns>IO monad</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<RT, (A First, B Second, C Third)> zip<RT, A, B, C>(
        (Eff<RT, A> First,
         Eff<RT, B> Second,
         Eff<RT, C> Third) tuple) =>
        from e1 in tuple.First.Fork()
        from e2 in tuple.Second.Fork()
        from e3 in tuple.Third.Fork()
        from r1 in e1.Await
        from r2 in e2.Await
        from r3 in e3.Await
        select (r1, r2, r3);

    /// <summary>
    /// Takes two IO monads and zips their result
    /// </summary>
    /// <remarks>
    /// Asynchronous operations will run concurrently
    /// </remarks>
    /// <param name="tuple">Tuple of IO monads to run</param>
    /// <typeparam name="RT">Runtime type</typeparam>
    /// <typeparam name="E">Error type</typeparam>
    /// <typeparam name="A">First IO monad bound value type</typeparam>
    /// <typeparam name="B">Second IO monad bound value type</typeparam>
    /// <typeparam name="C">Third IO monad bound value type</typeparam>
    /// <typeparam name="D">Fourth IO monad bound value type</typeparam>
    /// <returns>IO monad</returns>
    public static Eff<RT, (A First, B Second, C Third, D Fourth)> zip<RT, A, B, C, D>(
        (Eff<RT, A> First,
         Eff<RT, B> Second,
         Eff<RT, C> Third,
         Eff<RT, D> Fourth) tuple) =>
        from e1 in tuple.First.Fork()
        from e2 in tuple.Second.Fork()
        from e3 in tuple.Third.Fork()
        from e4 in tuple.Fourth.Fork()
        from r1 in e1.Await
        from r2 in e2.Await
        from r3 in e3.Await
        from r4 in e4.Await
        select (r1, r2, r3, r4);
    
    /// <summary>
    /// Takes two IO monads and zips their result
    /// </summary>
    /// <remarks>
    /// Asynchronous operations will run concurrently
    /// </remarks>
    /// <typeparam name="RT">Runtime type</typeparam>
    /// <typeparam name="E">Error type</typeparam>
    /// <typeparam name="A">First IO monad bound value type</typeparam>
    /// <typeparam name="B">Second IO monad bound value type</typeparam>
    /// <returns>IO monad</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<RT, (A First, B Second)> zip<RT, A, B>(
        Eff<RT, A> First,
        Eff<RT, B> Second) =>
        (First, Second).Zip();

    /// <summary>
    /// Takes two IO monads and zips their result
    /// </summary>
    /// <remarks>
    /// Asynchronous operations will run concurrently
    /// </remarks>
    /// <typeparam name="RT">Runtime type</typeparam>
    /// <typeparam name="E">Error type</typeparam>
    /// <typeparam name="A">First IO monad bound value type</typeparam>
    /// <typeparam name="B">Second IO monad bound value type</typeparam>
    /// <typeparam name="C">Third IO monad bound value type</typeparam>
    /// <returns>IO monad</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<RT, (A First, B Second, C Third)> zip<RT, A, B, C>(
        Eff<RT, A> First, 
        Eff<RT, B> Second, 
        Eff<RT, C> Third) =>
        (First, Second, Third).Zip();
    
    /// <summary>
    /// Takes two IO monads and zips their result
    /// </summary>
    /// <remarks>
    /// Asynchronous operations will run concurrently
    /// </remarks>
    /// <typeparam name="RT">Runtime type</typeparam>
    /// <typeparam name="E">Error type</typeparam>
    /// <typeparam name="A">First IO monad bound value type</typeparam>
    /// <typeparam name="B">Second IO monad bound value type</typeparam>
    /// <typeparam name="C">Third IO monad bound value type</typeparam>
    /// <typeparam name="D">Fourth IO monad bound value type</typeparam>
    /// <returns>IO monad</returns>
    public static Eff<RT, (A First, B Second, C Third, D Fourth)> zip<RT, A, B, C, D>(
        Eff<RT, A> First, 
        Eff<RT, B> Second, 
        Eff<RT, C> Third, 
        Eff<RT, D> Fourth) =>
        (First, Second, Third, Fourth).Zip();
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Lifting
    //

    /// <summary>
    /// Lift an effect into the `Eff` monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<RT, A> liftEff<RT, A>(Func<RT, Either<Error, A>> f) =>
        LanguageExt.Eff<RT, A>.Lift(f);

    /// <summary>
    /// Lift an effect into the `Eff` monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<RT, A> liftEff<RT, A>(Func<RT, Fin<A>> f) =>
        LanguageExt.Eff<RT, A>.Lift(f);

    /// <summary>
    /// Lift an effect into the `Eff` monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<RT, A> liftEff<RT, A>(Func<RT, A> f) =>
        LanguageExt.Eff<RT, A>.Lift(f);

    /// <summary>
    /// Lift an effect into the `Eff` monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<RT, A> liftEff<RT, A>(Func<RT, ValueTask<A>> f) =>
        LanguageExt.Eff<RT, A>.LiftIO(f);

    /// <summary>
    /// Lift an effect into the `Eff` monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<RT, A> liftEff<RT, A>(IO<A> ma) =>
        LanguageExt.Eff<RT, A>.LiftIO(ma);

    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // 
    // Forking
    //

    /// <summary>
    /// Queue this IO operation to run on the thread-pool. 
    /// </summary>
    /// <param name="timeout">Maximum time that the forked IO operation can run for. `None` for no timeout.</param>
    /// <returns>Returns a `ForkIO` data-structure that contains two IO effects that can be used to either cancel
    /// the forked IO operation or to await the result of it.
    /// </returns>
    [MethodImpl(Opt.Default)]
    public static Eff<RT, ForkIO<A>> fork<RT, A>(Eff<RT, A> ma, Option<TimeSpan> timeout = default) =>
        ma.Fork(timeout);    
    
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
    public static Eff<RT, B> map<RT, A, B>(Eff<RT, A> ma, Func<A, B> f) =>
        ma.Map(f);

    /// <summary>
    /// Maps the IO monad if it's in a success state
    /// </summary>
    /// <param name="f">Function to map the success value with</param>
    /// <returns>Mapped IO monad</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<RT, A> mapFail<RT, A>(Eff<RT, A> ma, Func<Error, Error> f) =>
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
    public static Eff<RT, B> bimap<RT, A, B>(Eff<RT, A> ma, Func<A, B> Succ, Func<Error, Error> Fail) =>
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
    public static Eff<RT, B> match<RT, A, B>(Eff<RT, A> ma, Func<A, B> Succ, Func<Error, B> Fail) =>
        ma.Match(Succ, Fail);

    /// <summary>
    /// Map the failure to a success value
    /// </summary>
    /// <param name="f">Function to map the fail value</param>
    /// <returns>IO in a success state</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<RT, A> ifFail<RT, A>(Eff<RT, A> ma, Func<Error, A> Fail) =>
        ma.IfFail(Fail);

    /// <summary>
    /// Map the failure to a new IO effect
    /// </summary>
    /// <param name="f">Function to map the fail value</param>
    /// <returns>IO that encapsulates that IfFail</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<RT, A> ifFailEff<RT, A>(Eff<RT, A> ma, Func<Error, Eff<RT, A>> Fail) =>
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
    public static Eff<RT, A> filter<RT, A>(Eff<RT, A> ma, Func<A, bool> predicate) =>
        ma.Filter(predicate);

    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Synchronisation between contexts
    //
    
    /// <summary>
    /// Make a transducer run on the `SynchronizationContext` that was captured at the start
    /// of an `Invoke` call.
    /// </summary>
    /// <remarks>
    /// The transducer receives its input value from the currently running sync-context and
    /// then proceeds to run its operation in the captured `SynchronizationContext`:
    /// typically a UI context, but could be any captured context.  The result of the
    /// transducer is the received back on the currently running sync-context. 
    /// </remarks>
    /// <param name="f">Transducer</param>
    /// <returns></returns>
    public static Eff<RT, A> post<RT, A>(Eff<RT, A> ma) =>
        ma.Post();
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Obsolete
    //
    
    /// <summary>
    /// Construct an effect that will either succeed, have an exceptional, or unexceptional failure
    /// </summary>
    /// <param name="f">Function to capture the effect</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Synchronous IO monad that captures the effect</returns>
    [Obsolete("Use either: `Prelude.lift` or `Eff<RT, A>.Lift`")]
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<RT, A> EffMaybe<RT, A>(Func<RT, Fin<A>> f) =>
        LanguageExt.Eff<RT, A>.Lift(f);
    
    /// <summary>
    /// Construct an effect that will either succeed or have an exceptional failure
    /// </summary>
    /// <param name="f">Function to capture the effect</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Synchronous IO monad that captures the effect</returns>
    [Obsolete("Use either: `Prelude.lift` or `Eff<RT, A>.Lift`")]
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<RT, A> Eff<RT, A>(Func<RT, A> f) =>
        LanguageExt.Eff<RT, A>.Lift(f);
}
