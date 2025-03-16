using System.Threading.Channels;
using LanguageExt.Traits;

namespace LanguageExt.Pipes.Concurrent;

record ReaderSourceT<M, A>(Channel<K<M, A>> Channel) : SourceT<M, A>
    where M : Monad<M>, Alternative<M>
{
    internal override SourceTIterator<M, A> GetIterator() =>
        new ReaderSourceTIterator<M, A>(Channel);
}
