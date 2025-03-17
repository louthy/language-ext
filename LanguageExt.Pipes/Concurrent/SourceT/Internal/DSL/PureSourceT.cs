using LanguageExt.Traits;

namespace LanguageExt.Pipes.Concurrent;

record PureSourceT<M, A>(A Value) : SourceT<M, A>
    where M : MonadIO<M>, Alternative<M>
{
    internal override SourceTIterator<M, A> GetIterator() =>
        new SingletonSourceTIterator<M, A>(Value);
}
