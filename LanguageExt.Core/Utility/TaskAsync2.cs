using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt.Common;

namespace LanguageExt;

internal static class TaskAsync<A, B>
{
    /// <summary>
    /// Runs a task concurrently and yields whilst waiting
    /// </summary>
    /// <param name="f">Function that yields a task to run</param>
    /// <param name="token">Cancellation token</param>
    /// <returns>Result of the operation</returns>
    public static TResult<(A First, B Second)> Run(
        Func<CancellationToken, Task<A>> first, 
        Func<CancellationToken, Task<B>> second, 
        CancellationToken token)
    {
        if (token.IsCancellationRequested) return TResult.Cancel<(A, B)>();

        // Launch the task
        var t1 = first(token);
        var t2 = second(token);

        // Spin waiting for the task to complete or be cancelled
        SpinWait sw = default;
        while ((!t1.IsCompleted || !t2.IsCompleted) && !token.IsCancellationRequested)
        {
            sw.SpinOnce();
        }

        if (t1.IsCanceled || t2.IsCanceled || token.IsCancellationRequested)
        {
            return TResult.Cancel<(A, B)>();
        }

        if (t1.IsFaulted)
        {
            return t1.Exception is null
                ? TResult.None<(A, B)>()
                : TResult.Fail<(A, B)>(Error.New(t1.Exception.InnerExceptions.FirstOrDefault() ?? t1.Exception));
        }

        if (t2.IsFaulted)
        {
            return t2.Exception is null
                ? TResult.None<(A, B)>()
                : TResult.Fail<(A, B)>(Error.New(t2.Exception.InnerExceptions.FirstOrDefault() ?? t2.Exception));
        }

        return TResult.Continue((t1.Result, t2.Result));
    }
    
    /// <summary>
    /// Runs a task concurrently and yields whilst waiting
    /// </summary>
    /// <param name="f">Function that yields a task to run</param>
    /// <param name="token">Cancellation token</param>
    /// <returns>Result of the operation</returns>
    public static TResult<(A First, B Second)> Run(
        Func<CancellationToken, ValueTask<A>> first, 
        Func<CancellationToken, ValueTask<B>> second, 
        CancellationToken token)
    {
        if (token.IsCancellationRequested) return TResult.Cancel<(A, B)>();

        // Launch the task
        var t1 = first(token);
        var t2 = second(token);

        // Spin waiting for the task to complete or be cancelled
        SpinWait sw = default;
        while ((!t1.IsCompleted || !t2.IsCompleted) && !token.IsCancellationRequested)
        {
            sw.SpinOnce();
        }

        if (t1.IsCanceled || t2.IsCanceled || token.IsCancellationRequested)
        {
            return TResult.Cancel<(A, B)>();
        }

        if (t1.IsFaulted)
        {
            var t = t1.AsTask();
            return t.Exception is null
                ? TResult.None<(A, B)>()
                : TResult.Fail<(A, B)>(Error.New(t.Exception.InnerExceptions.FirstOrDefault() ?? t.Exception));
        }

        if (t2.IsFaulted)
        {
            var t = t2.AsTask();
            return t.Exception is null
                ? TResult.None<(A, B)>()
                : TResult.Fail<(A, B)>(Error.New(t.Exception.InnerExceptions.FirstOrDefault() ?? t.Exception));
        }

        return TResult.Continue((t1.Result, t2.Result));
    }    
    
    
    /// <summary>
    /// Runs a task concurrently and yields whilst waiting
    /// </summary>
    /// <param name="f">Function that yields a task to run</param>
    /// <param name="token">Cancellation token</param>
    /// <returns>Result of the operation</returns>
    public static Fin<(A First, B Second)> Run(
        Func<A> first,
        Func<B> second,
        CancellationToken token)
    {
        if (token.IsCancellationRequested) return Errors.Cancelled;

        // Launch the task
        var t1 = Task.Run(first, token);
        var t2 = Task.Run(second, token);

        // Spin waiting for the task to complete or be cancelled
        SpinWait sw = default;
        while ((!t1.IsCompleted || !t2.IsCompleted) && !token.IsCancellationRequested)
        {
            sw.SpinOnce();
        }

        if (t1.IsCanceled || t2.IsCanceled || token.IsCancellationRequested)
        {
            return Fin<(A, B)>.Fail(Errors.Cancelled);
        }

        if (t1.IsFaulted)
        {
            return t1.Exception is null
                ? Fin<(A, B)>.Fail(Errors.None)
                : Fin<(A, B)>.Fail(Error.New(t1.Exception.InnerExceptions.FirstOrDefault() ?? t1.Exception));
        }

        if (t2.IsFaulted)
        {
            return t2.Exception is null
                ? Fin<(A, B)>.Fail(Errors.None)
                : Fin<(A, B)>.Fail(Error.New(t2.Exception.InnerExceptions.FirstOrDefault() ?? t2.Exception));
        }

        return Fin<(A, B)>.Succ((t1.Result, t2.Result));
    }
}
