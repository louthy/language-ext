using System;
using LanguageExt.Traits;

namespace LanguageExt;

record FilterTransducerM<M, A>(Func<A, bool> Predicate) : TransducerM<M, A, A>
    where M : Applicative<M>
{
    public override ReducerM<M, A, S> Reduce<S>(ReducerM<M, A, S> reducer) => 
        (s, x) => Predicate(x)
                      ? reducer(s, x)
                      : M.Pure(Reduced.Continue(s));
}
