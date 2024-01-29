using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Threading;
using LanguageExt.Common;

namespace LanguageExt;

/// <summary>
/// Observable extensions
/// </summary>
public static class ObservableExt
{
    /// <summary>
    /// Executes an action post-subscription.  This is useful when the action is 
    /// going to publish to the observable.  A kind of request/response.
    /// </summary>
    [Pure]
    public static IObservable<T> PostSubscribe<T>(
        this IObservable<T> self,
        Action action) =>
        new ActionObservable<T>(action, self);

    /// <summary>
    /// Executes an action post-subscription.  This is useful when the action is 
    /// going to publish to the observable.  A kind of request/response.
    /// </summary>
    [Pure]
    public static IObservable<T> PostSubscribe<T>(
        this IObservable<T> self,
        Func<Unit> action) =>
        new ActionObservable<T>(() => action(), self);

    /// <summary>
    /// Convert an `IObservable` to an `IAsyncEnumerable`
    /// </summary>
    public static IAsyncEnumerable<A> ToAsyncEnumerable<A>(
        this IObservable<A> observable,
        CancellationToken token) =>
        Observe<A>.Run(observable, token);

    class Observe<A> : IObserver<A>
    {
        readonly AutoResetEvent wait;
        readonly ConcurrentQueue<Fin<A>> queue;

        Observe(AutoResetEvent wait, ConcurrentQueue<Fin<A>> queue)
        {
            this.wait = wait;
            this.queue = queue;
        }

        public static async IAsyncEnumerable<A> Run(
            IObservable<A> observable, 
            [EnumeratorCancellation] CancellationToken token)
        {
            using var wait  = new AutoResetEvent(false);
            var       queue = new ConcurrentQueue<Fin<A>>();
            observable.Subscribe(new Observe<A>(wait, queue));

            while (true)
            {
                await wait.WaitOneAsync(token).ConfigureAwait(false);
                while (queue.TryDequeue(out var item))
                {
                    if (item.IsFail)
                    {
                        if (item.Error == Errors.None) yield break;
                        if (item.Error == Errors.Cancelled) throw new OperationCanceledException();
                        item.Error.Throw();
                        yield break;
                    }
                    else
                    {
                        yield return item.Value;
                    }
                }
            }                
        }

        public void OnCompleted()
        {
            queue.Enqueue(Errors.None);
            wait.Set();
        }

        public void OnError(Exception error)
        {
            queue.Enqueue(Error.New(error));
            wait.Set();
        }

        public void OnNext(A value)
        {
            queue.Enqueue(value);
            wait.Set();
        }
    }
}
