using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LanguageExt.Pipes.Concurrent;

record IteratorAsyncSource<A>(IAsyncEnumerable<A> Items) : Source<A>
{
    internal override SourceIterator<A> GetIterator() =>
        new IteratorAsyncSourceIterator<A> { Src = Items.GetIteratorAsync() };
    
    internal override async ValueTask<Reduced<S>> ReduceAsync<S>(S state, Reducer<A, S> reducer, CancellationToken token)
    {
        await foreach (var item in Items)
        {
            switch (await reducer(state, item))
            {
                case { Continue: true, Value: var nstate }:
                    state = nstate;
                    break;
                    
                case { Value: var nstate }:
                    return Reduced.Done(nstate);
            }
        }
        return Reduced.Done(state);
    }
}
