using System;
using LanguageExt.Common;
using System.Threading.Tasks;
using LanguageExt.Effects.Traits;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Threading;

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
    public static Eff<RT, A> SuccessEff<RT, A>(A value) 
        where RT : HasIO<RT, Error> =>
        LanguageExt.Eff<RT, A>.Pure(value);

    /// <summary>
    /// Construct a failed effect
    /// </summary>
    /// <param name="error">Error that represents the failure</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Synchronous IO monad that captures the failure</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<RT, A> FailEff<RT, A>(Error error) 
        where RT : HasIO<RT, Error> =>
        LanguageExt.Eff<RT, A>.Fail(error);    
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Runtime helpers
    //

    /// <summary>
    /// Make the runtime into the bound value
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<RT, RT> runtimeEff<RT>()
        where RT : HasIO<RT, Error> =>
        LanguageExt.Eff<RT, RT>.Lift(rt => rt);

    /// <summary>
    /// Create a new cancellation context and run the provided Aff in that context
    /// </summary>
    /// <param name="ma">Operation to run in the next context</param>
    /// <typeparam name="RT">Runtime environment</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>An asynchronous effect that captures the operation running in context</returns>
    public static Eff<RT, A> localCancelEff<RT, A>(Eff<RT, A> ma) where RT : HasIO<RT, Error> =>
        lift((RT rt) =>
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
    /// <typeparam name="RT">Runtime environment</typeparam>
    /// <returns>CancellationToken</returns>
    public static Eff<RT, CancellationToken> cancelTokenEff<RT>()
        where RT : HasIO<RT, Error> =>
        lift(static (RT env) => env.CancellationToken);
 
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
    public static Eff<RT, A> flatten<RT, A>(Eff<RT, Eff<RT, A>> mma)
        where RT : HasIO<RT, Error> =>
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
    public static Eff<RT, (A First, B Second)> zip<RT, A, B>(
         (Eff<RT, A> First, Eff<RT, B> Second) tuple)
         where RT : HasIO<RT, Error> =>
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
    public static Eff<RT, (A First, B Second, C Third)> zip<RT, A, B, C>(
        (Eff<RT, A> First, 
              Eff<RT, B> Second, 
              Eff<RT, C> Third) tuple)
        where RT : HasIO<RT, Error> =>
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
    public static Eff<RT, (A First, B Second, C Third, D Fourth)> zip<RT, A, B, C, D>(
        (Eff<RT, A> First,
            Eff<RT, B> Second,
            Eff<RT, C> Third,
            Eff<RT, D> Fourth) tuple)
        where RT : HasIO<RT, Error> =>
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
    public static Eff<RT, (A First, B Second)> zip<RT, A, B>(
        Eff<RT, A> First,
        Eff<RT, B> Second)
        where RT : HasIO<RT, Error> =>
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
    public static Eff<RT, (A First, B Second, C Third)> zip<RT, A, B, C>(
        Eff<RT, A> First, 
        Eff<RT, B> Second, 
        Eff<RT, C> Third)
        where RT : HasIO<RT, Error> =>
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
    public static Eff<RT, (A First, B Second, C Third, D Fourth)> zip<RT, A, B, C, D>(
        Eff<RT, A> First, 
        Eff<RT, B> Second, 
        Eff<RT, C> Third, 
        Eff<RT, D> Fourth)
        where RT : HasIO<RT, Error> =>
        (First, Second, Third, Fourth).Zip();
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Lifting
    //

    /// <summary>
    /// Lift a synchronous effect into the IO monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<RT, A> liftEff<RT, A>(Func<RT, Either<Error, A>> f)
        where RT : HasIO<RT, Error> =>
        LanguageExt.Eff<RT, A>.Lift(f);

    /// <summary>
    /// Lift a synchronous effect into the IO monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<RT, A> liftEff<RT, A>(Func<RT, Fin<A>> f)
        where RT : HasIO<RT, Error> =>
        LanguageExt.Eff<RT, A>.Lift(f);

    /// <summary>
    /// Lift a synchronous effect into the IO monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<RT, A> liftEff<RT, A>(Transducer<RT, Either<Error, A>> f)
        where RT : HasIO<RT, Error> =>
        LanguageExt.Eff<RT, A>.Lift(f);

    /// <summary>
    /// Lift a synchronous effect into the IO monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<RT, A> liftEff<RT, A>(Transducer<RT, Fin<A>> f)
        where RT : HasIO<RT, Error> =>
        LanguageExt.Eff<RT, A>.Lift(f);

    /// <summary>
    /// Lift a synchronous effect into the IO monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<RT, A> liftEff<RT, A>(Func<RT, Sum<Error, A>> f)
        where RT : HasIO<RT, Error> =>
        LanguageExt.Eff<RT, A>.Lift(f);

    /// <summary>
    /// Lift a synchronous effect into the IO monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<RT, A> liftEff<RT, A>(Transducer<RT, Sum<Error, A>> f)
        where RT : HasIO<RT, Error> =>
        LanguageExt.Eff<RT, A>.Lift(f);

    /// <summary>
    /// Lift a asynchronous effect into the IO monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<RT, A> liftEff<RT, A>(Func<RT, Task<Sum<Error, A>>> f)
        where RT : HasIO<RT, Error> =>
        LanguageExt.Eff<RT, A>.LiftIO(f);

    /// <summary>
    /// Lift a asynchronous effect into the IO monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<RT, A> liftEff<RT, A>(Func<RT, Task<Either<Error, A>>> f)
        where RT : HasIO<RT, Error> =>
        LanguageExt.Eff<RT, A>.LiftIO(f);

    /// <summary>
    /// Lift a asynchronous effect into the IO monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<RT, A> liftEff<RT, A>(Func<RT, Task<Fin<A>>> f)
        where RT : HasIO<RT, Error> =>
        LanguageExt.Eff<RT, A>.LiftIO(f);

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Memoisation and tail-recursion
    //
    
    /// <summary>
    /// Memoise the result, so subsequent calls don't invoke the side-IOect
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<RT, A> memo<RT, A>(Eff<RT, A> ma)
        where RT : HasIO<RT, Error> =>
        ma.Memo();
    
    /// <summary>
    /// Wrap this around the final `from` call in a `IO` LINQ expression to void a recursion induced space-leak.
    /// </summary>
    /// <example>
    /// 
    ///     Eff<RT, A> recursive(int x) =>
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
    /// <typeparam name="RT">Runtime type</typeparam>
    /// <typeparam name="E">Error type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>IO operation that's marked ready for tail recursion</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<RT, A> tail<RT, A>(Eff<RT, A> ma)
        where RT : HasIO<RT, Error> =>
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
    public static Eff<RT, ForkEff<RT, A>> fork<RT, A>(Eff<RT, A> ma, Option<TimeSpan> timeout = default) 
        where RT : HasIO<RT, Error> =>
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
    public static Eff<RT, B> map<RT, A, B>(Eff<RT, A> ma, Func<A, B> f)
        where RT : HasIO<RT, Error> =>
        ma.Map(f);

    /// <summary>
    /// Maps the IO monad if it's in a success state
    /// </summary>
    /// <param name="f">Function to map the success value with</param>
    /// <returns>Mapped IO monad</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<RT, B> map<RT, A, B>(Eff<RT, A> ma, Transducer<A, B> f)
        where RT : HasIO<RT, Error> =>
        ma.Map(f);

    /// <summary>
    /// Maps the IO monad if it's in a success state
    /// </summary>
    /// <param name="f">Function to map the success value with</param>
    /// <returns>Mapped IO monad</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<RT, A> mapFail<RT, A>(Eff<RT, A> ma, Func<Error, Error> f)
        where RT : HasIO<RT, Error> =>
        ma.MapFail(f);

    /// <summary>
    /// Maps the IO monad if it's in a success state
    /// </summary>
    /// <param name="f">Function to map the success value with</param>
    /// <returns>Mapped IO monad</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<RT, A> mapFail<RT, A>(Eff<RT, A> ma, Transducer<Error, Error> f)
        where RT : HasIO<RT, Error> =>
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
    public static Eff<RT, B> bimap<RT, A, B>(Eff<RT, A> ma, Func<A, B> Succ, Func<Error, Error> Fail)
        where RT : HasIO<RT, Error> =>
        ma.BiMap(Succ, Fail);

    /// <summary>
    /// Mapping of either the Success state or the Failure state depending on what
    /// state this IO monad is in.  
    /// </summary>
    /// <param name="Succ">Mapping to use if the IO monad is in a success state</param>
    /// <param name="Fail">Mapping to use if the IO monad is in a failure state</param>
    /// <returns>Mapped IO monad</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<RT, B> bimap<RT, A, B>(Eff<RT, A> ma, Transducer<A, B> Succ, Transducer<Error, Error> Fail)
        where RT : HasIO<RT, Error> =>
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
    public static Eff<RT, B> match<RT, A, B>(Eff<RT, A> ma, Func<A, B> Succ, Func<Error, B> Fail)
        where RT : HasIO<RT, Error> =>
        ma.Match(Succ, Fail);

    /// <summary>
    /// Pattern match the success or failure values and collapse them down to a success value
    /// </summary>
    /// <param name="Succ">Success value mapping</param>
    /// <param name="Fail">Failure value mapping</param>
    /// <returns>IO in a success state</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<RT, B> match<RT, A, B>(Eff<RT, A> ma, Transducer<A, B> Succ, Transducer<Error, B> Fail)
        where RT : HasIO<RT, Error> =>
        ma.Match(Succ, Fail);

    /// <summary>
    /// Map the failure to a success value
    /// </summary>
    /// <param name="f">Function to map the fail value</param>
    /// <returns>IO in a success state</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<RT, A> ifFail<RT, A>(Eff<RT, A> ma, Func<Error, A> Fail)
        where RT : HasIO<RT, Error> =>
        ma.IfFail(Fail);

    /// <summary>
    /// Map the failure to a success value
    /// </summary>
    /// <param name="f">Function to map the fail value</param>
    /// <returns>IO in a success state</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<RT, A> ifFail<RT, A>(Eff<RT, A> ma, Transducer<Error, A> Fail)
        where RT : HasIO<RT, Error> =>
        ma.IfFail(Fail);

    /// <summary>
    /// Map the failure to a success value
    /// </summary>
    /// <param name="f">Function to map the fail value</param>
    /// <returns>IO in a success state</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<RT, A> ifFail<RT, A>(Eff<RT, A> ma, A Fail)
        where RT : HasIO<RT, Error> =>
        ma.IfFail(Fail);

    /// <summary>
    /// Map the failure to a new IO effect
    /// </summary>
    /// <param name="f">Function to map the fail value</param>
    /// <returns>IO that encapsulates that IfFail</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<RT, A> ifFailEff<RT, A>(Eff<RT, A> ma, Func<Error, Eff<RT, A>> Fail)
        where RT : HasIO<RT, Error> =>
        ma.IfFailEff(Fail);

    /// <summary>
    /// Map the failure to a new IO effect
    /// </summary>
    /// <param name="f">Function to map the fail value</param>
    /// <returns>IO that encapsulates that IfFail</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<RT, A> ifFailEff<RT, A>(Eff<RT, A> ma, Eff<RT, A> Fail)
        where RT : HasIO<RT, Error> =>
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
    public static Eff<RT, A> filter<RT, A>(Eff<RT, A> ma, Func<A, bool> predicate)
        where RT : HasIO<RT, Error> =>
        ma.Filter(predicate);

    /// <summary>
    /// Only allow values through the effect if the predicate returns `true` for the bound value
    /// </summary>
    /// <param name="predicate">Predicate to apply to the bound value></param>
    /// <returns>Filtered IO</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<RT, A> filter<RT, A>(Eff<RT, A> ma, Transducer<A, bool> predicate)
        where RT : HasIO<RT, Error> =>
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
    public static Eff<RT, A> post<RT, A>(Eff<RT, A> ma)
        where RT : HasIO<RT, Error> =>
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
    [Obsolete("Use either: `Eff<RT, A>.Lift`, `Prelude.liftEff`, or `Transducer.lift`")]
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<RT, A> EffMaybe<RT, A>(Func<RT, Fin<A>> f) 
        where RT : HasIO<RT, Error> =>
        LanguageExt.Eff<RT, A>.Lift(f);
    
    /// <summary>
    /// Construct an effect that will either succeed or have an exceptional failure
    /// </summary>
    /// <param name="f">Function to capture the effect</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Synchronous IO monad that captures the effect</returns>
    [Obsolete("Use either: `Eff<RT, A>.Lift`, `Prelude.liftEff`, or `Transducer.lift`")]
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<RT, A> Eff<RT, A>(Func<RT, A> f) 
        where RT : HasIO<RT, Error>  =>
        LanguageExt.Eff<RT, A>.Lift(f);
}
