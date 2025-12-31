using System.Collections.Generic;
using LanguageExt.Traits;

namespace LanguageExt;

record IteratorSyncSourceT<M, A>(IEnumerable<K<M, A>> Items) : SourceT<M, A>
    where M : MonadIO<M>
{
    internal override K<M, Reduced<S>> ReduceInternalM<S>(S state, ReducerM<M, K<M, A>, S> reducer)
    {
        return Monad.recur((state, Items.GetIterator()), go);

        K<M, Next<(S state, Iterator<K<M, A>> iter), Reduced<S>>> go((S state, Iterator<K<M, A>> iter) self) =>
            isDone(self) >> (d => d ? done(self.state)
                                    : reducer(state, self.iter.Head) >>
                                      (ns => ns.Continue
                                                 ? next(ns.Value, self.iter.Tail)
                                                 : reduced(ns)));

        IO<bool> isDone((S state, Iterator<K<M, A>> iter) self) =>
            IO.lift(e => e.Token.IsCancellationRequested || self.iter.IsEmpty);

        K<M, Next<(S state, Iterator<K<M, A>> iter), Reduced<S>>> done(S state) =>
            reduced(Reduced.Done(state));

        K<M, Next<(S state, Iterator<K<M, A>> iter), Reduced<S>>> reduced(Reduced<S> reduced) =>
            M.Pure(Next.Done<(S state, Iterator<K<M, A>> iter), Reduced<S>>(reduced));

        K<M, Next<(S state, Iterator<K<M, A>> iter), Reduced<S>>> next(S state, Iterator<K<M, A>> tail) =>
            M.Pure(Next.Loop<(S state, Iterator<K<M, A>> iter), Reduced<S>>((state, tail.Split())));
    }
}
