using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LanguageExt;

record IteratorSyncSource<A>(IEnumerable<A> Items) : Source<A>
{
    internal override async ValueTask<Reduced<S>> ReduceAsync<S>(S state, ReducerAsync<A, S> reducer, CancellationToken token)
    {
        foreach (var item in Items)
        {
            if (token.IsCancellationRequested) return Reduced.Done(state);
            
            switch (await reducer(state, item))
            {
                case { Continue: true, Value: var value }:
                    state = value;
                    break;
                
                case { Value: var value }:
                    return Reduced.Done(value);
            }
        }
        return Reduced.Continue(state);
    }
}
