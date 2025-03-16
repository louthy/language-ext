using System.Collections.Generic;
using System.Threading.Tasks;
using LanguageExt.Traits;
using System.Threading;

namespace LanguageExt.Pipes.Concurrent;

record IteratorAsyncSourceT<M, A>(IAsyncEnumerable<K<M, A>> Items) : SourceT<M, A>
    where M : Monad<M>, Alternative<M>
{
    internal override SourceTIterator<M, A> GetIterator() =>
        new IteratorAsyncSourceTIterator<M, A> { Src = Items.GetIteratorAsync() };
}
