using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt.Common;

namespace LanguageExt.Pipes.Concurrent;

record IteratorSyncSource<A>(IEnumerable<A> Items) : Source<A>
{
    internal override SourceIterator<A> GetIterator() =>
        new IteratorSyncSourceIterator<A> { Src = Items.GetIterator() };
}
