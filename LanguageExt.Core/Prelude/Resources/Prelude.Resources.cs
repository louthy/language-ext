using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class Prelude
{
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
    public static K<M, A> use<M, A>(K<M, A> acquire, Action<A> release)
        where M : Monad<M> =>
        use(acquire, x => IO.lift(() =>
                                  {
                                      release(x);
                                      return unit;
                                  }));

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
        where M : Monad<M> =>
        acquire.Bind(
            val => IO.lift(
                env =>
                {
                    env.Resources.Acquire(val, release);
                    return val;
                }));

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
    public static K<M, A> use<M, A>(K<M, A> acquire)
        where M : Monad<M>
        where A : IDisposable =>
        acquire.Bind(
            val => IO.lift(env =>
                           {
                               env.Resources.Acquire(val);
                               return val;
                           }));

    /// <summary>
    /// Release the resource from the tracked IO environment
    /// </summary>
    /// <param name="value">Resource to release</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Unit</returns>
    [Pure]
    [MethodImpl(Opt.Default)]
    public static IO<Unit> release<A>(A value) =>
        new(env => env.Resources.Release(value).Run(env));

    /// <summary>
    /// The IO monad tracks resources automatically, this creates a local resource environment
    /// to run the `computation` in.  Once the computation has completed any resources acquired
    /// are automatically released.  Imagine this as the ultimate `using` statement.
    /// </summary>
    /// <param name="computation">Computation to run in a local scope</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Result of computation</returns>
    [Pure]
    [MethodImpl(Opt.Default)]
    public static K<M, A> local<M, A>(K<M, A> computation)
        where M : Monad<M> =>
        from mk in M.UnliftIO<A>()
        from io in mk(computation).Bracket()
        select io;    
    
    /// <summary>
    /// When acquiring, using, and releasing various resources, it can be quite convenient to write a function to manage
    /// the acquisition and releasing, taking a function of the acquired value that specifies an action to be performed
    /// in between.
    /// </summary>
    /// <param name="Acq">Acquire resource</param>
    /// <param name="Use">Function to use the acquired resource</param>
    /// <param name="Fin">Function to invoke to release the resource</param>
    [Pure]
    [MethodImpl(Opt.Default)]
    public static IO<C> bracket<A, B, C>(IO<A> Acq, Func<A, IO<C>> Use, Func<A, IO<B>> Fin) =>
        Acq.As().Bracket(Use, Fin);
    
    /// <summary>
    /// When acquiring, using, and releasing various resources, it can be quite convenient to write a function to manage
    /// the acquisition and releasing, taking a function of the acquired value that specifies an action to be performed
    /// in between.
    /// </summary>
    /// <param name="Acq">Acquire resource</param>
    /// <param name="Use">Function to use the acquired resource</param>
    /// <param name="Fin">Function to invoke to release the resource</param>
    [Pure]
    [MethodImpl(Opt.Default)]
    public static K<M, C> bracket<M, A, B, C>(K<M, A> Acq, Func<A, K<M, C>> Use, Func<A, K<M, B>> Fin)
        where M : Monad<M> =>
        from mka in M.UnliftIO<A>()
        from mkb in M.UnliftIO<B>()
        from mkc in M.UnliftIO<C>()
        from res in mka(Acq).Bracket(x => mkc(Use(x)), x => mkb(Fin(x)))
        select res;

    /// <summary>
    /// When acquiring, using, and releasing various resources, it can be quite convenient to write a function to manage
    /// the acquisition and releasing, taking a function of the acquired value that specifies an action to be performed
    /// in between.
    /// </summary>
    /// <param name="Acq">Acquire resource</param>
    /// <param name="Use">Function to use the acquired resource</param>
    /// <param name="Err">Function to run to handle any exceptions</param>
    /// <param name="Fin">Function to invoke to release the resource</param>
    [Pure]
    [MethodImpl(Opt.Default)]
    public static IO<C> bracket<A, B, C>(K<IO, A> Acq, Func<A, IO<C>> Use, Func<Error, IO<C>> Err, Func<A, IO<B>> Fin) =>
        Acq.As().Bracket(Use, Err, Fin);

    /// <summary>
    /// When acquiring, using, and releasing various resources, it can be quite convenient to write a function to manage
    /// the acquisition and releasing, taking a function of the acquired value that specifies an action to be performed
    /// in between.
    /// </summary>
    /// <param name="Acq">Acquire resource</param>
    /// <param name="Use">Function to use the acquired resource</param>
    /// <param name="Err">Function to run to handle any exceptions</param>
    /// <param name="Fin">Function to invoke to release the resource</param>
    [Pure]
    [MethodImpl(Opt.Default)]
    public static K<M, C> bracket<M, A, B, C>(K<M, A> Acq, Func<A, K<M, C>> Use, Func<Error, K<M, C>> Err, Func<A, K<M, B>> Fin)
        where M : Monad<M> =>
        from mka in M.UnliftIO<A>()
        from mkb in M.UnliftIO<B>()
        from mkc in M.UnliftIO<C>()
        from res in mka(Acq).Bracket(x => mkc(Use(x)), x => mkc(Err(x)), x => mkb(Fin(x)))
        select res;
}
