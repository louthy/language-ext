using System;
using LanguageExt.Traits;

namespace LanguageExt;


public class TransduceTo<OUT> : Cofunctor<TransduceTo<OUT>>
{
    static K<TransduceTo<OUT>, A> Cofunctor<TransduceTo<OUT>>.
        Comap<A, B>(Func<A, B> f, K<TransduceTo<OUT>, B> fb) =>
        Transducer.compose(Transducer.map(f), fb.As());
}
