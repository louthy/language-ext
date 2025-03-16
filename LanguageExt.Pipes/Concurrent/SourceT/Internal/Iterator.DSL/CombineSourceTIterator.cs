using System.Threading;
using System.Threading.Tasks;
using LanguageExt.Traits;

namespace LanguageExt.Pipes.Concurrent;

record CombineSourceTIterator<M, A>(Seq<SourceTIterator<M, A>> SourceTs) : SourceTIterator<M, A>
    where M : Monad<M>, Alternative<M>
{
    internal override ValueTask<bool> ReadyToRead(CancellationToken token)
    {
        if (SourceTs.Count == 0) return new(false);
        if (SourceTs.Count == 1) return SourceTs[0].ReadyToRead(token);
        return SourceTInternal.ReadyToRead(SourceTs, token);
    }

    public override K<M, A> Read()
    {
        if (SourceTs.Count == 0) return M.Empty<A>();
        if (SourceTs.Count == 1) return SourceTs[0].Read();
        return IO.token.BindAsync(t => SourceTInternal.Read(SourceTs, t));
    }
}
