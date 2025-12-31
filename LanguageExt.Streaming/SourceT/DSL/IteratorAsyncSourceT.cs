using System.Collections.Generic;
using LanguageExt.Traits;

namespace LanguageExt;

record IteratorAsyncSourceT<M, A>(IAsyncEnumerable<K<M, A>> Items) : SourceT<M, A>
    where M : MonadIO<M>
{
    internal override K<M, Reduced<S>> ReduceInternalM<S>(S state, ReducerM<M, K<M, A>, S> reducer) 
    {
        return Monad.recur((state, Items.GetIteratorAsync()), go);

        K<M, Next<(S state, IteratorAsync<K<M, A>> iter), Reduced<S>>>
            go((S state, IteratorAsync<K<M, A>> iter) self) =>
            isDone(self) >> (d => d ? done(self.state)
                                    : reduce(self.iter) >>
                                      (ns => ns.Continue
                                                 ? next(ns.Value, tail(self.iter))
                                                 : reduced(ns)));

        K<M, Reduced<S>> reduce(IteratorAsync<K<M, A>> iter) =>
            IO.liftVAsync(_ => iter.Head).Bind(h => reducer(state, h));
        
        IO<bool> isDone((S state, IteratorAsync<K<M, A>> iter) self) =>
            IO.liftVAsync(async e => e.Token.IsCancellationRequested || await self.iter.IsEmpty);

        K<M, Next<(S state, IteratorAsync<K<M, A>> iter), Reduced<S>>> done(S state) =>
            reduced(Reduced.Done(state));

        K<M, Next<(S state, IteratorAsync<K<M, A>> iter), Reduced<S>>> reduced(Reduced<S> reduced) =>
            M.Pure(Next.Done<(S state, IteratorAsync<K<M, A>> iter), Reduced<S>>(reduced));

        K<M, IteratorAsync<K<M, A>>> tail(IteratorAsync<K<M, A>> iter) =>
            M.LiftIO(IO.liftVAsync(_ => iter.Tail).Map(i => i.Split()));

        K<M, Next<(S state, IteratorAsync<K<M, A>> iter), Reduced<S>>> next(S state, K<M, IteratorAsync<K<M, A>>> tail) =>
            tail.Map(t => Next.Loop<(S state, IteratorAsync<K<M, A>> iter), Reduced<S>>((state, t)));
    }
}
