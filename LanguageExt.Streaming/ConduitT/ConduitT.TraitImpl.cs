using System;
using LanguageExt.Traits;

namespace LanguageExt;

public class ConduitT<M, A> : Cofunctor<ConduitT<M, A>>, Functor<ConduitT<M, A>>
    where M : MonadIO<M>, Monad<M>, Alternative<M>
{
    public static K<ConduitT<M, A>, X> Comap<X, B>(Func<X, B> f, K<ConduitT<M, A>, B> fb) => 
        fb.As().Comap(f);

    public static K<ConduitT<M, A>, C> Map<B, C>(Func<B, C> f, K<ConduitT<M, A>, B> ma) =>
        ma.As().Map(f);
}
