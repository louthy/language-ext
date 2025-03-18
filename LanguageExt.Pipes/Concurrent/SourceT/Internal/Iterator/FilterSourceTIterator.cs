using System;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt.Common;
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
                         if (ready || await ReadyToRead(t))
                         {
                             return Source.Read().Bind(v => Predicate(v) 
                                                                ? ClearReadyFlag(v) 
                                                                : Read());    
                         }
                         else
                         {
                             return M.Empty<A>();
                         }
                     });

    K<M, A> ClearReadyFlag(A ma)
    {
        ready = false;
        return M.Pure(ma);
    }

    internal override async ValueTask<bool> ReadyToRead(CancellationToken token)
    {
        if(token.IsCancellationRequested) return false;
        ready = await Source.ReadyToRead(token);
        return ready;
    }
}
