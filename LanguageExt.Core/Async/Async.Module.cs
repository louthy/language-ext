using System;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt.Common;

namespace LanguageExt;

/// <summary>
/// The `Async` module helps transition away from the `Task` / `async` / `await` world and into one
/// where awaiting is the default setting for concurrent programming and branching/forking is the
/// thing we do the least.
///
/// The `Async.await` function will convert a `Task<A>` into an `A` by waiting for the `Task` to
/// complete; it will yield the thread whilst it's waiting (to play nice with other tasks in the
/// task-pool).  This is just like the regular `await` keyword without all the ceremony and
/// colouring of methods.
///
/// `Async.fork` lifts a function into an IO monad, forks it, and then runs the IO monad returning
/// a `ForkIO` object.  The forked operation continues to run in parallel.  The `ForkIO` object
/// contains two properties: `Await` and `Cancel` that be used to either await the result or
/// cancel the operation.
///
/// These two functions remove the need for methods that are 'tainted' with `Task` or `async` /
/// `await` mechanics and assume that the thing we will do the most with asynchronous code is to
/// await it.
///
/// This module shouldn't be needed too much, as the IO monad is where most of the asynchrony
/// should be. But, when converting from existing async/await code, or if you're coming from
/// language-ext v4, or earlier, where there was lots of `*Async` methods in the key types, then
/// this module will help ease the transition.
/// </summary>
public static class Async
{
    /// <summary>
    /// Simple awaiter that yields the thread whilst waiting.  Allows for the `Task` to
    /// be used with synchronous code without blocking any threads for concurrent
    /// processing.
    /// </summary>
    /// <param name="operation">Task to await</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Result of the task, `TaskCanceledException`, or any exception raised by the task</returns>
    /// <exception cref="TaskCanceledException"></exception>
    public static A await<A>(Task<A> operation)
    {
        SpinWait sw = default;
        while (true)
        {
            if (operation.IsCanceled)
            {
                throw new TaskCanceledException();
            }
            else if (operation.IsFaulted)
            {
                operation.Exception.Rethrow();
            }
            else if (operation.IsCompleted)
            {
                return operation.Result;
            }
            sw.SpinOnce();
        }
    }

    /// <summary>
    /// `Async.fork` lifts a function into an IO monad, forks it, and then runs the IO monad returning
    /// the `ForkIO` object.  The forked operation continues to run in parallel.  The `ForkIO` object
    /// contains two properties: `Await` and `Cancel` that be used to either await the result or
    /// cancel the operation.
    /// </summary>
    /// <param name="operation">Operation to fork</param>
    /// <param name="timeout">Optional timeout</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>The `ForkIO` object contains two properties: `Await` and `Cancel` that be used to either
    /// await the result or cancel the operation.</returns>
    public static ForkIO<A> fork<A>(Func<A> operation, TimeSpan timeout = default) =>
        IO.lift(operation).ForkIO(timeout).Run();

    /// <summary>
    /// `Async.fork` lifts a function into an IO monad, forks it, and then runs the IO monad returning
    /// the `ForkIO` object.  The forked operation continues to run in parallel.  The `ForkIO` object
    /// contains two properties: `Await` and `Cancel` that be used to either await the result or
    /// cancel the operation.
    /// </summary>
    /// <param name="operation">Operation to fork</param>
    /// <param name="timeout">Optional timeout</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>The `ForkIO` object contains two properties: `Await` and `Cancel` that be used to either
    /// await the result or cancel the operation.</returns>
    public static ForkIO<A> fork<A>(Func<Task<A>> operation, TimeSpan timeout = default) =>
        IO.liftAsync(operation).ForkIO(timeout).Run();
}
