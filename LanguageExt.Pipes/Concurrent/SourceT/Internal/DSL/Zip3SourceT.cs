using System;
using static LanguageExt.Prelude;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using LanguageExt.Traits;

namespace LanguageExt.Pipes.Concurrent;

record Zip3SourceT<M, A, B, C>(
    SourceT<M, A> SourceTA, 
    SourceT<M, B> SourceTB, 
    SourceT<M, C> SourceTC) : 
    SourceT<M, (A First, B Second, C Third)>
    where M : MonadIO<M>, Alternative<M>
{
    internal override SourceTIterator<M, (A First, B Second, C Third)> GetIterator() =>
        new Zip3SourceTIterator<M, A, B, C>(SourceTA.GetIterator(), SourceTB.GetIterator(), SourceTC.GetIterator());
}
