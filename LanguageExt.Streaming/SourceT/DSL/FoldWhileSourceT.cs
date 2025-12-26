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
    public override K<M, Reduced<S1>> ReduceInternalM<S1>(S1 state, ReducerM<M, K<M, S>, S1> reducer) =>
        Source.ReduceInternalM((FState: State, IState: state),
                               (s, ma) =>
                                   ma >> (a => Pred(s.FState, a)
                                                   ? M.Pure(Reduced.Done((Folder(s.FState, a), s.IState)))
                                                   : reducer(s.IState, M.Pure(s.FState))
                                                        .Map(s1 => s1.Map(s2 => (s.FState, s2)))))
              .Map(s => s.Map(s1 => s1.IState));
}
