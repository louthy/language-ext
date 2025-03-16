using System;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt.Traits;

namespace LanguageExt.Pipes.Concurrent;

record FilterSourceTIterator<M, A>(SourceTIterator<M, A> Source, Func<A, bool> Predicate) : SourceTIterator<M, A>
    where M : Monad<M>, Alternative<M>
{
    bool ready;
    public override K<M, A> Read() =>
        IO.token
          .BindAsync(async t =>
                     {
                         if(!ready && !await Source.ReadyToRead(t)) return M.Empty<A>();
                         return Source.Read().Bind(v => Predicate(v) ? ClearReadyFlag(v) : Read());
                     });

    K<M, A> ClearReadyFlag(A ma)
    {
        ready = false;
        return M.Pure(ma);
    }

    internal override async ValueTask<bool> ReadyToRead(CancellationToken token)
    {
        ready = await Source.ReadyToRead(token);
        return ready;
    }
}
