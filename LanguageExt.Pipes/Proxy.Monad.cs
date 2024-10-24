using System;
using System.Collections.Generic;
using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt.Pipes;

/// <summary>
/// Monad transformer trait implementation for `Proxy`
/// </summary>
public class Proxy<UOut, UIn, DIn, DOut, M> : 
    MonadT<Proxy<UOut, UIn, DIn, DOut, M>, M>
    where M : Monad<M>
{
    static K<Proxy<UOut, UIn, DIn, DOut, M>, B> Monad<Proxy<UOut, UIn, DIn, DOut, M>>.Bind<A, B>(
        K<Proxy<UOut, UIn, DIn, DOut, M>, A> ma, 
        Func<A, K<Proxy<UOut, UIn, DIn, DOut, M>, B>> f) => 
        ma.As().Bind(x => f(x).As());

    static K<Proxy<UOut, UIn, DIn, DOut, M>, B> Functor<Proxy<UOut, UIn, DIn, DOut, M>>.Map<A, B>(
        Func<A, B> f, K<Proxy<UOut, UIn, DIn, DOut, M>, A> ma) => 
        ma.As().Map(f);

    static K<Proxy<UOut, UIn, DIn, DOut, M>, A> Applicative<Proxy<UOut, UIn, DIn, DOut, M>>.Pure<A>(A value) => 
        Proxy.Pure<UOut, UIn, DIn, DOut, M, A>(value);

    static K<Proxy<UOut, UIn, DIn, DOut, M>, B> Applicative<Proxy<UOut, UIn, DIn, DOut, M>>.Apply<A, B>(
        K<Proxy<UOut, UIn, DIn, DOut, M>, Func<A, B>> mf, 
        K<Proxy<UOut, UIn, DIn, DOut, M>, A> ma) => 
        mf.As().Apply(ma.As());

    static K<Proxy<UOut, UIn, DIn, DOut, M>, A> MonadT<Proxy<UOut, UIn, DIn, DOut, M>, M>.Lift<A>(K<M, A> ma) =>
        Proxy.lift<UOut, UIn, DIn, DOut, M, A>(ma);

    static K<Proxy<UOut, UIn, DIn, DOut, M>, A> MonadIO<Proxy<UOut, UIn, DIn, DOut, M>>.LiftIO<A>(IO<A> ma) => 
        Proxy.lift<UOut, UIn, DIn, DOut, M, A>(M.LiftIO(ma));

    static K<Proxy<UOut, UIn, DIn, DOut, M>, IO<A>> MonadIO<Proxy<UOut, UIn, DIn, DOut, M>>.ToIO<A>(K<Proxy<UOut, UIn, DIn, DOut, M>, A> ma) =>
        ma.As().ToIO();

    static K<Proxy<UOut, UIn, DIn, DOut, M>, A> Applicative<Proxy<UOut, UIn, DIn, DOut, M>>.Actions<A>(
        IEnumerable<K<Proxy<UOut, UIn, DIn, DOut, M>, A>> fas) =>
        fas.AsIterable().Map(fa => fa.ToIO()).Actions().Bind(io => io);
}
