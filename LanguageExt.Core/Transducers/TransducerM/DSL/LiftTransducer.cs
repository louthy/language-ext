using System;
using LanguageExt.Traits;

namespace LanguageExt;

/*
record LiftTransducerM<M, A, B>(Transducer<A, B> F) : TransducerM<M, A, B>
    where M : Monad<M>, Alternative<M>
{
    public override ReducerM<M, A, S> Reduce<S>(ReducerM<M, B, S> reducer) =>
        (s, x) => F.Reduce<K<M, S>>((ms, x1) => Reduced.ContinueAsync(ms.Bind<M, S, S>(s1 => reducer(s1, x1))))(M.Pure(s), x);
        //throw new NotImplementedException("TODO");
        //F.Reduce(reducer);
}
*/
