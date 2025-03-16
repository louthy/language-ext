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
    // TODO: Support Schedule

    internal override K<M, S1> ReduceInternal<S1>(S1 state, ReducerM<M, S, S1> reducer)
    {
        return go(state, State, Source.GetIterator());

        K<M, S1> go(S1 state, S foldState, SourceTIterator<M, A> iter) =>
            IO.liftVAsync(e => iter.ReadyToRead(e.Token))
              .Bind(flag => flag ? iter.Read()
                                       .Bind(x =>
                                             {
                                                 if (Pred(foldState, x))
                                                 {
                                                     return go(state, Folder(foldState, x), iter);
                                                 }
                                                 else
                                                 {
                                                     return reducer(state, foldState).Bind(s => go(s, State, iter));
                                                 }
                                             })
                                : M.Pure(state));
    }


    internal override SourceTIterator<M, S> GetIterator() => 
        new FoldWhileSourceTIterator<M, A, S>(Source.GetIterator(), Schedule, Folder, Pred, State);
}
