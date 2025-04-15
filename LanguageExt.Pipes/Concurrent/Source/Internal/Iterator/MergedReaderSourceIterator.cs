using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using LanguageExt.Common;

namespace LanguageExt.Pipes.Concurrent;

record MergedReaderSourceIterator<A>(ChannelReader<A> Reader, EnvIO EnvIO) : SourceIterator<A>
{
    internal override async ValueTask<A> ReadValue(CancellationToken token)
    {
        if (token.IsCancellationRequested)
        {
            await EnvIO.Source.CancelAsync();
            EnvIO.Dispose();
            throw Errors.Cancelled;
        }

        try
        {
            return await Reader.ReadAsync(token);
        }
        catch
        {
            await EnvIO.Source.CancelAsync();
            EnvIO.Dispose();
            throw;
        }
    }

    internal override async ValueTask<bool> ReadyToRead(CancellationToken token)
    {
        if (token.IsCancellationRequested)
        {
            await EnvIO.Source.CancelAsync();
            EnvIO.Dispose();
            return false;
        }
        
        try
        {
            var f = await Reader.WaitToReadAsync(token);
            if (!f) EnvIO.Dispose();
            return f;
        }
        catch
        {
            await EnvIO.Source.CancelAsync();
            EnvIO.Dispose();
            throw;
        }
    }
}
