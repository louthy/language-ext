using System.Threading;
using System.Threading.Tasks;

namespace LanguageExt;

record ForeverSource<A>(A Value) : Source<A>
{
    internal override async ValueTask<Reduced<S>> ReduceAsync<S>(S state, ReducerAsync<A, S> reducer, CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            switch (await reducer(state, Value))
            {
                case { Continue: true, Value: var value }:
                    state = value;
                    break;
                
                case { Value: var value }:
                    return Reduced.Done(value);
            }
        }
        return Reduced.Done(state);
    }
}
