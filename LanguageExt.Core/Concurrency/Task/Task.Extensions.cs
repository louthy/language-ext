using LanguageExt.ClassInstances;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt;

public static class TaskExtensions
{
    public static bool CompletedSuccessfully<A>(this Task<A> ma) =>
        ma is { IsCompleted: true, IsFaulted: false, IsCanceled: false };

    [Pure]
    public static Task<A> AsFailedTask<A>(this Exception ex)
    {
        var tcs = new TaskCompletionSource<A>();
        tcs.SetException(ex);
        return tcs.Task;
    }

    /// <summary>
    /// Convert a value to a Task that completes immediately
    /// </summary>
    [Pure]
    public static Task<A> AsTask<A>(this A self) =>
        Task.FromResult(self);

    /// <summary>
    /// Convert a ValueTask to a Task 
    /// </summary>
    [Pure]
    public static Task<A> ToRef<A>(this ValueTask<A> self) =>
        self.AsTask();
        
    /// <summary>
    /// Flatten the nested Task type
    /// </summary>
    [Pure]
    public static async Task<A> Flatten<A>(this Task<Task<A>> self)
    {
        var t = await self.ConfigureAwait(false);
        var u = await t.ConfigureAwait(false);
        return u;
    }

    /// <summary>
    /// Flatten the nested Task type
    /// </summary>
    [Pure]
    public static async Task<A> Flatten<A>(this Task<Task<Task<A>>> self)
    {
        var t = await self.ConfigureAwait(false);
        var u = await t.ConfigureAwait(false);
        var v = await u.ConfigureAwait(false);
        return v;
    }

    /// <summary>
    /// Standard LINQ Select implementation for Task
    /// </summary>
    [Pure]
    public static async Task<U> Select<T, U>(this Task<T> self, Func<T, U> map) =>
        map(await self.ConfigureAwait(false));

    /// <summary>
    /// Standard LINQ Where implementation for Task
    /// </summary>
    [Pure]
    public static async Task<T> Where<T>(this Task<T> self, Func<T, bool> pred)
    {
        var resT = await self.ConfigureAwait(false);
        var res = pred(resT);
        if (!res)
        {
            throw new TaskCanceledException();
        }

        return resT;
    }

    /// <summary>
    /// Standard LINQ SelectMany implementation for Task
    /// </summary>
    [Pure]
    public async static Task<U> SelectMany<T, U>(this Task<T> self,
        Func<T, Task<U>> bind) =>
        await bind(await self.ConfigureAwait(false)).ConfigureAwait(false);

    /// <summary>
    /// Standard LINQ SelectMany implementation for Task
    /// </summary>
    [Pure]
    public static async Task<V> SelectMany<T, U, V>(this Task<T> self,
        Func<T, Task<U>> bind,
        Func<T, U, V> project)
    {
        var resT = await self.ConfigureAwait(false);
        var resU = await bind(resT).ConfigureAwait(false);
        return project(resT, resU);
    }

    /// <summary>
    /// Get the Count of a Task T.  Returns either 1 or 0 if cancelled or faulted.
    /// </summary>
    [Pure]
    public static async Task<int> Count<T>(this Task<T> self)
    {
        try
        {
            await self.ConfigureAwait(false);
            return 1;
        }
        catch (Exception)
        {
            return 0;
        }
    }

    /// <summary>
    /// Monadic bind operation for Task
    /// </summary>
    [Pure]
    public static Task<U> Bind<T, U>(this Task<T> self, Func<T, Task<U>> bind) =>
        self.SelectMany(bind);

    /// <summary>
    /// Returns false if the Task is cancelled or faulted, otherwise
    /// it returns the result of pred(Result)
    /// </summary>
    [Pure]
    public static async Task<bool> Exists<T>(this Task<T> self, Func<T, bool> pred) =>
        pred(await self.ConfigureAwait(false));

    /// <summary>
    /// Returns false if the Task is cancelled or faulted, otherwise
    /// it returns the result of pred(Result)
    /// </summary>
    [Pure]
    public static async Task<bool> ExistsAsync<T>(this Task<T> self, Func<T, Task<bool>> pred) =>
        await pred(await self.ConfigureAwait(false)).ConfigureAwait(false);

    /// <summary>
    /// Returns false if the Task is cancelled or faulted, otherwise
    /// it returns the result of pred(Result)
    /// </summary>
    [Pure]
    public static async Task<bool> ForAll<T>(this Task<T> self, Func<T, bool> pred) =>
        pred(await self.ConfigureAwait(false));

    /// <summary>
    /// Returns false if the Task is cancelled or faulted, otherwise
    /// it returns the result of pred(Result)
    /// </summary>
    [Pure]
    public static async Task<bool> ForAllAsync<T>(this Task<T> self, Func<T, Task<bool>> pred) =>
        await pred(await self.ConfigureAwait(false)).ConfigureAwait(false);

    /// <summary>
    /// Filters the task.  This throws a BottomException when pred(Result)
    /// returns false
    /// </summary>
    [Pure]
    public static Task<T> Filter<T>(this Task<T> self, Func<T, bool> pred) =>
        self.Where(pred);

    /// <summary>
    /// Folds the Task.  Returns folder(state,Result) if not faulted or
    /// cancelled.  Returns state otherwise.
    /// </summary>
    [Pure]
    public static async Task<S> Fold<T, S>(this Task<T> self, S state, Func<S, T, S> folder) =>
        folder(state, await self.ConfigureAwait(false));

    /// <summary>
    /// Folds the Task.  Returns folder(state,Result) if not faulted or
    /// cancelled.  Returns state otherwise.
    /// </summary>
    [Pure]
    public static async Task<S> FoldAsync<T, S>(this Task<T> self, S state, Func<S, T, Task<S>> folder) =>
        await folder(state, await self.ConfigureAwait(false)).ConfigureAwait(false);

    /// <summary>
    /// Iterates the Task.  Invokes f(Result) if not faulted or cancelled
    /// </summary>
    public static async Task<Unit> Iter<T>(this Task<T> self, Action<T> f)
    {
        f(await self.ConfigureAwait(false));
        return unit;
    }

    /// <summary>
    /// Impure iteration of the bound value in the structure
    /// </summary>
    /// <returns>
    /// Returns the original unmodified structure
    /// </returns>
    public static Task<A> Do<A>(this Task<A> ma, Action<A> f) =>
        ma.Map(x => {
            f(x);
            return x;
        });

    /// <summary>
    /// Returns map(Result) if not faulted or cancelled.
    /// </summary>
    [Pure]
    public static async Task<U> Map<T, U>(this Task<T> self, Func<T, U> map) =>
        map(await self.ConfigureAwait(false));

    /// <summary>
    /// Returns map(Result) if not faulted or cancelled.
    /// </summary>
    [Pure]
    public static async Task<U> MapAsync<T, U>(this Task<T> self, Func<T, Task<U>> map) =>
        await map(await self.ConfigureAwait(false)).ConfigureAwait(false);

    [Pure]
    public static async Task<V> Join<T, U, K, V>(this Task<T> source,
        Task<U> inner,
        Func<T, K> outerKeyMap,
        Func<U, K> innerKeyMap,
        Func<T, U, V> project)
    {
        await Task.WhenAll(source, inner).ConfigureAwait(false);
        if (!EqDefault<K>.Equals(outerKeyMap(source.Result), innerKeyMap(inner.Result)))
        {
            throw new OperationCanceledException();
        }

        return project(source.Result, inner.Result);
    }

    [Pure]
    public static async Task<V> GroupJoin<T, U, K, V>(this Task<T> source,
        Task<U> inner,
        Func<T, K> outerKeyMap,
        Func<U, K> innerKeyMap,
        Func<T, Task<U>, V> project)
    {
        T t = await source.ConfigureAwait(false);
        return project(t, inner.Where(u => EqDefault<K>.Equals(outerKeyMap(t), innerKeyMap(u))));
    }

    [Pure]
    public static async Task<A> Plus<A>(this Task<A> ma, Task<A> mb)
    {
        try
        {
            return await ma.ConfigureAwait(false);
        }
        catch
        {
            return await mb.ConfigureAwait(false);
        }
    }

    [Pure]
    public static async Task<A> PlusFirst<A>(this Task<A> ma, Task<A> mb) =>
        await (await Task.WhenAny(ma, mb).ConfigureAwait(false)).ConfigureAwait(false);

    public static async Task<A> Cast<A>(this Task source)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        await source.ConfigureAwait(false);
            
        return source.GetType() switch
               {
                   var taskTy when taskTy.IsGenericType                    && 
                                   taskTy.GenericTypeArguments.Length == 1 &&
                                   taskTy.GenericTypeArguments[0]     == typeof(A) => (A)((dynamic)source).Result,
                   _ => default!
               };            
    }

    public static async Task<Unit> ToUnit(this Task source)
    {
        await source.ConfigureAwait(false);
        return unit;
    }

    /// <summary>
    /// Tasks a lazy sequence of tasks and iterates them in a 'measured way'.  A default window size of
    /// `Sys.DefaultAsyncSequenceConcurrency` tasks is used, which means there are `Environment.ProcessorCount / 2`
    /// 'await streams' (by default).  An await stream essentially awaits one task from the sequence, and on
    /// completion goes and gets the next task from the lazy sequence and awaits that too.  This continues until the
    /// end of the lazy sequence, or forever for infinite streams.
    /// </summary>
    internal static Task<IList<B>> WindowMap<A, B>(this IEnumerable<Task<A>> ma, Func<A, B> f, CancellationToken token) =>
        WindowMap(ma, SysInfo.DefaultAsyncSequenceParallelism, f, token);

    /// <summary>
    /// Tasks a lazy sequence of tasks and maps them in a 'measured way'.  A default window size of
    /// `windowSize` tasks is used, which means there are `windowSize` 'await streams'.  An await stream 
    /// essentially awaits one task from the sequence, and on completion goes and gets the next task from 
    /// the lazy sequence and awaits that too.  This continues until the end of the lazy sequence, or forever 
    /// for infinite streams.  Therefore there are at most `windowSize` tasks running concurrently.
    /// </summary>
    internal static async Task<IList<B>> WindowMap<A, B>(
        this IEnumerable<Task<A>> ma, 
        int windowSize, 
        Func<A, B> f,
        CancellationToken token)
    {
        var sync = new object();
        using var wait = new CountdownEvent(windowSize);
        using var iter = ma.GetEnumerator();

        var index = -1;
        var results = new List<B>();
        var errors = new List<Exception>();

        for (var i = 0; i < windowSize; i++)
        {
            #pragma warning disable CS4014 // call is not awaited
            Task.Run(go, token);
            #pragma warning restore CS4014
        }

        Option<(int Index, Task<B>)> next()
        {
            lock (sync)
            {
                index++;
                try
                {
                    if (iter.MoveNext())
                    {
                        results.Add(default!);
                        return Some((index, iter.Current.Map(f)));
                    }
                    else
                    {
                        return default;
                    }
                }
                catch (Exception e)
                {
                    errors.Add(e);
                    return default;
                }
            }
        }
        
        void go()
        {
            try
            {
                while (!token.IsCancellationRequested)
                {
                    var otask = next();
                    if (otask.IsNone) return;
                    var (ix, task) = ((int, Task<B>))otask;

                    SpinWait sw = default;
                    while (!task.IsCompleted && !token.IsCancellationRequested)
                    {
                        sw.SpinOnce();
                    }
                    
                    lock (sync)
                    {
                        
                        if (token.IsCancellationRequested)
                        {
                            throw new TaskCanceledException();
                        }
                        else if (task.IsCanceled)
                        {
                            errors.Add(task.Exception is not null ? task.Exception : new TaskCanceledException());
                        }
                        else if (task.IsFaulted)
                        {
                            if (task.Exception is not null) errors.Add(task.Exception);
                        }
                        else if (task.IsCompleted)
                        {
                            results[ix] = task.Result;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                lock (sync)
                {
                    errors.Clear();
                    errors.Add(e);
                }
            }
            finally
            {
                wait.Signal();
            }
        }

        await wait.WaitHandle.WaitOneAsync(token).ConfigureAwait(false);
        
        if (errors.Count > 0)
        {
            var allErrors = errors
                .SelectMany(e => e is AggregateException ae ? ae.InnerExceptions.ToArray() : new[] { e })
                .ToArray();

            if (allErrors.Length > 1)
            {
                // Throw an aggregate of all exceptions
                throw new AggregateException(allErrors);
            }
            else if (allErrors.Length == 1)
            {
                // Throw an aggregate of all exceptions
                allErrors[0].Rethrow();
                return default!;
            }
        }

        return results;
    }
}
