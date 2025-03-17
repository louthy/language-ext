using System;
using LanguageExt.Traits;

namespace LanguageExt.Pipes.Concurrent;

record MapIOSourceT<M, A, B>(SourceT<M, A> Source, Func<IO<A>, IO<B>> F) : SourceT<M, B>
    where M : MonadIO<M>, Alternative<M>
{
    internal override SourceTIterator<M, B> GetIterator() =>
        new MapIOSourceTIterator<M, A, B>(Source.GetIterator(), F);
}
