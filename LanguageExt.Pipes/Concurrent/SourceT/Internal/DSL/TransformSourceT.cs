using LanguageExt.Traits;

namespace LanguageExt.Pipes.Concurrent;

record TransformSourceT<M, A, B>(SourceT<M, A> SourceT, Transducer<A, B> Transducer) : SourceT<M, B>
    where M : Monad<M>, Alternative<M>
{
    internal override SourceTIterator<M, B> GetIterator() =>
        new TransformSourceTIterator<M, A, B>(SourceT.GetIterator(), Transducer);

    internal override K<M, S> ReduceInternal<S>(S state, ReducerM<M, B, S> reducer)
    {
        return read(state, SourceT.GetIterator());

        K<M, S> read(S state, SourceTIterator<M, A> iter) =>
            IO.liftVAsync(e => iter.ReadyToRead(e.Token))
              .Bind(f => f
                             ? iter.Read()
                                   .Bind(x => Transducer.ReduceM(reducer)(state, x))
                                   .Bind(ns => read(ns, iter))
                             : M.Pure(state));
    }
}
