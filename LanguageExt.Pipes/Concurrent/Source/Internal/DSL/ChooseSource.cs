using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using static LanguageExt.Prelude;

namespace LanguageExt.Pipes.Concurrent;

record ChooseSource<A>(Source<A> SourceA, Source<A> SourceB) : Source<A>
{
    internal override SourceIterator<A> GetIterator() =>
        new ChooseSourceIterator<A>(SourceA.GetIterator(), SourceB.GetIterator());
}
