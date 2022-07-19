using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace LanguageExt;

/// <summary>
/// An asynchronous queue that guarantees that only one item in the queue is being processed at any one time.
/// It also guarantees that every item in the queue is processed before a `Stop` action.
/// </summary>
class Channel<A> : IAsyncEnumerable<A>
{
    readonly ConcurrentQueue<Ev<A>> queue;
    readonly AutoResetEvent wait;
    
    /// <summary>
    /// Construct
    /// </summary>
    public Channel()
    {
        queue = new ConcurrentQueue<Ev<A>>();
        wait = new AutoResetEvent(false);
    }

    /// <summary>
    /// Clean up
    /// </summary>
    ~Channel() =>
        wait.Dispose();

    /// <summary>
    /// Number of items in the channel
    /// </summary>
    public int Count =>
        queue.Count;

    /// <summary>
    /// Post an event
    /// </summary>
    /// <param name="event">Event</param>
    internal Unit Post(Ev<A> @event)
    {
        queue.Enqueue(@event);
        wait.Set();
        return default;
    }

    /// <summary>
    /// Post an item
    /// </summary>
    /// <param name="value">Value</param>
    public Unit Post(A value) =>
        Post(Ev.Item(value));

    /// <summary>
    /// Stop event
    /// </summary>
    public Unit Stop() =>
        Post(Ev<A>.Stop);

    public async IAsyncEnumerator<A> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        while (true)
        {
            await wait.WaitOneAsync(cancellationToken).ConfigureAwait(false);
            if (cancellationToken.IsCancellationRequested) yield break;

            while (queue.TryDequeue(out var item))
            {
                if (cancellationToken.IsCancellationRequested) yield break;

                switch (item)
                {
                    case ItemEv<A> e:
                        yield return e.Value;
                        break;

                    case StopEv<A>:
                        yield break;
                }
            }
        }
    }
}

/// <summary>
/// Event 
/// </summary>
static class Ev
{
    public static Ev<A> Item<A>(A value) => new ItemEv<A>(value);
}

/// <summary>
/// Event 
/// </summary>
abstract record Ev<A>
{
    public static readonly Ev<A> Stop = StopEv<A>.Default;
}

/// <summary>
/// New value event
/// </summary>
sealed record ItemEv<A>(A Value) : Ev<A>;

/// <summary>
/// Stop event
/// </summary>
sealed record StopEv<A> : Ev<A>
{
    public static readonly Ev<A> Default = new StopEv<A>();
}
