using System;
using Ch = System.Threading.Channels;
namespace LanguageExt.Pipes.Concurrent;

public static class Mailbox
{
    /// <summary>
    /// Create a new unbounded mailbox 
    /// </summary>
    /// <typeparam name="A">Value type</typeparam>
    /// <returns>Constructed mailbox with an `Inbox` and an `Outbox`</returns>
    public static Mailbox<A, A> spawn<A>() =>
        spawn(Buffer<A>.Unbounded);

    /// <summary>
    /// Create a new mailbox with the buffer settings provided 
    /// </summary>
    /// <param name="buffer">Buffer settings</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <returns>Constructed mailbox with an `Inbox` and an `Outbox`</returns>
    /// <exception cref="NotSupportedException">Thrown for invalid buffer settings</exception>
    public static Mailbox<A, A> spawn<A>(Buffer<A> buffer)
    {
        Ch.Channel<A> channel;
        switch (buffer)
        {
            case UnboundedBuffer<A>:
                channel = Ch.Channel.CreateUnbounded<A>();
                break;

            case BoundedBuffer<A>(var size):
            {
                var opts = new Ch.BoundedChannelOptions((int)size) { FullMode = Ch.BoundedChannelFullMode.Wait };
                channel = Ch.Channel.CreateBounded<A>(opts);
                break;
            }

            case SingleBuffer<A>:
            {
                var opts = new Ch.BoundedChannelOptions(1) { FullMode = Ch.BoundedChannelFullMode.Wait };
                channel = Ch.Channel.CreateBounded<A>(opts);
                break;
            }

            case LatestBuffer<A>(var initial):
            {
                var opts = new Ch.BoundedChannelOptions(1) { FullMode = Ch.BoundedChannelFullMode.DropOldest };
                channel = Ch.Channel.CreateBounded<A>(opts);
                channel.Writer.TryWrite(initial);
                break;
            }

            case NewestBuffer<A>(var size):
            {
                var opts = new Ch.BoundedChannelOptions((int)size) { FullMode = Ch.BoundedChannelFullMode.DropOldest };
                channel = Ch.Channel.CreateBounded<A>(opts);
                break;
            }

            case NewBuffer<A>:
            {
                var opts = new Ch.BoundedChannelOptions(1) { FullMode = Ch.BoundedChannelFullMode.DropOldest };
                channel = Ch.Channel.CreateBounded<A>(opts);
                break;
            }

            default:
                throw new NotSupportedException();
        }

        return new Mailbox<A, A>(new InboxWriter<A>(channel.Writer), new OutboxReader<A>(channel.Reader));
    }
}
