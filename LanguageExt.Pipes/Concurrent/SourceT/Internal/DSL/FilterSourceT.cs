using System;
using LanguageExt.Traits;

namespace LanguageExt.Pipes.Concurrent;

record FilterSourceT<M, A>(SourceT<M, A> Source, Func<A, bool> Predicate) : SourceT<M, A>
    where M : MonadIO<M>, Alternative<M>
{
    internal override SourceTIterator<M, A> GetIterator() =>
        new FilterSourceTIterator<M, A>(Source.GetIterator(), Predicate);
}
