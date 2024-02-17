using System;
using System.Threading.Tasks;

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
        IO<Unit>.LiftIO(async _ =>
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
        IO<Unit>.LiftIO(async env =>
                        {
                            await action(env).ConfigureAwait(false);
                            return unit;
                        });

    /// <summary>
    /// Lift a asynchronous IO function 
    /// </summary>
    /// <param name="function">Function</param>
    /// <returns>Value that can be used with monadic types in LINQ expressions</returns>
    public static IO<A> liftIO<A>(Func<Task<A>> function) =>
        new(async _ => await function().ConfigureAwait(false));

    /// <summary>
    /// Lift a asynchronous IO function 
    /// </summary>
    /// <param name="function">Function</param>
    /// <returns>Value that can be used with monadic types in LINQ expressions</returns>
    public static IO<A> liftIO<A>(Func<EnvIO, Task<A>> function) =>
        new(async e => await function(e).ConfigureAwait(false));
}
