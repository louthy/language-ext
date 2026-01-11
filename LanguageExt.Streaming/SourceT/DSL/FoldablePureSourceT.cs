using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

record FoldablePureSourceT<F, M, A>(K<F, A> Items) : SourceT<M, A>
    where M : MonadIO<M>
    where F : IterableK<F>
{
    internal override K<M, Reduced<S>> ReduceInternalM<S>(S state, ReducerM<M, K<M, A>, S> reducer)
    {
        return from i in steps()
               from r in Monad.recur((Iter: i, State: state), go)
               from _ in release(i)
               select r;

        IO<Iterator<A>> steps() =>
            use(() => Items.ForwardIterator().Using());

        K<M, Next<(Iterator<A> Iter, S State), Reduced<S>>> go((Iterator<A> Iter, S State) step) =>
            IO.token >> (t => t.IsCancellationRequested
                                  ? done(state)
                                  : step.Iter is (Exist<A> (var head), var tail)
                                      ? reducer(step.State, M.Pure(head)) *
                                        (ns => ns.Continue
                                                   ? next(tail, ns.Value)
                                                   : reduced(ns))
                                      : done(step.State));

        K<M, Next<(Iterator<A> Iter, S State), Reduced<S>>> done(S state) =>
            M.Pure(reduced(Reduced.Done(state)));

        Next<(Iterator<A> Iter, S State), Reduced<S>> reduced(Reduced<S> reduced) =>
            Next.Done<(Iterator<A> Iter, S State), Reduced<S>>(reduced);

        Next<(Iterator<A> Iter, S State), Reduced<S>> next(Iterator<A> iter, S state) =>
            Next.Loop<(Iterator<A> Iter, S State), Reduced<S>>((iter, state));
    }
}
