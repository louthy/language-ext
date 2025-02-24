using System;

namespace LanguageExt;

record FilterTransducer<A>(Func<A, bool> Predicate) : Transducer<A, A> 
{
    public override Reducer<A, S> Reduce<S>(Reducer<A, S> reducer) =>
        (s, x) => Predicate(x) 
                      ? reducer(s, x) 
                      : s;
}
