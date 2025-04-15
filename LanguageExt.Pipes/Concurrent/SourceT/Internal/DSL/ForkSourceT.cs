using LanguageExt.Traits;

namespace LanguageExt.Pipes.Concurrent;

record ForkSourceT<M, A>(SourceT<M, A> Source) : SourceT<M, ForkIO<A>>
    where M : MonadIO<M>, Alternative<M>
{
    internal override SourceTIterator<M, ForkIO<A>> GetIterator() => 
        throw new System.NotImplementedException();
}
