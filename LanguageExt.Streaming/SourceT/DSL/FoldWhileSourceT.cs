using System;
using LanguageExt.Traits;

namespace LanguageExt;

record FoldWhileSourceT<M, A, S>(
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
                       (s, ma) => ma.Bind(a => Pred(s.FState, a)
                                                   ? M.Pure((Folder(s.FState, a), s.IState))
                                                   : reducer(s.IState, M.Pure(s.FState)).Map(s1 => (s.FState, s1))))
              .Map(s => s.IState);
}
