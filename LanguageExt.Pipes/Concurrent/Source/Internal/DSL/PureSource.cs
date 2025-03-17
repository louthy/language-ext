using System.Threading;
using System.Threading.Tasks;

namespace LanguageExt.Pipes.Concurrent;

record PureSource<A>(A Value) : Source<A>
{
    internal override SourceIterator<A> GetIterator() =>
        new SingletonSourceIterator<A>(Value);
    
    internal override ValueTask<Reduced<S>> ReduceAsync<S>(S state, Reducer<A, S> reducer, CancellationToken token) =>
        reducer(state, Value);
}
