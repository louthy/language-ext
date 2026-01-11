using System;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

record FoldableSourceT<F, M, A>(K<F, K<M, A>> Items) : SourceT<M, A>
    where M : MonadIO<M>
    where F : IterableK<F>
{
    internal override K<M, Reduced<S>> ReduceInternalM<S>(S state, ReducerM<M, K<M, A>, S> reducer)
    {
        return from i in steps()
               from r in Monad.recur((Iter: i, State: state), go)
               from _ in release(i)
               select r;

        IO<Iterator<K<M, A>>> steps() =>
            use(() => Items.ForwardIterator().Using());

        K<M, Next<(Iterator<K<M, A>> Iter, S State), Reduced<S>>> go((Iterator<K<M, A>> Iter, S State) step) =>
            IO.token >> (t => t.IsCancellationRequested
                                  ? done(state)
                                  : step.Iter is (Exist<A> (var head), var tail)
                                      ? reducer(step.State, M.Pure(head)) *
                                        (ns => ns.Continue
                                                   ? next(tail, ns.Value)
                                                   : reduced(ns))
                                      : done(step.State));

        K<M, Next<(Iterator<K<M, A>> Iter, S State), Reduced<S>>> done(S state) =>
            M.Pure(reduced(Reduced.Done(state)));

        Next<(Iterator<K<M, A>> Iter, S State), Reduced<S>> reduced(Reduced<S> reduced) =>
            Next.Done<(Iterator<K<M, A>> Iter, S State), Reduced<S>>(reduced);

        Next<(Iterator<K<M, A>> Iter, S State), Reduced<S>> next(Iterator<K<M, A>> tail, S state) =>
            Next.Loop<(Iterator<K<M, A>> Iter, S State), Reduced<S>>((tail, state));        
    }        
}
