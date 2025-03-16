using System;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt.Traits;

namespace LanguageExt.Pipes.Concurrent;

record FoldWhileSourceTIterator<M, A, S>(
    SourceTIterator<M, A> Source,
    Schedule Schedule,
    Func<S, A, S> Folder,
    Func<S, A, bool> Pred,
    S State) : SourceTIterator<M, S>
    where M : Monad<M>, Alternative<M>
{
    // TODO: Support Schedule

    public override K<M, S> Read()
    {
        return go(State, Pred, this);

        static K<M, S> go(S state, Func<S, A, bool> pred, FoldWhileSourceTIterator<M, A, S> self) =>
            IO.liftVAsync(e => self.ReadyToRead(e.Token))
              .Bind(flag =>
                    {
                        if (flag)
                        {
                            var mx = self.Source.Read();
                            return mx.Bind(
                                x =>
                                {
                                    if (pred(state, x))
                                    {
                                        state = self.Folder(state, x);
                                        return go(state, pred, self);
                                    }
                                    else
                                    {
                                        return M.Pure(state);
                                    }
                                });
                        }
                        else
                        {
                            return M.Pure(state);
                        }
                    });
    }

    internal override ValueTask<bool> ReadyToRead(CancellationToken token) =>
        Source.ReadyToRead(token);
}
