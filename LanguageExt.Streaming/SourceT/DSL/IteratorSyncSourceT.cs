using System.Collections.Generic;
using LanguageExt.Traits;

namespace LanguageExt;

record IteratorSyncSourceT<M, A>(IEnumerable<K<M, A>> Items) : SourceT<M, A>
    where M : MonadIO<M>, Alternative<M>
{
    public override K<M, Reduced<S>> ReduceInternalM<S>(S state, ReducerM<M, K<M, A>, S> reducer)
    {
        return go(state, Items.GetIterator());
        K<M, Reduced<S>> go(S state, Iterator<K<M, A>> iter)
        {
            if (iter.IsEmpty) return M.Pure(Reduced.Done(state));
            return reducer(state, iter.Head) >>
                   (ns => ns.Continue
                              ? go(ns.Value, iter.Tail.Split())
                              : M.Pure(ns));
        }
    }
}
