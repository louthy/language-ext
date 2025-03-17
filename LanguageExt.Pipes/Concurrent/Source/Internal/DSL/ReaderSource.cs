using System.Threading.Channels;

namespace LanguageExt.Pipes.Concurrent;

record ReaderSource<A>(Channel<A> Channel) : Source<A>
{
    internal override SourceIterator<A> GetIterator() =>
        new ReaderSourceIterator<A>(Channel);
}
