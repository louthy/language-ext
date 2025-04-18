using System.Collections.Generic;
using LanguageExt.Traits;

namespace LanguageExt.Pipes.Concurrent;

record IteratorSyncSourceT<M, A>(IEnumerable<K<M, A>> Items) : SourceT<M, A>
    where M : MonadIO<M>, Alternative<M>
{
    public override K<M, S> ReduceM<S>(S state, ReducerM<M, K<M, A>, S> reducer)
    {
        return go(state, Items.GetIterator());
        K<M, S> go(S state, Iterator<K<M, A>> iter)
        {
            if (iter.IsEmpty) return M.Pure(state);
            return reducer(state, iter.Head).Bind(s => go(s, iter.Tail.Split()));
        }
    }
}
