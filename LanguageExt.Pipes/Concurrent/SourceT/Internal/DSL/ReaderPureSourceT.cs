using System.Threading.Channels;
using LanguageExt.Traits;

namespace LanguageExt.Pipes.Concurrent;

record ReaderPureSourceT<M, A>(Channel<A> Channel) : SourceT<M, A>
    where M : Monad<M>, Alternative<M>
{
    internal override SourceTIterator<M, A> GetIterator() =>
        new ReaderPureSourceTIterator<M, A>(Channel);
}
