using System.Threading;
using System.Threading.Tasks;
using static LanguageExt.Prelude;
using System.Collections.Concurrent;

namespace LanguageExt.Pipes.Concurrent;

record Zip2Source<A, B>(Source<A> SourceA, Source<B> SourceB) : Source<(A First, B Second)>
{
    internal override SourceIterator<(A First, B Second)> GetIterator() =>
        new Zip2SourceIterator<A, B>(SourceA.GetIterator(), SourceB.GetIterator());
}
