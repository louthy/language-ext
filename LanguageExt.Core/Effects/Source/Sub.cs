/*
#pragma warning disable LX_StreamT

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using LanguageExt.Traits;

namespace LanguageExt;

/// <summary>
/// Stream subscription
/// </summary>
/// <typeparam name="M">Monad type lifted in the stream</typeparam>
/// <typeparam name="A">Stream value type</typeparam>
abstract class Sub<A> : IDisposable
{
    public abstract void Dispose();
    public abstract void Post(A value);
    public abstract void Complete();
}

/// <summary>
/// Stream subscription
/// </summary>
/// <typeparam name="M">Monad type lifted in the stream</typeparam>
/// <typeparam name="A">Stream value type</typeparam>
class Sub<M, A> : Sub<A>
    where M : Monad<M>
{
    readonly ConcurrentQueue<A> queue;
    readonly AutoResetEvent wait;
    readonly IEnumerator<A> enumerator; 
    readonly Action unsubscribe;
    long active;

    /// <summary>
    /// Stream of items
    /// </summary>
    public readonly StreamT<M, A> Stream;
 
    internal Sub(Action unsubscribe)
    {
        this.unsubscribe = unsubscribe;
        queue = new ConcurrentQueue<A>();
        wait = new AutoResetEvent(false);
        enumerator = ToEnumerable().GetEnumerator();
        Stream = StreamT<M, A>.Lift(enumerator);
    }

    public override void Post(A value)
    {
        queue.Enqueue(value);
        wait.Set();
    }

    public override void Complete()
    {
        if (Interlocked.CompareExchange(ref active, 1, 0) == 0)
        {
            wait.Set();

            // Wait for the queue to empty
            SpinWait sw = default;
            while (!queue.IsEmpty)
            {
                sw.SpinOnce();
            }

            unsubscribe();
            Dispose();
        }
    }

    IEnumerable<A> ToEnumerable()
    {
        while (true)
        {
            switch (Interlocked.Read(ref active))
            {
                case 0:
                    while (queue.TryDequeue(out var e))
                    {
                        yield return e;
                    }
                    break;
                    
                case 1:
                    while (queue.TryDequeue(out var e))
                    {
                        yield return e;
                    }
                    Interlocked.Exchange(ref active, 2);
                    yield break;
                    
                default:
                    yield break;
            }
            wait.WaitOne();
        }
    }

    public override void Dispose()
    {
        enumerator.Dispose();
        wait.Dispose();
    }
}
*/
