using LanguageExt.Traits;

namespace LanguageExt.Pipes.Concurrent;

record TransformSourceT<M, A, B>(SourceT<M, A> SourceT, Transducer<A, B> Transducer) : SourceT<M, B>
    where M : Monad<M>, Alternative<M>
{
    internal override SourceTIterator<M, B> GetIterator() =>
        new TransformSourceTIterator<M, A, B>(SourceT.GetIterator(), Transducer);
}
