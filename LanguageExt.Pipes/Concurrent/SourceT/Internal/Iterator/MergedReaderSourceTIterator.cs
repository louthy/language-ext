using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt.Pipes.Concurrent;

record MergedReaderSourceTIterator<M, A>(ChannelReader<K<M, A>> Reader, EnvIO EnvIO) : SourceTIterator<M, A>
    where M : MonadIO<M>, Alternative<M>
{
    public override ReadResult<M, A> Read()
    {
        return ReadResult<M>.Value(IO.token.BindAsync(go));
        
        async ValueTask<K<M, A>> go(CancellationToken token)
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
