using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using LanguageExt.Common;

namespace LanguageExt.Pipes.Concurrent;

record ReaderSourceIterator<A>(ChannelReader<A> Reader) : SourceIterator<A>
{
    internal override ValueTask<A> ReadValue(CancellationToken token)
    {
        if(token.IsCancellationRequested) return ValueTask.FromException<A>(Errors.Cancelled);
        var tf = Reader.WaitToReadAsync(token);
        if (tf.IsCompleted)
        {
            if (tf.Result)
            {
                return Reader.ReadAsync(token);
            }
            else
            {
                return ValueTask.FromException<A>(Errors.SourceClosed);
            }
        }
        return ReadValueAsync(tf, token);
    }

    async ValueTask<A> ReadValueAsync(ValueTask<bool> tflag, CancellationToken token)
    {
        var f = await tflag;
        if (f)
        {
            return await Reader.ReadAsync(token);
        }
        else
        {
            throw Errors.SourceClosed;
        }
    }

    internal override ValueTask<bool> ReadyToRead(CancellationToken token) =>
        Reader.WaitToReadAsync(token);
}
