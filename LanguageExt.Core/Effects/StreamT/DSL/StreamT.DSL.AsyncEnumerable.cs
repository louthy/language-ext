using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt.Traits;

namespace LanguageExt;

/// <summary>
/// Lazy sequence monad transformer
/// </summary>
internal record StreamAsyncEnumerableT<M, A>(IAsyncEnumerable<A> items) : StreamT<M, A>
    where M : Monad<M>
{
    public override K<M, MList<A>> runListT =>
        M.LiftIO(IO.env)
         .Bind(e => StreamEnumerableT<M, A>.Lift(items.ToBlockingEnumerable(e.Token).GetEnumerator())
                                           .runListT);

    public override StreamT<M, A> Tail() =>
        new StreamAsyncEnumerableT<M, A>(items.Skip(1));

    public override StreamT<M, B> Map<B>(Func<A, B> f) =>
        new StreamAsyncEnumerableT<M, B>(items.Map(f));

    /// <summary>
    /// Interleave the items of two streams
    /// </summary>
    /// <param name="rhs">Other stream to merge with</param>
    /// <returns>Stream transformer</returns>
    public override StreamT<M, A> Merge(K<StreamT<M>, A> rhs) =>
        rhs switch
        {
            StreamAsyncEnumerableT<M, A> r => new StreamAsyncEnumerableT<M, A>(MergeAsync(items, r.items)),
            StreamEnumerableT<M, A> r      => new StreamAsyncEnumerableT<M, A>(MergeSync(items, r.items)),
            _                              => base.Merge(rhs)
        };

    static async IAsyncEnumerable<A> MergeSync(IAsyncEnumerable<A> lhs, IEnumerable<A> rhs)
    {
        using var iter = rhs.GetEnumerator();
        await foreach(var a in lhs)
        {
            if (iter.MoveNext())
            {
                yield return a;
                yield return iter.Current;
            }
            else
            {
                yield return a;
            }
        }
        while (iter.MoveNext())
        {
            yield return iter.Current;
        }
    }

    static async IAsyncEnumerable<A> MergeAsync(IAsyncEnumerable<A> lhs, IAsyncEnumerable<A> rhs)
    {
        var complete = new CountdownEvent(2);
        var wait = new AutoResetEvent(true);
        ConcurrentQueue<A> queue = [];
        var lt = EnumerateAsync(lhs, queue, wait);
        var rt = EnumerateAsync(rhs, queue, wait);
        var wt = Task.WhenAll(lt, rt);
        while (!wt.IsCompleted)
        {
            wait.WaitOne();
            while (queue.TryDequeue(out var value))
            {
                yield return value;
            }
        }
    }

    static Task EnumerateAsync(
        IAsyncEnumerable<A> ma, 
        ConcurrentQueue<A> queue, 
        AutoResetEvent wait) =>
        Task.Run(async () =>
                 {
                     await foreach (var a in ma)
                     {
                         queue.Enqueue(a);
                         wait.Set();
                     }
                 });
}
