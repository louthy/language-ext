using LanguageExt.Traits;

namespace LanguageExt;

record FoldablePureSourceT<F, M, A>(K<F, A> Items) : SourceT<M, A>
    where M : MonadIO<M>
    where F : Foldable<F>
{
    internal override K<M, Reduced<S>> ReduceInternalM<S>(S state, ReducerM<M, K<M, A>, S> reducer)
    {
        return steps() >> (f => Monad.recur(f, go));

        IO<Fold<A, S>> steps() =>
            // Needs to remain lazy
            IO.lift(e => Items.FoldStep(state));
        
        K<M, Next<Fold<A, S>, Reduced<S>>> go(Fold<A, S> fold) =>
            IO.token.Bind(t => t.IsCancellationRequested
                                   ? done(state)
                                   : fold switch
                                     {
                                         Fold<A, S>.Done(var s) =>
                                             done(s),

                                         Fold<A, S>.Loop(var s, var a, var n) =>
                                             reducer(s, M.Pure(a)) *
                                             (ns => ns.Continue
                                                        ? next(n(ns.Value))
                                                        : reduced(ns))
                                     });

        K<M, Next<Fold<A, S>, Reduced<S>>> done(S state) =>
            M.Pure(reduced(Reduced.Done(state)));

        Next<Fold<A, S>, Reduced<S>> reduced(Reduced<S> reduced) =>
            Next.Done<Fold<A, S>, Reduced<S>>(reduced);

        Next<Fold<A, S>, Reduced<S>> next(Fold<A, S> tail) =>
            Next.Loop<Fold<A, S>, Reduced<S>>(tail);
    }
}
