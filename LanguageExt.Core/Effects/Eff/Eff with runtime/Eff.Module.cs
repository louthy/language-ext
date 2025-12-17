using System;
using LanguageExt.Common;
using System.Threading.Tasks;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using static LanguageExt.Prelude;

namespace LanguageExt;

public partial class Eff
{
    /// <summary>
    /// Construct a successful effect with a pure value
    /// </summary>
    /// <param name="value">Pure value to construct the monad with</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Synchronous IO monad that captures the pure value</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<RT, A> Success<RT, A>(A value) =>
        Eff<RT, A>.Pure(value);

    /// <summary>
    /// Construct a failed effect
    /// </summary>
    /// <param name="error">Error that represents the failure</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Synchronous IO monad that captures the failure</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<RT, A> Fail<RT, A>(Error error) =>
        Eff<RT, A>.Fail(error);

    /// <summary>
    /// Unit effect
    /// </summary>
    public static Eff<RT, Unit> unit<RT>() =>
        Success<RT, Unit>(default);

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
        new(Eff<RT>.getState);

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
    public static Eff<OuterRT, A> local<OuterRT, InnerRT, A>(Func<OuterRT, InnerRT> f, Eff<InnerRT, A> ma) =>
         // Get the current state of the Eff  
         from st in getState<OuterRT>()

         // Run the local operation
         from rs in IO.local(ma.effect.Run(f(st.Runtime))).As()
         
         // Ignore any changes to the state and just return the result of the local operation
         select rs;
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Lifting
    //

    /// <summary>
    /// Lift an effect into the `Eff` monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<RT, A> lift<RT, A>(Func<EnvIO, RT, Either<Error, A>> f) =>
        +envIO.Bind(e => Eff<RT, A>.Lift(rt => f(e, rt)));

    /// <summary>
    /// Lift an effect into the `Eff` monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<RT, A> lift<RT, A>(Func<RT, Fin<A>> f) =>
        Eff<RT, A>.Lift(f);

    /// <summary>
    /// Lift an effect into the `Eff` monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<RT, A> lift<RT, A>(Func<EnvIO, RT, Fin<A>> f) =>
        +envIO.Bind(e => Eff<RT, A>.Lift(rt => f(e, rt)));

    /// <summary>
    /// Lift an effect into the `Eff` monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<RT, A> lift<RT, A>(Func<RT, A> f) =>
        Eff<RT, A>.Lift(f);

    /// <summary>
    /// Lift an effect into the `Eff` monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<RT, A> lift<RT, A>(Func<EnvIO, RT, A> f) =>
        +envIO.Bind(e => Eff<RT, A>.Lift(rt => f(e, rt)));

    /// <summary>
    /// Lift an effect into the `Eff` monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<RT, A> lift<RT, A>(Func<RT, Task<A>> f) =>
        Eff<RT, A>.LiftIO(f);

    /// <summary>
    /// Lift an effect into the `Eff` monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<RT, A> lift<RT, A>(Func<EnvIO, RT, Task<A>> f) =>
        +envIO.Bind(e => Eff<RT, A>.LiftIO(rt => f(e, rt)));

    /// <summary>
    /// Lift an effect into the `Eff` monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<RT, A> lift<RT, A>(Func<RT, Task<Fin<A>>> f) =>
        Eff<RT, A>.LiftIO(f);

    /// <summary>
    /// Lift an effect into the `Eff` monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<RT, A> lift<RT, A>(Func<EnvIO, RT, Task<Fin<A>>> f) =>
        +envIO.Bind(e => Eff<RT, A>.LiftIO(rt => f(e, rt)));

    /// <summary>
    /// Lift an effect into the `Eff` monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<RT, A> lift<RT, A>(IO<A> ma) =>
        Eff<RT, A>.LiftIO(ma);
}
