using System;
using LanguageExt.Traits;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using static LanguageExt.Prelude;

namespace LanguageExt.Pipes.Concurrent;

record Zip4SourceT<M, A, B, C, D>(SourceT<M, A> SourceTA, SourceT<M, B> SourceTB, SourceT<M, C> SourceTC, SourceT<M, D> SourceTD) 
    : SourceT<M, (A First, B Second, C Third, D Fourth)>
    where M : MonadIO<M>, Alternative<M>
{
    internal override SourceTIterator<M, (A First, B Second, C Third, D Fourth)> GetIterator() =>
        new Zip4SourceTIterator<M, A, B, C, D>(
            SourceTA.GetIterator(), 
            SourceTB.GetIterator(), 
            SourceTC.GetIterator(), 
            SourceTD.GetIterator());
}
