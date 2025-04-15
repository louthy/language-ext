using System.Threading;
using System.Threading.Tasks;
using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt.Pipes.Concurrent;

record IteratorAsyncSourceTIterator<M, A> : SourceTIterator<M, A>
    where M : MonadIO<M>, Alternative<M>
{
    internal required IteratorAsync<K<M, A>> Src;

    public override ReadResult<M, A> Read()
    {
        return ReadResult<M>.Value(IO.token.BindAsync(go));
        async ValueTask<K<M, A>> go(CancellationToken token)
        {
            if (await Src.IsEmpty) return M.Empty<A>();
            var head = await Src.Head;
            Src = (await Src.Tail).Split();
            return head;
        }
    }

    internal override async ValueTask<bool> ReadyToRead(CancellationToken token) =>
        !await Src.IsEmpty;
}
