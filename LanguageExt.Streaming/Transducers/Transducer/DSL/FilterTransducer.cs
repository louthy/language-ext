using System;
using LanguageExt.Traits;

namespace LanguageExt;

record FilterTransducer<A>(Func<A, bool> Predicate) : Transducer<A, A>
{
    public override ReducerIO<A, S> Reduce<S>(ReducerIO<A, S> reducer) =>
        (s, x) =>
            IO.liftVAsync(async e => !e.Token.IsCancellationRequested && Predicate(x)
                                         ? await reducer(s, x).RunAsync(e.Token)
                                         : Reduced.Continue(s));
    
    public override TransducerM<M, A, A> Lift<M>() => 
        new FilterTransducerM<M, A>(Predicate);
}
