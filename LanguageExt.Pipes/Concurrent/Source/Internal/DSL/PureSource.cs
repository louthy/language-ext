using System.Threading;
using System.Threading.Tasks;

namespace LanguageExt.Pipes.Concurrent;

record PureSource<A>(A Value) : Source<A>
{
    internal override SourceIterator<A> GetIterator() =>
        new SingletonSourceIterator<A>(Value);
}
