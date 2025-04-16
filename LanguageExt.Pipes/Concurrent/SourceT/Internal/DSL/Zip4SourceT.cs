using System;
using LanguageExt.Traits;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using static LanguageExt.Prelude;

namespace LanguageExt.Pipes.Concurrent;

record Zip4SourceT<M, A, B, C, D>(SourceT<M, A> SourceA, SourceT<M, B> SourceB, SourceT<M, C> SourceC, SourceT<M, D> SourceD) 
    : SourceT<M, (A First, B Second, C Third, D Fourth)>
    where M : MonadIO<M>, Alternative<M>
{
    internal override SourceTIterator<M, (A First, B Second, C Third, D Fourth)> GetIterator() =>
        new Zip4SourceTIterator<M, A, B, C, D>(
            SourceA.GetIterator(), 
            SourceB.GetIterator(), 
            SourceC.GetIterator(), 
            SourceD.GetIterator());
}
