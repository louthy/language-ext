using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LanguageExt.Pipes.Concurrent;

record IteratorAsyncSource<A>(IAsyncEnumerable<A> Items) : Source<A>
{
    internal override SourceIterator<A> GetIterator() =>
        new IteratorAsyncSourceIterator<A> { Src = Items.GetIteratorAsync() };
}
