using System;
using LanguageExt.Traits;

namespace LanguageExt;

record FoldableSourceT<F, M, A>(K<F, K<M, A>> Items) : SourceT<M, A>
    where M : MonadIO<M>
    where F : Foldable<F>
{
    internal override K<M, Reduced<S>> ReduceInternalM<S>(S state, ReducerM<M, K<M, A>, S> reducer)
    {
        return steps() >> (f => Monad.recur(f, go));

        IO<Fold<K<M, A>, S>> steps() =>
            // Needs to remain lazy
            IO.lift(e => Items.FoldStep(state));

        K<M, Next<Fold<K<M, A>, S>, Reduced<S>>> go(Fold<K<M, A>, S> fold) =>
            IO.token.Bind(t => t.IsCancellationRequested
                                   ? done(state)
                                   : fold switch
                                     {
                                         Fold<K<M, A>, S>.Done(var s) =>
                                             done(s),

                                         Fold<K<M, A>, S>.Loop(var s, var ma, var n) =>
                                             reducer(s, ma) *
                                             (ns => ns.Continue
                                                        ? next(n(ns.Value))
                                                        : reduced(ns)),
                                         
                                         _ => throw new NotSupportedException()
                                     });

        K<M, Next<Fold<K<M, A>, S>, Reduced<S>>> done(S state) =>
            M.Pure(reduced(Reduced.Done(state)));

        Next<Fold<K<M, A>, S>, Reduced<S>> reduced(Reduced<S> reduced) =>
            Next.Done<Fold<K<M, A>, S>, Reduced<S>>(reduced);

        Next<Fold<K<M, A>, S>, Reduced<S>> next(Fold<K<M, A>, S> tail) =>
            Next.Loop<Fold<K<M, A>, S>, Reduced<S>>(tail);        
    }        
}
