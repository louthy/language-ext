using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

record IteratorSyncSourceT<M, A>(Iterator<K<M, A>> Items) : SourceT<M, A>
    where M : MonadIO<M>
{
    internal override K<M, Reduced<S>> ReduceInternalM<S>(S state, ReducerM<M, K<M, A>, S> reducer)
    {
        return from i in steps()
               from r in Monad.recur((Iter: i, State: state), go)
               from _ in release(i)
               select r;

        IO<Iterator<K<M, A>>> steps() =>
            use(() => Items.Using());

        K<M, Next<(Iterator<K<M, A>> iter, S state), Reduced<S>>> go((Iterator<K<M, A>> iter, S state) self) =>
            isCancel() >> (c => c ? done(self.state)
                                  : self.iter is (Exist<K<M, A>>(var head), var tail)
                                        ? reducer(state, head) >> (ns => ns.Continue
                                                                             ? next(tail, ns.Value)
                                                                             : reduced(ns))
                                        : done(self.state));

        static IO<bool> isCancel() =>
            IO.lift(e => e.Token.IsCancellationRequested);

        K<M, Next<(Iterator<K<M, A>> iter, S state), Reduced<S>>> done(S state) =>
            reduced(Reduced.Done(state));

        K<M, Next<(Iterator<K<M, A>> iter, S state), Reduced<S>>> reduced(Reduced<S> reduced) =>
            M.Pure(Next.Done<(Iterator<K<M, A>> iter, S state), Reduced<S>>(reduced));

        K<M, Next<(Iterator<K<M, A>> iter, S state), Reduced<S>>> next(Iterator<K<M, A>> tail, S state) =>
            M.Pure(Next.Loop<(Iterator<K<M, A>> iter, S state), Reduced<S>>((tail, state)));
    }
}
