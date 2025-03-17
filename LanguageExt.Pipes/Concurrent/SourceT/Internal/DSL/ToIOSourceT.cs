using LanguageExt.Traits;

namespace LanguageExt.Pipes.Concurrent;

record ToIOSourceT<M, A>(SourceT<M, A> Source) : SourceT<M, IO<A>>
    where M : MonadIO<M>, Alternative<M>
{
    internal override SourceTIterator<M, IO<A>> GetIterator() =>
        new ToIOSourceTIterator<M, A>(Source.GetIterator());
}
