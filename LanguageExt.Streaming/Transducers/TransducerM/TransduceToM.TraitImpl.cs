using System;
using LanguageExt.Traits;

namespace LanguageExt;


public class TransduceToM<M, OUT> : Cofunctor<TransduceToM<M, OUT>>
{
    static K<TransduceToM<M, OUT>, A> Cofunctor<TransduceToM<M, OUT>>.
        Comap<A, B>(Func<A, B> f, K<TransduceToM<M, OUT>, B> fb) =>
        TransducerM.map<M, A, B>(f).Compose(fb.As());
}
