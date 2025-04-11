using System;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

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

    S CurrentState { get; set; } = State;
    
    public override ReadResult<M, S> Read() =>
        Source.Read() switch
        {
            ReadM<M, A> (var ma) =>
                ReadResult<M>.Iter(ma.Map(x =>
                                          {
                                              var cs = CurrentState;
                                              if (!Pred(cs, x)) return new SingletonSourceTIterator<M, S>(cs);
                                              var ns = Folder(CurrentState, x);
                                              CurrentState = ns;
                                              return EmptySourceTIterator<M, S>.Default;
                                          })),
            
            ReadIter<M, A> (var miter) =>
                ReadResult<M>.Iter(
                    miter.Map(
                        iter => (SourceTIterator<M, S>)new FoldUntilSourceTIterator<M, A, S>(
                            iter, 
                            Schedule,
                            Folder,
                            Pred,
                            CurrentState)))
        };

    internal override async ValueTask<bool> ReadyToRead(CancellationToken token) =>
        !token.IsCancellationRequested && await Source.ReadyToRead(token);
}
