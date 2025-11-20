using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using LanguageExt.Common;
using LanguageExt.DSL;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class Prelude
{
    /// <summary>
    /// Acquire a resource and have it tracked by the IO environment.  The resource
    /// can be released manually using `release` or from wrapping a section of IO
    /// code with `bracketIO`.
    /// </summary>
    /// <param name="acquire">Computation that acquires the resource</param>
    /// <param name="release">Action to release the resource</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Acquired resource</returns>
    [Pure]
    [MethodImpl(Opt.Default)]
    public static K<M, A> use<M, A>(K<M, A> acquire, Action<A> release)
        where M : MonadUnliftIO<M> =>
        acquire.MapIO(
            acq => new IOUse<A, A>(
                acq,
                x => IO.lift(
                    () =>
                    {
                        release(x);
                        return unit;
                    }),
                IO.pure));

    /// <summary>
    /// Acquire a resource and have it tracked by the IO environment.  The resource
    /// can be released manually using `release` or from wrapping a section of IO
    /// code with `bracketIO`.
    /// </summary>
    /// <param name="acquire">Computation that acquires the resource</param>
    /// <param name="release">Action to release the resource</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Acquired resource</returns>
    [Pure]
    [MethodImpl(Opt.Default)]
    public static IO<A> use<A>(IO<A> acquire, Action<A> release) =>
        new IOUse<A, A>(
            acquire,
            x => IO.lift(
                () =>
                {
                    release(x);
                    return unit;
                }),
            IO.pure);

    /// <summary>
    /// Acquire a resource and have it tracked by the IO environment.  The resource
    /// can be released manually using `release` or from wrapping a section of IO
    /// code with `@using`.
    /// </summary>
    /// <param name="acquire">Computation that acquires the resource</param>
    /// <param name="release">Action to release the resource</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Acquired resource</returns>
    [Pure]
    [MethodImpl(Opt.Default)]
    public static IO<A> use<A>(Func<A> acquire, Action<A> release) =>
        use(IO.lift(acquire), release).As();

    /// <summary>
    /// Acquire a resource and have it tracked by the IO environment.  The resource
    /// can be released manually using `release` or from wrapping a section of IO
    /// code with `@using`.
    /// </summary>
    /// <param name="acquire">Computation that acquires the resource</param>
    /// <param name="release">Action to release the resource</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Acquired resource</returns>
    [Pure]
    [MethodImpl(Opt.Default)]
    public static IO<A> use<A>(Func<A> acquire, Func<A, IO<Unit>> release) =>
        use(IO.lift(acquire), release).As();

    /// <summary>
    /// Acquire a resource and have it tracked by the IO environment.  The resource
    /// can be released manually using `release` or from wrapping a section of IO
    /// code with `@using`.
    /// </summary>
    /// <param name="acquire">Computation that acquires the resource</param>
    /// <param name="release">Action to release the resource</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Acquired resource</returns>
    [Pure]
    [MethodImpl(Opt.Default)]
    public static K<M, A> use<M, A>(K<M, A> acquire, Func<A, IO<Unit>> release)
        where M : MonadUnliftIO<M> =>
        acquire.MapIO(acq => new IOUse<A, A>(acq, release, IO.pure));

    /// <summary>
    /// Acquire a resource and have it tracked by the IO environment.  The resource
    /// can be released manually using `release` or from wrapping a section of IO
    /// code with `@using`.
    /// </summary>
    /// <param name="acquire">Computation that acquires the resource</param>
    /// <param name="release">Action to release the resource</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Acquired resource</returns>
    [Pure]
    [MethodImpl(Opt.Default)]
    public static IO<A> use<A>(IO<A> acquire, Func<A, IO<Unit>> release) =>
        new IOUse<A, A>(acquire, release, IO.pure);

    /// <summary>
    /// Acquire a resource and have it tracked by the IO environment.  The resource
    /// can be released manually using `release` or from wrapping a section of IO
    /// code with `@using`.
    /// </summary>
    /// <param name="acquire">Computation that acquires the resource</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Acquired resource</returns>
    [Pure]
    [MethodImpl(Opt.Default)]
    public static IO<A> use<A>(Func<EnvIO, A> acquire) 
        where A : IDisposable =>
        use(IO.lift(acquire)).As();

    /// <summary>
    /// Acquire a resource and have it tracked by the IO environment.  The resource
    /// can be released manually using `release` or from wrapping a section of IO
    /// code with `@using`.
    /// </summary>
    /// <param name="acquire">Computation that acquires the resource</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Acquired resource</returns>
    [Pure]
    [MethodImpl(Opt.Default)]
    public static IO<A> use<A>(Func<A> acquire)
        where A : IDisposable =>
        new IOUseDisposable<A, A>(IO.lift(acquire), IO.pure);

    /// <summary>
    /// Acquire a resource and have it tracked by the IO environment.  The resource
    /// can be released manually using `release` or from wrapping a section of IO
    /// code with `@using`.
    /// </summary>
    /// <param name="acquire">Computation that acquires the resource</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Acquired resource</returns>
    [Pure]
    [MethodImpl(Opt.Default)]
    public static IO<A> useAsync<A>(Func<A> acquire)
        where A : IAsyncDisposable =>
        new IOUseAsyncDisposable<A, A>(IO.lift(acquire), IO.pure);

    /// <summary>
    /// Acquire a resource and have it tracked by the IO environment.  The resource
    /// can be released manually using `release` or from wrapping a section of IO
    /// code with `@using`.
    /// </summary>
    /// <param name="acquire">Computation that acquires the resource</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Acquired resource</returns>
    [Pure]
    [MethodImpl(Opt.Default)]
    public static K<M, A> use<M, A>(K<M, A> acquire)
        where M : MonadUnliftIO<M>
        where A : IDisposable =>
        acquire.MapIO(acq => new IOUseDisposable<A, A>(acq, IO.pure));

    /// <summary>
    /// Acquire a resource and have it tracked by the IO environment.  The resource
    /// can be released manually using `release` or from wrapping a section of IO
    /// code with `@using`.
    /// </summary>
    /// <param name="acquire">Computation that acquires the resource</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Acquired resource</returns>
    [Pure]
    [MethodImpl(Opt.Default)]
    public static IO<A> use<A>(IO<A> acquire)
        where A : IDisposable =>
        new IOUseDisposable<A, A>(acquire, IO.pure);

    /// <summary>
    /// Acquire a resource and have it tracked by the IO environment.  The resource
    /// can be released manually using `release` or from wrapping a section of IO
    /// code with `@using`.
    /// </summary>
    /// <param name="acquire">Computation that acquires the resource</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Acquired resource</returns>
    [Pure]
    [MethodImpl(Opt.Default)]
    public static K<M, A> useAsync<M, A>(K<M, A> acquire)
        where M : MonadUnliftIO<M>
        where A : IAsyncDisposable =>
        acquire.MapIO(acq => new IOUseAsyncDisposable<A, A>(acq, IO.pure));

    /// <summary>
    /// Acquire a resource and have it tracked by the IO environment.  The resource
    /// can be released manually using `release` or from wrapping a section of IO
    /// code with `@using`.
    /// </summary>
    /// <param name="acquire">Computation that acquires the resource</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Acquired resource</returns>
    [Pure]
    [MethodImpl(Opt.Default)]
    public static IO<A> useAsync<A>(IO<A> acquire)
        where A : IAsyncDisposable =>
        new IOUseAsyncDisposable<A, A>(acquire, IO.pure);

    /// <summary>
    /// Release the resource from the tracked IO environment
    /// </summary>
    /// <param name="value">Resource to release</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Unit</returns>
    [Pure]
    [MethodImpl(Opt.Default)]
    public static IO<Unit> release<A>(A value) =>
        IO.lift(env => env.Resources.Release(value).Run(env));

    /// <summary>
    /// The IO monad tracks resources automatically; this creates a local resource environment
    /// to run the `computation` in.  Once the computation is completed, any resources acquired
    /// are automatically released.  Imagine this as the ultimate `using` statement.
    /// </summary>
    /// <param name="computation">Computation to run in a local scope</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Result of computation</returns>
    [Pure]
    [MethodImpl(Opt.Default)]
    public static K<M, A> bracketIO<M, A>(K<M, A> computation)
        where M : MonadUnliftIO<M> =>
        computation.BracketIO();

    /// <summary>
    /// The IO monad tracks resources automatically; this creates a local resource environment
    /// to run the `computation` in.  Once the computation is completed, any resources acquired
    /// are automatically released.  Imagine this as the ultimate `using` statement.
    /// </summary>
    /// <param name="computation">Computation to run in a local scope</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Result of computation</returns>
    [Pure]
    [MethodImpl(Opt.Default)]
    public static IO<A> bracketIO<M, A>(IO<A> computation)
        where M : MonadUnliftIO<M> =>
        computation.Bracket();
    
    /// <summary>
    /// When acquiring, using, and releasing various resources, it can be quite convenient to write a function to manage
    /// the acquisition and releasing, taking a function of the acquired value that specifies an action to be performed
    /// in between.
    /// </summary>
    /// <remarks>
    /// Consider using `bracketIO(computation)` rather than `bracket(acq, use, fin)`, the semantics are more attractive
    /// as there's no need to provide function handlers, the cleanup is automatic. 
    /// </remarks>
    /// <param name="Acq">Acquire resource</param>
    /// <param name="Use">Function to use the acquired resource</param>
    /// <param name="Fin">Function to invoke to release the resource</param>
    [Pure]
    [MethodImpl(Opt.Default)]
    public static K<M, C> bracketIO<M, A, B, C>(K<M, A> Acq, Func<A, IO<C>> Use, Func<A, IO<B>> Fin)
        where M : MonadUnliftIO<M> =>
        Acq.BracketIO(Use, Fin);
    
    /// <summary>
    /// When acquiring, using, and releasing various resources, it can be quite convenient to write a function to manage
    /// the acquisition and releasing, taking a function of the acquired value that specifies an action to be performed
    /// in between.
    /// </summary>
    /// <remarks>
    /// Consider using `bracketIO(computation)` rather than `bracket(acq, use, fin)`, the semantics are more attractive
    /// as there's no need to provide function handlers, the cleanup is automatic. 
    /// </remarks>
    /// <param name="Acq">Acquire resource</param>
    /// <param name="Use">Function to use the acquired resource</param>
    /// <param name="Fin">Function to invoke to release the resource</param>
    [Pure]
    [MethodImpl(Opt.Default)]
    public static IO<C> bracketIO<M, A, B, C>(IO<A> Acq, Func<A, IO<C>> Use, Func<A, IO<B>> Fin)
        where M : MonadUnliftIO<M> =>
        Acq.Bracket(Use, Fin);
    
    /// <summary>
    /// When acquiring, using, and releasing various resources, it can be quite convenient to write a function to manage
    /// the acquisition and releasing, taking a function of the acquired value that specifies an action to be performed
    /// in between.
    /// </summary>
    /// <remarks>
    /// Consider using `bracketIO(computation)` rather than `bracket(acq, use, fin)`, the semantics are more attractive
    /// as there's no need to provide function handlers, the cleanup is automatic. 
    /// </remarks>
    /// <param name="Acq">Acquire resource</param>
    /// <param name="Use">Function to use the acquired resource</param>
    /// <param name="Catch">Function called when an `Error` is raised within the `Acq` computation</param>
    /// <param name="Finally">Function to invoke to release the resource</param>
    [Pure]
    [MethodImpl(Opt.Default)]
    public static K<M, C> bracketIO<M, A, B, C>(K<M, A> Acq, Func<A, IO<C>> Use, Func<Error, IO<C>> Catch, Func<A, IO<B>> Fin)
        where M : MonadUnliftIO<M> =>
        Acq.BracketIO(Use, Catch, Fin);
    
    /// <summary>
    /// When acquiring, using, and releasing various resources, it can be quite convenient to write a function to manage
    /// the acquisition and releasing, taking a function of the acquired value that specifies an action to be performed
    /// in between.
    /// </summary>
    /// <remarks>
    /// Consider using `bracketIO(computation)` rather than `bracket(acq, use, fin)`, the semantics are more attractive
    /// as there's no need to provide function handlers, the cleanup is automatic. 
    /// </remarks>
    /// <param name="Acq">Acquire resource</param>
    /// <param name="Use">Function to use the acquired resource</param>
    /// <param name="Catch">Function called when an `Error` is raised within the `Acq` computation</param>
    /// <param name="Finally">Function to invoke to release the resource</param>
    [Pure]
    [MethodImpl(Opt.Default)]
    public static IO<C> bracketIO<M, A, B, C>(IO<A> Acq, Func<A, IO<C>> Use, Func<Error, IO<C>> Catch, Func<A, IO<B>> Fin)
        where M : MonadIO<M> =>
        Acq.Bracket(Use, Catch, Fin);
    
}
