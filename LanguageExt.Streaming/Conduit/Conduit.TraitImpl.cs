using System;
using LanguageExt.Traits;

namespace LanguageExt;

public class Conduit<A> : Cofunctor<Conduit<A>>, Functor<Conduit<A>>
{
    public static K<Conduit<A>, X> Comap<X, B>(Func<X, B> f, K<Conduit<A>, B> fb) => 
        fb.As().Comap(f);

    public static K<Conduit<A>, C> Map<B, C>(Func<B, C> f, K<Conduit<A>, B> ma) =>
        ma.As().Map(f);
}
