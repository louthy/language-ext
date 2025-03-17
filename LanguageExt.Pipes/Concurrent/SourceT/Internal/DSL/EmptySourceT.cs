using LanguageExt.Traits;

namespace LanguageExt.Pipes.Concurrent;

record EmptySourceT<M, A> : SourceT<M, A>
    where M : MonadIO<M>, Alternative<M>
{
    public static readonly SourceT<M, A> Default = new EmptySourceT<M, A>();

    internal override SourceTIterator<M, A> GetIterator() =>
        EmptySourceTIterator<M, A>.Default;
}
