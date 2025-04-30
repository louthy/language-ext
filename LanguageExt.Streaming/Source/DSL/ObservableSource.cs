using System;
using System.Threading;
using System.Threading.Tasks;

namespace LanguageExt;

record ObservableSource<A>(IObservable<A> Items) : Source<A>
{
    internal override async ValueTask<Reduced<S>> ReduceAsync<S>(
        S state, 
        ReducerAsync<A, S> reducer, 
        CancellationToken token)
    {
        await foreach (var item in Items.ToAsyncEnumerable(token))
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
