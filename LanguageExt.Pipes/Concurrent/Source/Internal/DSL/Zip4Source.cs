using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using static LanguageExt.Prelude;

namespace LanguageExt.Pipes.Concurrent;

record Zip4Source<A, B, C, D>(Source<A> SourceA, Source<B> SourceB, Source<C> SourceC, Source<D> SourceD) 
    : Source<(A First, B Second, C Third, D Fourth)>
{
    internal override SourceIterator<(A First, B Second, C Third, D Fourth)> GetIterator() =>
        new Zip4SourceIterator<A, B, C, D>(
            SourceA.GetIterator(), 
            SourceB.GetIterator(), 
            SourceC.GetIterator(), 
            SourceD.GetIterator());
}
