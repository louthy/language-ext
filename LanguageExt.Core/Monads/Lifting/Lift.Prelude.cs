using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class Prelude
{
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Asynchronous IO lifting
    //

    /// <summary>
    /// Lift an action
    /// </summary>
    /// <param name="action">Function</param>
    /// <returns>Value that can be used with monadic types in LINQ expressions</returns>
    public static Lift<Unit> lift(Action action) =>
        lift<Unit>(() =>
                   {
                       action();
                       return default;
                   });

    /// <summary>
    /// Lift a function 
    /// </summary>
    /// <param name="function">Function</param>
    /// <returns>Value that can be used with monadic types in LINQ expressions</returns>
    public static Lift<A> lift<A>(Func<A> function) =>
        new(function);

    /// <summary>
    /// Lift a function 
    /// </summary>
    /// <param name="function">Function</param>
    /// <returns>Value that can be used with monadic types in LINQ expressions</returns>
    public static Lift<A, B> lift<A, B>(Func<A, B> function) =>
        new(function);
    
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Asynchronous IO lifting
    //

    /// <summary>
    /// Lift a asynchronous IO action
    /// </summary>
    /// <param name="action">Function</param>
    /// <returns>Value that can be used with monadic types in LINQ expressions</returns>
    public static IO<Unit> liftIO(Func<Task> action) =>
        IO.liftAsync(async _ =>
                     {
                         await action().ConfigureAwait(false);
                         return unit;
                     });

    /// <summary>
    /// Lift a asynchronous IO action
    /// </summary>
    /// <param name="action">Action</param>
    /// <returns>Value that can be used with monadic types in LINQ expressions</returns>
    public static IO<Unit> liftIO(Func<EnvIO, Task> action) =>
        IO.liftAsync(async env =>
                     {
                         await action(env).ConfigureAwait(false);
                         return unit;
                     });

    /// <summary>
    /// Lift an asynchronous IO function 
    /// </summary>
    /// <param name="function">Function</param>
    /// <returns>Value that can be used with monadic types in LINQ expressions</returns>
    public static IO<A> liftIO<A>(Func<Task<A>> function) =>
        IO.liftAsync(async () => await function().ConfigureAwait(false));

    /// <summary>
    /// Lift an asynchronous IO function 
    /// </summary>
    /// <param name="function">Function</param>
    /// <returns>Value that can be used with monadic types in LINQ expressions</returns>
    public static IO<A> liftIO<A>(Func<EnvIO, Task<A>> function) =>
        IO.liftAsync<A>(async e => await function(e).ConfigureAwait(false));

    /// <summary>
    /// Lift an option into an `OptionT IO` 
    /// </summary>
    public static OptionT<IO, A> liftIO<A>(Option<A> ma) =>
        OptionT.lift<IO, A>(ma);

    /// <summary>
    /// Lift an Option into an `OptionT IO` 
    /// </summary>
    public static OptionT<IO, A> liftIO<A>(Task<Option<A>> ma) =>
        new(IO.liftAsync(async () => await ma.ConfigureAwait(false)));

    /// <summary>
    /// Lift an `Either` into a `EitherT IO` 
    /// </summary>
    public static EitherT<L, IO, A> liftIO<L, A>(Either<L, A> ma) =>
        EitherT.lift<L, IO, A>(ma);

    /// <summary>
    /// Lift an `Either` into a `EitherT IO` 
    /// </summary>
    public static EitherT<L, IO, A> liftIO<L, A>(Task<Either<L, A>> ma) =>
        new(IO.liftAsync(async () => await ma.ConfigureAwait(false)));

    /// <summary>
    /// Lift an `Either` into a `EitherT IO` 
    /// </summary>
    public static FinT<IO, A> liftIO<A>(Task<Fin<A>> ma) =>
        new(IO.liftAsync(async () => await ma.ConfigureAwait(false)));

    /// <summary>
    /// Lift a `Validation` into a `ValidationT IO` 
    /// </summary>
    public static ValidationT<L, IO, A> liftIO<L, A>(Validation<L, A> ma) 
        where L : Monoid<L> =>
        ValidationT.lift<L, IO, A>(ma);

    /// <summary>
    /// Lift a `Validation` into a `ValidationT IO` 
    /// </summary>
    public static ValidationT<L, IO, A> liftIO<L, A>(Task<Validation<L, A>> ma) 
        where L : Monoid<L> =>
        new(_ => IO.liftAsync(async () => await ma.ConfigureAwait(false)));
    
    /// <summary>
    /// Lift the IO monad into a transformer-stack with an IO as its innermost monad.
    /// </summary>
    public static K<T, A> liftIO<T, M, A>(IO<A> ma)
        where T : MonadT<T, M>
        where M : Monad<M> => 
        T.Lift(M.LiftIOMaybe(ma));
}
