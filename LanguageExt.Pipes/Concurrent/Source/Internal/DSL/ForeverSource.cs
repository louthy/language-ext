using System.Threading;
using System.Threading.Tasks;

namespace LanguageExt.Pipes.Concurrent;

record ForeverSource<A>(A Value) : Source<A>
{
    internal override SourceIterator<A> GetIterator() =>
        new ForeverSourceIterator<A>(Value);
}
