using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

record IteratorAsyncSourceT<M, A>(IteratorIO<K<M, A>> Items) : SourceT<M, A>
    where M : MonadIO<M>
{
    internal override K<M, Reduced<S>> ReduceInternalM<S>(S state, ReducerM<M, K<M, A>, S> reducer)
    {
        return from xs in use(() => Items.Using())
               from rs in Monad.recur((state, xs), go) >>
                          release(xs)
               select rs;

        K<M, Next<(S state, IteratorIO<K<M, A>> iter), Reduced<S>>> go((S state, IteratorIO<K<M, A>> iter) self) =>
            isDone() >> (flag => flag ? done(self.state)
                                      : self.iter.NextIO() >>
                                        (n => n is (Exist<K<M, A>>(var head), var tail)
                                                  ? reducer(state, head) >> (ns => ns.Continue
                                                                                 ? next(ns.Value, tail)
                                                                                 : reduced(ns))
                                                  : done(self.state)));

        IO<bool> isDone() =>
            IO.lift(e => e.Token.IsCancellationRequested);

        K<M, Next<(S state, IteratorIO<K<M, A>> iter), Reduced<S>>> done(S state) =>
            reduced(Reduced.Done(state));

        K<M, Next<(S state, IteratorIO<K<M, A>> iter), Reduced<S>>> reduced(Reduced<S> reduced) =>
            M.Pure(Next.Done<(S state, IteratorIO<K<M, A>> iter), Reduced<S>>(reduced));


        K<M, Next<(S state, IteratorIO<K<M, A>> iter), Reduced<S>>> next(S state, IteratorIO<K<M, A>> tail) =>
            M.Pure(Next.Loop<(S state, IteratorIO<K<M, A>> iter), Reduced<S>>((state, tail)));
    }
}
