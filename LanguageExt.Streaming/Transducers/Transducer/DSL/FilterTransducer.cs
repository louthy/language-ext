using System;
using LanguageExt.Traits;

namespace LanguageExt;

record FilterTransducer<A>(Func<A, bool> Predicate) : Transducer<A, A>
{
    public override ReducerAsync<A, S> Reduce<S>(ReducerAsync<A, S> reducer) =>
        (s, x) => Predicate(x)
                      ? reducer(s, x)
                      : Reduced.ContinueAsync(s);
}
