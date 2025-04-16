using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using LanguageExt.Traits;

namespace LanguageExt.Pipes.Concurrent;

record Reader2SourceTIterator<M, A, B>(
    ChannelReader<K<M, A>> ReaderA, 
    ChannelReader<K<M, B>> ReaderB) : SourceTIterator<M, (A First, B Second)>
    where M : MonadIO<M>, Alternative<M>
{
    public override ReadResult<M, (A First, B Second)> Read()
    {
        return ReadResult<M>.Value(IO.token.BindAsync(go));
        
        async ValueTask<K<M, (A First, B Second)>> go(CancellationToken token)
        {
            var ta = ReaderA.ReadAsync(token).AsTask();
            var tb = ReaderB.ReadAsync(token).AsTask();
            await Task.WhenAll(ta, tb);
            return ta.Result.Zip(tb.Result);
        }
    }

    internal override async ValueTask<bool> ReadyToRead(CancellationToken token) =>
        !token.IsCancellationRequested       &&
        await ReaderA.WaitToReadAsync(token) &&
        await ReaderB.WaitToReadAsync(token);
}
