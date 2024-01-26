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
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<MinRT> runtimeEff() =>
        LanguageExt.Eff<MinRT>.Lift(rt => rt);

    /// <summary>
    /// Create a new cancellation context and run the provided Aff in that context
    /// </summary>
    /// <param name="ma">Operation to run in the next context</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>An asynchronous effect that captures the operation running in context</returns>
    public static Eff<A> localCancelEff<A>(Eff<A> ma) =>
        lift((MinRT rt) =>
             {
                 var rt1 = rt.LocalCancel;
                 using (rt1.CancellationTokenSource)
                 {
                     return ma.Run(rt1);
                 }
             });
    

    /// <summary>
    /// Cancellation token
    /// </summary>
    /// <returns>CancellationToken</returns>
    public static Eff<CancellationToken> cancelTokenEff() =>
        lift(static (MinRT env) => env.CancellationToken);
    
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
        new(mma.Morphism.Map(ma => ma.Map(r => r.Morphism)).Flatten());
    
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
    public static Eff<(A First, B Second)> zip<A, B>(
         (Eff<A> First, Eff<B> Second) tuple) =>
         new(Transducer.zip(tuple.First.Morphism, tuple.Second.Morphism));

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
    public static Eff<(A First, B Second, C Third)> zip<A, B, C>(
        (Eff<A> First, Eff<B> Second, Eff<C> Third) tuple) =>
        new(Transducer.zip(tuple.First.Morphism, tuple.Second.Morphism, tuple.Third.Morphism));

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
    public static Eff<(A First, B Second, C Third, D Fourth)> zip<A, B, C, D>(
        (Eff<A> First,
            Eff<B> Second,
            Eff<C> Third,
            Eff<D> Fourth) tuple) =>
        new(Transducer.zip(tuple.First.Morphism, tuple.Second.Morphism, tuple.Third.Morphism, tuple.Fourth.Morphism));    

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
    public static Eff<(A First, B Second)> zip<A, B>(
        Eff<A> First,
        Eff<B> Second) =>
        (First, Second).Zip();

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
    public static Eff<(A First, B Second, C Third)> zip<A, B, C>(
        Eff<A> First, 
        Eff<B> Second, 
        Eff<C> Third) =>
        (First, Second, Third).Zip();

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
    public static Eff<(A First, B Second, C Third, D Fourth)> zip<A, B, C, D>(
        Eff<A> First,
        Eff<B> Second,
        Eff<C> Third,
        Eff<D> Fourth) =>
        new(Transducer.zip(First.Morphism, Second.Morphism, Third.Morphism, Fourth.Morphism));    

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Lifting
    //
    
    
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
    public static Eff<A> liftEff<A>(Func<Sum<Error, A>> f) =>
        LanguageExt.Eff<A>.Lift(f);

    /// <summary>
    /// Lift a asynchronous effect into the IO monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<A> liftEff<A>(Func<Task<Sum<Error, A>>> f) =>
        LanguageExt.Eff<A>.LiftIO(f);

    /// <summary>
    /// Lift a asynchronous effect into the IO monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<A> liftEff<A>(Func<Task<Either<Error, A>>> f) =>
        LanguageExt.Eff<A>.LiftIO(f);

    /// <summary>
    /// Lift a asynchronous effect into the IO monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<A> liftEff<A>(Func<Task<Fin<A>>> f) =>
        LanguageExt.Eff<A>.LiftIO(f);

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
    public static Eff<A> liftEff<A>(Func<MinRT, Fin<A>> f) =>
        LanguageExt.Eff<A>.Lift(f);

    /// <summary>
    /// Lift a synchronous effect into the IO monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<A> liftEff<A>(Transducer<MinRT, Either<Error, A>> f) =>
        LanguageExt.Eff<A>.Lift(f);

    /// <summary>
    /// Lift a synchronous effect into the IO monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<A> liftEff<A>(Transducer<MinRT, Fin<A>> f) =>
        LanguageExt.Eff<A>.Lift(f);

    /// <summary>
    /// Lift a synchronous effect into the IO monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<A> liftEff<A>(Func<MinRT, Sum<Error, A>> f) =>
        LanguageExt.Eff<A>.Lift(f);

    /// <summary>
    /// Lift a synchronous effect into the IO monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<A> liftEff<A>(Transducer<MinRT, Sum<Error, A>> f) =>
        LanguageExt.Eff<A>.Lift(f);

    /// <summary>
    /// Lift a asynchronous effect into the IO monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<A> liftEff<A>(Func<MinRT, Task<Sum<Error, A>>> f) =>
        LanguageExt.Eff<A>.LiftIO(f);

    /// <summary>
    /// Lift a asynchronous effect into the IO monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<A> liftEff<A>(Func<MinRT, Task<Either<Error, A>>> f) =>
        LanguageExt.Eff<A>.LiftIO(f);

    /// <summary>
    /// Lift a asynchronous effect into the IO monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<A> liftEff<A>(Func<MinRT, Task<Fin<A>>> f) =>
        LanguageExt.Eff<A>.LiftIO(f);

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Memoisation and tail-recursion
    //
    
    /// <summary>
    /// Memoise the result, so subsequent calls don't invoke the side-IOect
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<A> memo<A>(Eff<A> ma) =>
        ma.Memo();
    
    /// <summary>
    /// Wrap this around the final `from` call in a `IO` LINQ expression to void a recursion induced space-leak.
    /// </summary>
    /// <example>
    /// 
    ///     Eff<A> recursive(int x) =>
    ///         from x in writeLine(x)
    ///         from r in tail(recursive(x + 1))
    ///         select r;      <--- this never runs
    /// 
    /// </example>
    /// <remarks>
    /// This means the result of the LINQ expression comes from the final `from`, _not_ the `select.  If the
    /// type of the `final` from differs from the type of the `select` then this has no effect.
    /// </remarks>
    /// <remarks>
    /// Background: When making recursive LINQ expressions, the final `select` is problematic because it means there's code
    /// to run _after_ the final `from` expression.  This means there's you're guaranteed to have a space-leak due to the
    /// need to hold thunks to the final `select` on every recursive step.
    ///
    /// This function ignores the `select` altogether and says that the final `from` is where we get our return result
    /// from and therefore there's no need to hold the thunk. 
    /// </remarks>
    /// <param name="ma">IO operation</param>
    /// <typeparam name="E">Error type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>IO operation that's marked ready for tail recursion</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<A> tail<A>(Eff<A> ma) =>
        new(tail(ma.Morphism));
    
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
    public static Eff<ForkEff<A>> fork<A>(Eff<A> ma, Option<TimeSpan> timeout = default) => 
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
    public static Eff<B> map<A, B>(Eff<A> ma, Func<A, B> f) =>
        ma.Map(f);

    /// <summary>
    /// Maps the IO monad if it's in a success state
    /// </summary>
    /// <param name="f">Function to map the success value with</param>
    /// <returns>Mapped IO monad</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<B> map<A, B>(Eff<A> ma, Transducer<A, B> f) =>
        ma.Map(f);

    /// <summary>
    /// Maps the IO monad if it's in a success state
    /// </summary>
    /// <param name="f">Function to map the success value with</param>
    /// <returns>Mapped IO monad</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<A> mapFail<A>(Eff<A> ma, Func<Error, Error> f) =>
        ma.MapFail(f);

    /// <summary>
    /// Maps the IO monad if it's in a success state
    /// </summary>
    /// <param name="f">Function to map the success value with</param>
    /// <returns>Mapped IO monad</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<A> mapFail<A>(Eff<A> ma, Transducer<Error, Error> f) =>
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

    /// <summary>
    /// Mapping of either the Success state or the Failure state depending on what
    /// state this IO monad is in.  
    /// </summary>
    /// <param name="Succ">Mapping to use if the IO monad is in a success state</param>
    /// <param name="Fail">Mapping to use if the IO monad is in a failure state</param>
    /// <returns>Mapped IO monad</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<B> bimap<A, B>(Eff<A> ma, Transducer<A, B> Succ, Transducer<Error, Error> Fail) =>
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
    /// Pattern match the success or failure values and collapse them down to a success value
    /// </summary>
    /// <param name="Succ">Success value mapping</param>
    /// <param name="Fail">Failure value mapping</param>
    /// <returns>IO in a success state</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<B> match<A, B>(Eff<A> ma, Transducer<A, B> Succ, Transducer<Error, B> Fail) =>
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
    /// Map the failure to a success value
    /// </summary>
    /// <param name="f">Function to map the fail value</param>
    /// <returns>IO in a success state</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<A> ifFail<A>(Eff<A> ma, Transducer<Error, A> Fail) =>
        ma.IfFail(Fail);

    /// <summary>
    /// Map the failure to a success value
    /// </summary>
    /// <param name="f">Function to map the fail value</param>
    /// <returns>IO in a success state</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<A> ifFail<A>(Eff<A> ma, A Fail) =>
        ma.IfFail(Fail);

    /// <summary>
    /// Map the failure to a new IO effect
    /// </summary>
    /// <param name="f">Function to map the fail value</param>
    /// <returns>IO that encapsulates that IfFail</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<A> ifFailEff<A>(Eff<A> ma, Func<Error, Eff<A>> Fail) =>
        ma.IfFailEff(Fail);

    /// <summary>
    /// Map the failure to a new IO effect
    /// </summary>
    /// <param name="f">Function to map the fail value</param>
    /// <returns>IO that encapsulates that IfFail</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<A> ifFailEff<A>(Eff<A> ma, Eff<A> Fail) =>
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

    /// <summary>
    /// Only allow values through the effect if the predicate returns `true` for the bound value
    /// </summary>
    /// <param name="predicate">Predicate to apply to the bound value></param>
    /// <returns>Filtered IO</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<A> filter<A>(Eff<A> ma, Transducer<A, bool> predicate) =>
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
    public static Eff<A> post<A>(Eff<A> ma) =>
        new(Transducer.post(ma.Morphism));
    
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
    [Obsolete("Use either: `Eff<A>.Lift`, `Prelude.liftEff`, or `lift`")]
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<A> EffMaybe<A>(Func<Fin<A>> f) => 
        LanguageExt.Eff<A>.Lift(f);
    
    /// <summary>
    /// Construct an effect that will either succeed or have an exceptional failure
    /// </summary>
    /// <param name="f">Function to capture the effect</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Synchronous IO monad that captures the effect</returns>
    [Obsolete("Use either: `Eff<A>.Lift`, `Prelude.liftEff`, or `lift`")]
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<A> Eff<A>(Func<A> f) => 
        LanguageExt.Eff<A>.Lift(f);
}
