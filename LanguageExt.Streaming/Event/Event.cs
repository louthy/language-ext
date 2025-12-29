using System;
using System.Threading;
using System.Threading.Channels;
using LanguageExt.Traits;
using LanguageExt.UnsafeValueAccess;
using L = LanguageExt;

namespace LanguageExt;

/// <summary>
/// Adds itself to the `Action` event-delegate and then forwards anything posted by the event
/// to any subscribers to this type.  We're trying to make events a bit more 'first class,
/// rather than the 'runt of the litter' that they are now.
///
/// So, as long as you can find a way to make your event into a single argument action, you
/// can then use it with the streaming functionality within this library.
/// </summary>
/// <typeparam name="A">Value type</typeparam>
public class Event<A> : IDisposable
{
    readonly AtomHashMap<Channel<A>, Subscription> subscribers = AtomHashMap<Channel<A>, Subscription>.Empty;
    Action<A> @delegate;
    int disposed;

    /// <summary>
    /// Construct an event from subscribing to the delegate
    /// </summary>
    /// <param name="delegate">The delegate that this type will subscribe to</param>
    public Event(ref Action<A> @delegate)
    {
        @delegate += Post;
        this.@delegate = @delegate;
    }

    /// <summary>
    /// Subscribe to this event
    /// </summary>
    public IO<Source<A>> Subscribe() =>
        SubscribeInternal()
           .Map(s => Source.lift(s.Channel));
    
    /// <summary>
    /// Subscribe to this event
    /// </summary>
    public SourceT<M, A> Subscribe<M>()
        where M : MonadIO<M>, Alternative<M> =>
        from sub in SubscribeInternal()
        from val in SourceT.lift<M, A>(sub.Channel)
        select val;

    /// <summary>
    /// Subscribe a channel to this event
    /// </summary>
    /// <param name="channel"></param>
    /// <returns></returns>
    IO<Subscription> SubscribeInternal() =>
        IO.lift(e =>
                {
                    var channel = Channel.CreateUnbounded<A>();
                    var sub     = new Subscription(this, channel);
                    subscribers.AddOrUpdate(channel, sub);
                    e.Resources.Acquire(sub);
                    return sub;
                });

    public IO<Unit> Unsubscribe(Subscription sub) =>
        IO.lift(e => subscribers.Swap(map =>
                                      {
                                          if (map.ContainsKey(sub.Channel))
                                          {
                                              sub.Channel.Writer.TryComplete();
                                              map = map.Remove(sub.Channel);
                                          }
                                          return map;
                                      }));

    public IO<Unit> UnsubscribeAll() =>
        IO.lift(e =>
                {
                    foreach (var sub in subscribers.Values)
                    {
                        sub.Channel.Writer.TryComplete();
                    }
                    return subscribers.Clear();
                });

    void Post(A value)
    {
        foreach (var sub in subscribers.Values)
        {
            sub.Channel.Writer.TryWrite(value);
        }
    }
    
    public void Dispose()
    {
        if (Interlocked.CompareExchange(ref disposed, 1, 0) == 0)
        {
            #pragma warning disable CS8601 // Possible null reference assignment.
            @delegate -= Post;
            UnsubscribeAll().Run();
        }
    }
    
    public class Subscription : IDisposable
    {
        internal Channel<A> Channel;
        readonly Event<A> @event;
        int disposed;

        internal Subscription(Event<A> @event, Channel<A> channel)
        {
            this.@event = @event;
            Channel = channel;
        }

        public void Dispose()
        {
            if (Interlocked.CompareExchange(ref disposed, 1, 0) == 0)
            {
                @event.Unsubscribe(this).Run();
            }
        }
    }
}
