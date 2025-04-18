using System.Threading;
using System.Threading.Tasks;

namespace LanguageExt;

record CombineSource<A>(Seq<Source<A>> Sources) : Source<A>
{
    internal override async ValueTask<Reduced<S>> ReduceAsync<S>(S state, ReducerAsync<A, S> reducer, CancellationToken token)
    {
        foreach (var source in Sources)
        {
            switch (await source.ReduceAsync(state, reducer, token))
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
