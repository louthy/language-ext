using LanguageExt.Traits;

namespace LanguageExt;

record TransformSourceT<M, A, B>(SourceT<M, A> Source, TransducerM<M, A, B> Transducer) : SourceT<M, B>
    where M : MonadIO<M>
{
    internal override K<M, Reduced<S>> ReduceInternalM<S>(S state, ReducerM<M, K<M, B>, S> reducer)
    {
        var t = Transducer.Reduce<S>((s1, b) => reducer(s1, M.Pure(b)));
        return Source.ReduceInternalM(state, (s, ma) => ma.Bind(a => t(s, a)));
    }
}

record TransformSourceT2<M, A, B>(SourceT<M, A> Source, Transducer<A, B> Transducer) : SourceT<M, B>
    where M : MonadIO<M>
{
    readonly TransducerM<M, A, B> TransducerM = Transducer.Lift<M>();
    
    internal override K<M, Reduced<S>> ReduceInternalM<S>(S state, ReducerM<M, K<M, B>, S> reducer)
    {
        var t  = TransducerM.Reduce((S s1, B b) => reducer(s1, M.Pure(b)));
        return Source.ReduceInternalM(state, (s, ma) => ma.Bind(a => t(s, a)));
    }
}
