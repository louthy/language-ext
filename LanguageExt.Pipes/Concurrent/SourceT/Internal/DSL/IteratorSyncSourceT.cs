using System.Collections.Generic;
using LanguageExt.Traits;

namespace LanguageExt.Pipes.Concurrent;

record IteratorSyncSourceT<M, A>(IEnumerable<K<M, A>> Items) : SourceT<M, A>
    where M : MonadIO<M>, Alternative<M>
{
    internal override SourceTIterator<M, A> GetIterator() =>
        new IteratorSyncSourceTIterator<M, A> { Src = Items.GetIterator() };
}
