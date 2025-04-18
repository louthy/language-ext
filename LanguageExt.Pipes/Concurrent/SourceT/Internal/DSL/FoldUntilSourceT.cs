using System;
using LanguageExt.Traits;

namespace LanguageExt.Pipes.Concurrent;

record FoldUntilSourceT<M, A, S>(
    SourceT<M, A> Source,
    Schedule Schedule,
    Func<S, A, S> Folder,
    Func<S, A, bool> Pred,
    S State) : SourceT<M, S>
    where M : MonadIO<M>, Alternative<M>
{
    // TODO: Schedule
    public override K<M, S1> ReduceM<S1>(S1 state, ReducerM<M, K<M, S>, S1> reducer) =>
        Source.ReduceM((FState: State, IState: state),
                       (s, ma) => from a in ma
                                  let ns = Folder(s.FState, a)
                                  from r in Pred(ns, a)
                                                ? reducer(s.IState, M.Pure(ns)).Map(s1 => (ns, s1))
                                                : M.Pure((ns, s.IState))
                                  select r)
              .Map(s => s.IState);
}
