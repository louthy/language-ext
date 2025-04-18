using System.Threading;
using System.Threading.Tasks;

namespace LanguageExt.Pipes.Concurrent;

record PureSource<A>(A Value) : Source<A>
{
    internal override ValueTask<Reduced<S>> ReduceAsync<S>(S state, ReducerAsync<A, S> reducer, CancellationToken token) => 
        reducer(state, Value);
}
