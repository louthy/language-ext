#nullable enable
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt.Common;
using LanguageExt.Transducers;

namespace LanguageExt;

internal static class TaskAsync<A>
{
    internal static Error FromAggregate(AggregateException? e)
    {
        if (e is null) return Errors.None;
        var errs = e.InnerExceptions.Map(Error.New).ToSeq();
        if (errs.Count == 0) return Errors.None;
        if (errs.Count == 1) return errs.Head;
        return Error.Many(errs);
    }

    /// <summary>
    /// Runs a task concurrently and yields whilst waiting
    /// </summary>
    /// <param name="f">Function that yields a task to run</param>
    /// <param name="token">Cancellation token</param>
    /// <returns>Result of the operation</returns>
    public static TResult<A> Run(Func<CancellationToken, Task<A>> f, CancellationToken token)
    {
        if (token.IsCancellationRequested) return TResult.Cancel<A>();

        // Launch the task
        var t = f(token);

        // Spin waiting for the task to complete or be cancelled
        SpinWait sw = default;
        while (!t.IsCompleted && !token.IsCancellationRequested)
        {
            sw.SpinOnce();
        }

        if (t.IsCanceled || token.IsCancellationRequested)
        {
            return TResult.Cancel<A>();
        }

        if (t.IsFaulted)
        {
            return t.Exception is null
                ? TResult.None<A>()
                : TResult.Fail<A>(FromAggregate(t.Exception));
        }

        return TResult.Continue(t.Result);
    }
    
    /// <summary>
    /// Runs a task concurrently and yields whilst waiting
    /// </summary>
    /// <param name="f">Function that yields a task to run</param>
    /// <param name="token">Cancellation token</param>
    /// <returns>Result of the operation</returns>
    public static TResult<A> Run(Func<CancellationToken, ValueTask<A>> f, CancellationToken token)
    {
        if (token.IsCancellationRequested) return TResult.Cancel<A>();

        // Launch the task
        var vt = f(token);

        // Spin waiting for the task to complete or be cancelled
        SpinWait sw = default;
        while (!vt.IsCompleted && !token.IsCancellationRequested)
        {
            sw.SpinOnce();
        }

        if (vt.IsCanceled || token.IsCancellationRequested)
        {
            return TResult.Cancel<A>();
        }

        if (vt.IsFaulted)
        {
            var t = vt.AsTask();
            return t.Exception is null
                ? TResult.None<A>()
                : TResult.Fail<A>(FromAggregate(t.Exception));
        }

        return TResult.Continue(vt.Result);
    }
    
    /// <summary>
    /// Runs a task concurrently and yields whilst waiting
    /// </summary>
    /// <param name="f">Function that yields a task to run</param>
    /// <param name="token">Cancellation token</param>
    /// <returns>Result of the operation</returns>
    public static Fin<A> Run(Func<A> f, CancellationToken token)
    {
        if (token.IsCancellationRequested) return Errors.Cancelled;

        // Launch the task
        var t = Task.Run(f, token);

        // Spin waiting for the task to complete or be cancelled
        SpinWait sw = default;
        while (!t.IsCompleted && !token.IsCancellationRequested)
        {
            sw.SpinOnce();
        }

        if (t.IsCanceled || token.IsCancellationRequested)
        {
            return Errors.Cancelled;
        }

        if (t.IsFaulted)
        {
            return FromAggregate(t.Exception);
        }

        return t.Result;
    }
    
    /// <summary>
    /// Runs a task concurrently and yields whilst waiting
    /// </summary>
    /// <param name="f">Function that yields a task to run</param>
    /// <param name="token">Cancellation token</param>
    /// <returns>Result of the operation</returns>
    public static TResult<A> Run(Func<CancellationToken, Task<TResult<A>>> f, CancellationToken token)
    {
        if (token.IsCancellationRequested) return TResult.Cancel<A>();

        // Launch the task
        var t = f(token);

        // Spin waiting for the task to complete or be cancelled
        SpinWait sw = default;
        while (!t.IsCompleted && !token.IsCancellationRequested)
        {
            sw.SpinOnce();
        }

        if (t.IsCanceled || token.IsCancellationRequested)
        {
            return TResult.Cancel<A>();
        }

        if (t.IsFaulted)
        {
            return t.Exception is null
                ? TResult.None<A>()
                : TResult.Fail<A>(FromAggregate(t.Exception));
        }

        return t.Result;
    }
    
    /// <summary>
    /// Runs a task concurrently and yields whilst waiting
    /// </summary>
    /// <param name="f">Function that yields a task to run</param>
    /// <param name="token">Cancellation token</param>
    /// <returns>Result of the operation</returns>
    public static TResult<A> Run(Func<CancellationToken, ValueTask<TResult<A>>> f, CancellationToken token)
    {
        if (token.IsCancellationRequested) return TResult.Cancel<A>();

        // Launch the task
        var vt = f(token);

        // Spin waiting for the task to complete or be cancelled
        SpinWait sw = default;
        while (!vt.IsCompleted && !token.IsCancellationRequested)
        {
            sw.SpinOnce();
        }

        if (vt.IsCanceled || token.IsCancellationRequested)
        {
            return TResult.Cancel<A>();
        }

        if (vt.IsFaulted)
        {
            var t = vt.AsTask();
            return t.Exception is null
                    ? TResult.None<A>()
                    : TResult.Fail<A>(FromAggregate(t.Exception));
        }

        return vt.Result;
    }
    
    /// <summary>
    /// Runs a task concurrently and yields whilst waiting
    /// </summary>
    /// <param name="f">Function that yields a task to run</param>
    /// <param name="value">Value to pass to the function to run</param>
    /// <param name="token">Cancellation token</param>
    /// <returns>Result of the operation</returns>
    public static TResult<B> Run<B>(Func<CancellationToken, A, Task<TResult<B>>> f, A value, CancellationToken token)
    {
        if (token.IsCancellationRequested) return TResult.Cancel<B>();

        // Launch the task
        var t = f(token, value);

        // Spin waiting for the task to complete or be cancelled
        SpinWait sw = default;
        while (!t.IsCompleted && !token.IsCancellationRequested)
        {
            sw.SpinOnce();
        }

        if (t.IsCanceled || token.IsCancellationRequested)
        {
            return TResult.Cancel<B>();
        }

        if (t.IsFaulted)
        {
            return t.Exception is null
                    ? TResult.None<B>()
                    : TResult.Fail<B>(FromAggregate(t.Exception));
        }

        return t.Result;
    }
    
    /// <summary>
    /// Runs a task concurrently and yields whilst waiting
    /// </summary>
    /// <param name="f">Function that yields a task to run</param>
    /// <param name="value">Value to pass to the function to run</param>
    /// <param name="token">Cancellation token</param>
    /// <returns>Result of the operation</returns>
    public static TResult<B> Run<B>(Func<CancellationToken, A, ValueTask<TResult<B>>> f, A value, CancellationToken token)
    {
        if (token.IsCancellationRequested) return TResult.Cancel<B>();

        // Launch the task
        var vt = f(token, value);

        // Spin waiting for the task to complete or be cancelled
        SpinWait sw = default;
        while (!vt.IsCompleted && !token.IsCancellationRequested)
        {
            sw.SpinOnce();
        }

        if (vt.IsCanceled || token.IsCancellationRequested)
        {
            return TResult.Cancel<B>();
        }

        if (vt.IsFaulted)
        {
            var t = vt.AsTask();
            return t.Exception is null
                ? TResult.None<B>()
                : TResult.Fail<B>(FromAggregate(t.Exception));
        }

        return vt.Result;
    }
    
        
    /// <summary>
    /// Runs a task concurrently and yields whilst waiting
    /// </summary>
    /// <param name="f">Function that yields a task to run</param>
    /// <param name="value">Value to pass to the function to run</param>
    /// <param name="token">Cancellation token</param>
    /// <returns>Result of the operation</returns>
    public static TResult<B> Run<B>(Func<CancellationToken, A, Task<B>> f, A value, CancellationToken token)
    {
        if (token.IsCancellationRequested) return TResult.Cancel<B>();

        // Launch the task
        var t = f(token, value);

        // Spin waiting for the task to complete or be cancelled
        SpinWait sw = default;
        while (!t.IsCompleted && !token.IsCancellationRequested)
        {
            sw.SpinOnce();
        }

        if (t.IsCanceled || token.IsCancellationRequested)
        {
            return TResult.Cancel<B>();
        }

        if (t.IsFaulted)
        {
            return t.Exception is null
                    ? TResult.None<B>()
                    : TResult.Fail<B>(FromAggregate(t.Exception));
        }

        return TResult.Continue(t.Result);
    }
    
    /// <summary>
    /// Runs a task concurrently and yields whilst waiting
    /// </summary>
    /// <param name="f">Function that yields a task to run</param>
    /// <param name="value">Value to pass to the function to run</param>
    /// <param name="token">Cancellation token</param>
    /// <returns>Result of the operation</returns>
    public static TResult<B> Run<B>(Func<CancellationToken, A, ValueTask<B>> f, A value, CancellationToken token)
    {
        if (token.IsCancellationRequested) return TResult.Cancel<B>();

        // Launch the task
        var vt = f(token, value);

        // Spin waiting for the task to complete or be cancelled
        SpinWait sw = default;
        while (!vt.IsCompleted && !token.IsCancellationRequested)
        {
            sw.SpinOnce();
        }

        if (vt.IsCanceled || token.IsCancellationRequested)
        {
            return TResult.Cancel<B>();
        }

        if (vt.IsFaulted)
        {
            var t = vt.AsTask();
            return t.Exception is null
                ? TResult.None<B>()
                : TResult.Fail<B>(FromAggregate(t.Exception));
        }

        return TResult.Continue(vt.Result);
    }
    
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //   Async
    // 
    
    /// <summary>
    /// Runs a task concurrently and yields whilst waiting
    /// </summary>
    /// <param name="f">Function that yields a task to run</param>
    /// <param name="token">Cancellation token</param>
    /// <param name="@finally">What to run when the function is complete</param>
    /// <returns>Result of the operation</returns>
    public static Task<TResult<A>> RunAsync(
        Func<CancellationToken, A> f,
        Action? @finally,
        CancellationToken token) =>
        Task.Run(() =>
        {
            try
            {
                return TResult.Continue(f(token));
            }
            catch (Exception e)
            {
                return e is OperationCanceledException
                    ? TResult.Cancel<A>()
                    : TResult.Fail<A>(e);
            }
            finally
            {
                @finally?.Invoke();
            }
        }, token);
    
    /// <summary>
    /// Runs a task concurrently and yields whilst waiting
    /// </summary>
    /// <param name="f">Function that yields a task to run</param>
    /// <param name="token">Cancellation token</param>
    /// <param name="@finally">What to run when the function is complete</param>
    /// <returns>Result of the operation</returns>
    public static Task<Fin<A>> RunAsync(
        Func<A> f, 
        Action? @finally,
        CancellationToken token) =>
        Task.Run(() =>
        {
            try
            {
                return Fin<A>.Succ(f());
            }
            catch (Exception e)
            {
                return e is OperationCanceledException
                    ? Fin<A>.Fail(Errors.Cancelled)
                    : Fin<A>.Fail(e);
            }
            finally
            {
                @finally?.Invoke();
            }
        }, token);
    
    /// <summary>
    /// Runs a task concurrently and yields whilst waiting
    /// </summary>
    /// <param name="f">Function that yields a task to run</param>
    /// <param name="token">Cancellation token</param>
    /// <param name="@finally">What to run when the function is complete</param>
    /// <returns>Result of the operation</returns>
    public static Task<TResult<A>> RunAsync(
        Func<CancellationToken, TResult<A>> f, 
        Action? @finally,
        CancellationToken token) =>
        Task.Run(() =>
        {
            try
            {
                return f(token);
            }
            catch (Exception e)
            {
                return e is OperationCanceledException
                    ? TResult.Cancel<A>()
                    : TResult.Fail<A>(e);
            }
            finally
            {
                @finally?.Invoke();
            }
        }, token);
    
    /// <summary>
    /// Runs a task concurrently and yields whilst waiting
    /// </summary>
    /// <param name="f">Function that yields a task to run</param>
    /// <param name="value">Value to pass to the function to run</param>
    /// <param name="token">Cancellation token</param>
    /// <param name="@finally">What to run when the function is complete</param>
    /// <returns>Result of the operation</returns>
    public static Task<TResult<B>> RunAsync<B>(
        Func<CancellationToken, A, TResult<B>> f, 
        A value, 
        Action? @finally,
        CancellationToken token) =>
        Task.Run(() =>
        {
            try
            {
                return f(token, value);
            }
            catch (Exception e)
            {
                return e is OperationCanceledException
                    ? TResult.Cancel<B>()
                    : TResult.Fail<B>(e);
            }
            finally
            {
                @finally?.Invoke();
            }
        }, token);
        
    /// <summary>
    /// Runs a task concurrently and yields whilst waiting
    /// </summary>
    /// <param name="f">Function that yields a task to run</param>
    /// <param name="value">Value to pass to the function to run</param>
    /// <param name="token">Cancellation token</param>
    /// <param name="@finally">What to run when the function is complete</param>
    /// <returns>Result of the operation</returns>
    public static Task<TResult<B>> RunAsync<B>(
        Func<CancellationToken, A, B> f, 
        A value, 
        Action? @finally,
        CancellationToken token) =>
        Task.Run(() =>
        {
            try
            {
                return TResult.Continue(f(token, value));
            }
            catch (Exception e)
            {
                return e is OperationCanceledException
                    ? TResult.Cancel<B>()
                    : TResult.Fail<B>(e);
            }
            finally
            {
                @finally?.Invoke();
            }
        }, token);
    
}
