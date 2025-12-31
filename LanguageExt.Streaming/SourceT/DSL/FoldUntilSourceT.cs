using System;
using LanguageExt.Traits;

namespace LanguageExt;

record FoldUntilSourceT<M, A, S>(
    SourceT<M, A> Source,
    Schedule Schedule,
    Func<S, A, S> Folder,
    Func<(S State, A Value), bool> Pred,
    S State) : SourceT<M, S>
    where M : MonadIO<M>
{
    // TODO: Schedule
    internal override K<M, Reduced<S1>> ReduceInternalM<S1>(S1 state, ReducerM<M, K<M, S>, S1> reducer) =>
        Source.ReduceInternalM((FState: State, IState: state),
                               (rs, ma) => from a in ma
                                           let ns = Folder(rs.FState, a)
                                           from r in Pred((ns, a))
                                                         ? reducer(rs.IState, M.Pure(ns)).Map(s1 => s1.Map(s2 => (ns, s2)))
                                                         : M.Pure(Reduced.Continue((ns, rs.IState)))
                                           select r)
              .Map(s => s.Map(s1 => s1.IState));
}
