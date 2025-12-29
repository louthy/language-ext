using System.Threading;
using System.Threading.Tasks;

namespace LanguageExt;

record TransformSource<A, B>(Source<A> Source, Transducer<A, B> Transducer) : Source<B>
{
    internal override IO<Reduced<S>> ReduceInternal<S>(S state, ReducerIO<B, S> reducer) =>
        Source.ReduceInternal(state, Transducer.Reduce(reducer));
}
