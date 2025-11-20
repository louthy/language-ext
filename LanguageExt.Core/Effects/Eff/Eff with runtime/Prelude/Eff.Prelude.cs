using System;
using LanguageExt.Common;
using System.Threading.Tasks;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace LanguageExt;

public static partial class Prelude
{
    /// <summary>
    /// Construct a successful effect with a pure value
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
        liftEff<RT, RT>(identity);

    /// <summary>
    /// Get all the internal state of the `Eff`
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<RT, (RT Runtime, EnvIO EnvIO)> getState<RT>() =>
        new(LanguageExt.Eff<RT>.getState);

    /// <summary>
    /// Create a new cancellation context and run the provided Aff in that context
    /// </summary>
    /// <param name="ma">Operation to run in the next context</param>
    /// <typeparam name="RT">Runtime environment</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>An asynchronous effect that captures the operation running in context</returns>
    public static Eff<RT, A> localCancel<RT, A>(Eff<RT, A> ma) =>
        ma.LocalIO().As();

    /// <summary>
    /// Create a new local context for the environment by mapping the outer environment and then
    /// using the result as a new context when running the IO monad provided
    /// </summary>
    /// <param name="f">Function to map the outer environment into a new one to run `ma`</param>
    /// <param name="ma">IO monad to run in the new context</param>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<OuterRT, A> localEff<OuterRT, InnerRT, A>(Func<OuterRT, InnerRT> f, Eff<InnerRT, A> ma) =>
         // Get the current state of the Eff  
         from st in getState<OuterRT>()

         // Run the local operation
         from rs in IO.local(ma.effect.Run(f(st.Runtime))).As()
         
         // Ignore any changes to the state and just return the result of the local operation
         select rs;
 
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
    public static Eff<RT, A> liftEff<RT, A>(Func<RT, Task<A>> f) =>
        LanguageExt.Eff<RT, A>.LiftIO(f);

    /// <summary>
    /// Lift an effect into the `Eff` monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<RT, A> liftEff<RT, A>(Func<RT, Task<Fin<A>>> f) =>
        LanguageExt.Eff<RT, A>.LiftIO(f);

    /// <summary>
    /// Lift an effect into the `Eff` monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<RT, A> liftEff<RT, A>(IO<A> ma) =>
        LanguageExt.Eff<RT, A>.LiftIO(ma);

    
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
}
