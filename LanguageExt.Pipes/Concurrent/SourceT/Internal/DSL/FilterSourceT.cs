using System;
using LanguageExt.Traits;

namespace LanguageExt.Pipes.Concurrent;

record FilterSourceT<M, A>(SourceT<M, A> Source, Func<A, bool> Predicate) : SourceT<M, A>
    where M : Monad<M>, Alternative<M>
{
    internal override SourceTIterator<M, A> GetIterator() =>
        new FilterSourceTIterator<M, A>(Source.GetIterator(), Predicate);

    internal override K<M, S> ReduceInternal<S>(S state, ReducerM<M, A, S> reducer)
    {
        return read(state, Source.GetIterator());

        K<M, S> read(S state, SourceTIterator<M, A> iter) =>
            IO.liftVAsync(e => iter.ReadyToRead(e.Token))
              .Bind(f => f ? iter.Read()
                                 .Bind(x => Predicate(x) 
                                                ? reducer(state, x).Bind(ns => read(ns, iter)) 
                                                : M.Pure(state).Bind(ns => read(ns, iter)))
                           : M.Pure(state));
    }
}
