using LanguageExt.Traits;

namespace LanguageExt;

record TransformSourceT<M, A, B>(SourceT<M, A> Source, TransducerM<M, A, B> Transducer) : SourceT<M, B>
    where M : MonadIO<M>
{
    public override K<M, Reduced<S>> ReduceInternalM<S>(S state, ReducerM<M, K<M, B>, S> reducer)
    {
        var t = Transducer.Reduce<S>((s1, b) => reducer(s1, M.Pure(b)));
        return Source.ReduceInternalM(state, (s, ma) => ma.Bind(a => t(s, a)));
    }
}
