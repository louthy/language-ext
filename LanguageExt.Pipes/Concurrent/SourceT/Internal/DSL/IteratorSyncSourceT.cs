using System.Collections.Generic;
using LanguageExt.Traits;

namespace LanguageExt.Pipes.Concurrent;

record IteratorSyncSourceT<M, A>(IEnumerable<K<M, A>> Items) : SourceT<M, A>
    where M : Monad<M>, Alternative<M>
{
    internal override SourceTIterator<M, A> GetIterator() =>
        new IteratorSyncSourceTIterator<M, A> { Src = Items.GetIterator() };

    internal override K<M, S> ReduceInternal<S>(S state, ReducerM<M, A, S> reducer)
    {
        return go(state, Items.GetIterator());

        K<M, S> go(S state, Iterator<K<M, A>> iter)
        {
            var self = iter.Clone();
            return self.IsEmpty
                       ? M.Pure(state)
                       : self.Head
                             .Bind(a => reducer(state, a)
                                      .Bind(s => go(s, self.Tail.Split())));
        }
    }
}
