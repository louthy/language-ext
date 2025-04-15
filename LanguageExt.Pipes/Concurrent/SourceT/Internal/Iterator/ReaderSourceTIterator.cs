using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using LanguageExt.Traits;

namespace LanguageExt.Pipes.Concurrent;

record ReaderSourceTIterator<M, A>(ChannelReader<K<M, A>> Reader) : SourceTIterator<M, A>
    where M : MonadIO<M>, Alternative<M>
{
    public override ReadResult<M, A> Read()
    {
        return ReadResult<M>.Value(IO.token.BindAsync(go));
        
        async ValueTask<K<M, A>> go(CancellationToken token)
        {
            if(await Reader.WaitToReadAsync(token))
            {
                return await Reader.ReadAsync(token);
            }
            else
            {
                return M.Empty<A>();
            }
        }
    }

    internal override async ValueTask<bool> ReadyToRead(CancellationToken token) =>
        !token.IsCancellationRequested && await Reader.WaitToReadAsync(token);
}
