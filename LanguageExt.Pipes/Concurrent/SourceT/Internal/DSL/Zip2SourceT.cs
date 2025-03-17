using System;
using LanguageExt.Traits;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using static LanguageExt.Prelude;

namespace LanguageExt.Pipes.Concurrent;

record Zip2SourceT<M, A, B>(SourceT<M, A> SourceTA, SourceT<M, B> SourceTB) : SourceT<M, (A First, B Second)>
    where M : MonadIO<M>, Alternative<M>
{
    internal override SourceTIterator<M, (A First, B Second)> GetIterator() =>
        new Zip2SourceTIterator<M, A, B>(SourceTA.GetIterator(), SourceTB.GetIterator());
}
