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
    public static Eff<A> Success<A>(A value) =>
        Eff<A>.Pure(value);

    /// <summary>
    /// Construct a failed effect
    /// </summary>
    /// <param name="error">Error that represents the failure</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Synchronous IO monad that captures the failure</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<A> Fail<A>(Error error) =>
        Eff<A>.Fail(error);

    /// <summary>
    /// Unit effect
    /// </summary>
    public static Eff<Unit> unit() =>
        unitEff;

    /// <summary>
    /// Create a new cancellation context and run the provided Aff in that context
    /// </summary>
    /// <param name="ma">Operation to run in the next context</param>
    /// <typeparam name="RT">Runtime environment</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>An asynchronous effect that captures the operation running in context</returns>
    public static Eff<A> localCancel<A>(Eff<A> ma) =>
        ma.LocalIO().As();
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Lifting
    //

    /// <summary>
    /// Lift an effect into the `Eff` monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<A> lift<A>(Func<Either<Error, A>> f) =>
        Eff<A>.Lift(f);

    /// <summary>
    /// Lift an effect into the `Eff` monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<A> lift<A>(Func<Fin<A>> f) =>
        Eff<A>.Lift(f);

    /// <summary>
    /// Lift an effect into the `Eff` monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<A> lift<A>(Func<EnvIO, Fin<A>> f) =>
        +envIO.Bind(e => Eff<A>.Lift(() => f(e)));

    /// <summary>
    /// Lift an effect into the `Eff` monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<A> lift<A>(Func<A> f) =>
        Eff<A>.Lift(f);

    /// <summary>
    /// Lift an effect into the `Eff` monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<A> lift<A>(Func<EnvIO, A> f) =>
        +envIO.Bind(e => Eff<A>.Lift(() => f(e)));

    /// <summary>
    /// Lift an effect into the `Eff` monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<A> lift<A>(Func<Task<A>> f) =>
        Eff<A>.LiftIO(f);

    /// <summary>
    /// Lift an effect into the `Eff` monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<A> lift<A>(Func<EnvIO, Task<A>> f) =>
        +envIO.Bind(e => Eff<A>.LiftIO(() => f(e)));

    /// <summary>
    /// Lift an effect into the `Eff` monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<A> lift<A>(Func<Task<Fin<A>>> f) =>
        Eff<A>.LiftIO(f);

    /// <summary>
    /// Lift an effect into the `Eff` monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<A> lift<A>(Func<EnvIO, Task<Fin<A>>> f) =>
        +envIO.Bind(e => Eff<A>.LiftIO(() => f(e)));

    /// <summary>
    /// Lift an effect into the `Eff` monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<A> lift<A>(IO<A> ma) =>
        Eff<A>.LiftIO(ma);
}
