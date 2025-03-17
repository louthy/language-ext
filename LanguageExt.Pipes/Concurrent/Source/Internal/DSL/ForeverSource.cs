using System.Threading;
using System.Threading.Tasks;

namespace LanguageExt.Pipes.Concurrent;

record ForeverSource<A>(A Value) : Source<A>
{
    internal override SourceIterator<A> GetIterator() =>
        new ForeverSourceIterator<A>(Value);

    internal override async ValueTask<Reduced<S>> ReduceAsync<S>(S state, Reducer<A, S> reducer, CancellationToken token)
    {
        while (true)
        {
            switch (await reducer(state, Value))
            {
                case { Continue: true, Value: var nstate }:
                    state = nstate;
                    break;
                
                case { Value: var nstate }:
                    return Reduced.Done(nstate);
            }
        }
    }
}
