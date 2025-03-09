using System;
using Ch = System.Threading.Channels;
namespace LanguageExt.Pipes.Concurrent;

public static class Conduit
{
    /// <summary>
    /// Create a new unbounded Conduit 
    /// </summary>
    /// <param name="label">Label for debugging purposes</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <returns>Constructed Conduit with an `Sink` and an `Source`</returns>
    public static Conduit<A, A> spawn<A>() =>
        spawn(Buffer<A>.Unbounded);

    /// <summary>
    /// Create a new Conduit with the buffer settings provided 
    /// </summary>
    /// <param name="buffer">Buffer settings</param>
    /// <param name="label">Label for debugging purposes</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <returns>Constructed Conduit with an `Sink` and an `Source`</returns>
    /// <exception cref="NotSupportedException">Thrown for invalid buffer settings</exception>
    public static Conduit<A, A> spawn<A>(Buffer<A> buffer)
    {
        var channel = MakeChannel(buffer);
        return new Conduit<A, A>(new SinkWriter<A>(channel.Writer), new ReaderSource<A>(channel));
    }
    
    static Ch.Channel<A> MakeChannel<A>(Buffer<A> buffer)
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

        return channel;
    }
}
