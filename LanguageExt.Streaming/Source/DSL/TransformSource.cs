using System.Threading;
using System.Threading.Tasks;

namespace LanguageExt;

record TransformSource<A, B>(Source<A> Source, Transducer<A, B> Transducer) : Source<B>
{
    internal override ValueTask<Reduced<S>> ReduceAsync<S>(S state, ReducerAsync<B, S> reducer, CancellationToken token) =>
        Source.ReduceAsync(state, Transducer.Reduce(reducer), token);
}
