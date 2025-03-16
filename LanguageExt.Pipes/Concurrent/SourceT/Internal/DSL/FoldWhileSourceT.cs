using System;
using LanguageExt.Traits;

namespace LanguageExt.Pipes.Concurrent;

record FoldWhileSourceT<M, A, S>(
    SourceT<M, A> Source,
    Schedule Schedule,
    Func<S, A, S> Folder,
    Func<S, A, bool> Pred,
    S State) : SourceT<M, S>
    where M : Monad<M>, Alternative<M>
{
    internal override SourceTIterator<M, S> GetIterator() => 
        new FoldWhileSourceTIterator<M, A, S>(Source.GetIterator(), Schedule, Folder, Pred, State);
}
