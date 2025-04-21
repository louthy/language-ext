using System;
using LanguageExt.Traits;
using Ch = System.Threading.Channels;
namespace LanguageExt;

public static class ConduitT
{
    /// <summary>
    /// Create a new unbounded Conduit 
    /// </summary>
    /// <param name="label">Label for debugging purposes</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <returns>Constructed Conduit with an `Sink` and an `Source`</returns>
    public static ConduitT<M, A, A> make<M, A>() 
        where M : MonadIO<M>, Alternative<M> =>
        make<M, A>(Buffer<A>.Unbounded);

    /// <summary>
    /// Create a new Conduit with the buffer settings provided 
    /// </summary>
    /// <param name="buffer">Buffer settings</param>
    /// <param name="label">Label for debugging purposes</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <returns>Constructed Conduit with an `Sink` and an `Source`</returns>
    /// <exception cref="NotSupportedException">Thrown for invalid buffer settings</exception>
    public static ConduitT<M, A, A> make<M, A>(Buffer<A> buffer)
        where M : MonadIO<M>, Alternative<M> 
    {
        var channel = MakeChannel<M, A>(buffer);
        return new ConduitT<M, A, A>(new SinkWriter<K<M, A>>(channel.Writer).Comap((A a) => M.Pure(a)), 
                                     new ReaderSourceT<M, A>(channel));
    }
    
    static Ch.Channel<K<M, A>> MakeChannel<M, A>(Buffer<A> buffer)
        where M : Maybe.MonadIO<M>, Monad<M>, Alternative<M> 
    {
        Ch.Channel<K<M, A>> channel;
        switch (buffer)
        {
            case UnboundedBuffer<A>:
                channel = Ch.Channel.CreateUnbounded<K<M, A>>();
                break;

            case BoundedBuffer<A>(var size):
            {
                var opts = new Ch.BoundedChannelOptions((int)size) { FullMode = Ch.BoundedChannelFullMode.Wait };
                channel = Ch.Channel.CreateBounded<K<M, A>>(opts);
                break;
            }

            case SingleBuffer<A>:
            {
                var opts = new Ch.BoundedChannelOptions(1) { FullMode = Ch.BoundedChannelFullMode.Wait };
                channel = Ch.Channel.CreateBounded<K<M, A>>(opts);
                break;
            }

            case LatestBuffer<A>(var initial):
            {
                var opts = new Ch.BoundedChannelOptions(1) { FullMode = Ch.BoundedChannelFullMode.DropOldest };
                channel = Ch.Channel.CreateBounded<K<M, A>>(opts);
                channel.Writer.TryWrite(M.Pure(initial));
                break;
            }

            case NewestBuffer<A>(var size):
            {
                var opts = new Ch.BoundedChannelOptions((int)size) { FullMode = Ch.BoundedChannelFullMode.DropOldest };
                channel = Ch.Channel.CreateBounded<K<M, A>>(opts);
                break;
            }

            case NewBuffer<A>:
            {
                var opts = new Ch.BoundedChannelOptions(1) { FullMode = Ch.BoundedChannelFullMode.DropOldest };
                channel = Ch.Channel.CreateBounded<K<M, A>>(opts);
                break;
            }

            default:
                throw new NotSupportedException();
        }

        return channel;
    }
}
