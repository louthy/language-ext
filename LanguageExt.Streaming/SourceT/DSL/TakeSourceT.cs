using LanguageExt.Traits;

namespace LanguageExt;

record TakeSourceT<M, A>(SourceT<M, A> Source, int Count) : SourceT<M, A>
    where M : MonadIO<M>, Alternative<M>
{
    public override K<M, Reduced<S>> ReduceInternalM<S>(S state, ReducerM<M, K<M, A>, S> reducer)
    {
        var remaining = Count;
        return Source.ReduceInternalM(state,
                                      (s, ma) =>
                                      {
                                          if (remaining < 1) return M.Pure(Reduced.Done(s));
                                          remaining--;
                                          return remaining == 0
                                                    ? reducer(s, ma).Map(n => Reduced.Done(n.Value))
                                                    : reducer(s, ma);
                                      });
    }
}
