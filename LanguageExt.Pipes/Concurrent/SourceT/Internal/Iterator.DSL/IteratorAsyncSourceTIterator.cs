using System.Threading;
using System.Threading.Tasks;
using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt.Pipes.Concurrent;

record IteratorAsyncSourceTIterator<M, A> : SourceTIterator<M, A>
    where M : Monad<M>, Alternative<M>
{
    internal required IteratorAsync<K<M, A>> Src;

    public override K<M, A> Read()
    {
        return IO.token.BindAsync(go);
        async ValueTask<K<M, A>> go(CancellationToken token)
        {
            if (token.IsCancellationRequested) throw Errors.Cancelled;
            if (await Src.IsEmpty) return M.Empty<A>();
            var head = await Src.Head;
            Src = (await Src.Tail).Split();
            return head;
        }
    }

    internal override async ValueTask<bool> ReadyToRead(CancellationToken token) =>
        !await Src.IsEmpty;
}
