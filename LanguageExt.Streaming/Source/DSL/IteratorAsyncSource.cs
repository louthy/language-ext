using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace LanguageExt;

record IteratorAsyncSource<A>(IAsyncEnumerable<A> Items) : Source<A>
{
    internal override async ValueTask<Reduced<S>> ReduceAsync<S>(
        S state, 
        ReducerAsync<A, S> reducer, 
        CancellationToken token)
    {
        await foreach (var item in Items)
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
