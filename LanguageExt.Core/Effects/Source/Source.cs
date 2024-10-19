using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt;

/// <summary>
/// A source of a stream
/// </summary>
/// <remarks>
/// A `Source` manages a queue of events that it distributes to its subscribers asynchronously.
///
/// Each subscriber runs on the same thread as the event-distributor.  So, if you have multiple
/// subscribers they will be processed serially for each event.  If you want the subscribers to
/// run in parallel then you must lift the `IO` monad when calling `Subscribe` and `forkIO` the
/// stream.  This gives fine-grained control over when to run events in parallel.
/// </remarks>
/// <typeparam name="A">Value type flowing downstream</typeparam>
public class Source<A> : IDisposable
{
    readonly CancellationTokenRegistration cancelReg;
    readonly AutoResetEvent wait;
    readonly ConcurrentQueue<Event> queue;
    readonly ConcurrentDictionary<long, Sub<A>> subscriptions;
    readonly Task stream;
    readonly EnvIO envIO;
    long identifier;
    long completed;

    Source(EnvIO envIO)
    {
        this.envIO = envIO;
        wait = new AutoResetEvent (false);
        queue = new ConcurrentQueue<Event>();
        cancelReg = envIO.Token.Register(() => Complete());
        subscriptions = new ConcurrentDictionary<long, Sub<A>>();
        stream = Task.Factory.StartNew(Dequeue, TaskCreationOptions.LongRunning);
    }

    /// <summary>
    /// Start a new source
    /// </summary>
    /// <returns>A source in an IO computation</returns>
    public static IO<Source<A>> Start() =>
        IO.lift(envIO => new Source<A>(envIO));

    /// <summary>
    /// Subscribe to the source and await the values
    /// </summary>
    /// <remarks>
    /// Each subscriber runs on the same thread as the event-distributor.  So, if you have multiple
    /// subscribers they will be processed serially for each event.  If you want the subscribers to
    /// run in parallel then you must lift the `IO` monad when calling `Subscribe` and `forkIO` the
    /// stream.  This gives fine-grained control over when to run events in parallel.
    /// </remarks>
    /// <typeparam name="M">Monad type lifted into the stream</typeparam>
    /// <returns>StreamT monad transformer that will get the values coming downstream</returns>
    public StreamT<M, A> Await<M>()
        where M : Monad<M>
    {
        var id  = Interlocked.Increment(ref identifier);
        var sub = new Sub<M, A>(() => subscriptions.TryRemove(id, out _));
        subscriptions.TryAdd(id, sub);
        return sub.Stream;
    }

    void Dequeue()
    {
        while (Interlocked.Read(ref completed) != 2)
        {
            while (queue.TryDequeue(out var e))
            {
                switch (e)
                {
                    case ValueEvent ve:
                        foreach (var sub in subscriptions) sub.Value.Post(ve.Value);
                        break;
                    
                    case CompleteEvent:
                        foreach (var sub in subscriptions) sub.Value.Complete();
                        break;
                }
            }
            wait.WaitOne();
        }
    }

    /// <summary>
    /// Post a value to flow downstream
    /// </summary>
    /// <param name="value">Value</param>
    /// <returns>IO effect</returns>
    public IO<Unit> Post(A value) =>
        IO.lift(() =>
                {
                    if (Interlocked.Read(ref completed) == 0)
                    {
                        queue.Enqueue(new ValueEvent(value));
                        wait.Set();
                    }
                    else
                    {
                        throw Errors.SourceCompleted.ToException();
                    }
                });

    /// <summary>
    /// Post a completion event to flow downstream to shut down the `Source` and its subscribers
    /// </summary>
    /// <returns>IO effect</returns>
    public IO<Unit> Complete() =>
        IO.lift(() =>
                {
                    if (Interlocked.CompareExchange(ref completed, 1, 0) == 0)
                    {
                        // Queue the completion event
                        queue.Enqueue(CompleteEvent.Default);
                        wait.Set();

                        // Wait for the subscribers to empty
                        SpinWait sw = default;
                        while (!subscriptions.IsEmpty)
                        {
                            sw.SpinOnce();
                        }
                        completed = 2;

                        try { cancelReg.Dispose(); } catch { /* ignore */ }
                        try { envIO.Dispose(); } catch { /* ignore */ }
                        try { wait.Dispose(); } catch { /* ignore */ }
                        try { stream.Dispose(); } catch { /* ignore */ }
                    }
                    else
                    {
                        throw Errors.SourceCompleted.ToException();
                    }
                });

    /// <summary>
    /// Dispose
    /// </summary>
    public void Dispose() =>
        Complete().Run(envIO);

    record Event;
    record ValueEvent(A Value) : Event;
    record CompleteEvent : Event
    {
        public static Event Default = new CompleteEvent();
    }
}
