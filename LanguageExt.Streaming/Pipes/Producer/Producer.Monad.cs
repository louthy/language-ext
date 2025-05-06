using System;
using System.Collections.Generic;
using System.Linq;
using LanguageExt.Async.Linq;
using LanguageExt.Traits;

namespace LanguageExt.Pipes;

public class Producer<RT, OUT> : 
    MonadT<Producer<RT, OUT>, Eff<RT>>,
    MonadUnliftIO<Producer<RT, OUT>>
{
    static K<Producer<RT, OUT>, B> Monad<Producer<RT, OUT>>.Bind<A, B>(
        K<Producer<RT, OUT>, A> ma, 
        Func<A, K<Producer<RT, OUT>, B>> f) => 
        ma.As().Bind(x => f(x).As());

    static K<Producer<RT, OUT>, B> Functor<Producer<RT, OUT>>.Map<A, B>(
        Func<A, B> f, 
        K<Producer<RT, OUT>, A> ma) => 
        ma.As().Map(f);

    static K<Producer<RT, OUT>, A> Applicative<Producer<RT, OUT>>.Pure<A>(A value) => 
        Producer.pure<RT, OUT, A>(value);

    static K<Producer<RT, OUT>, B> Applicative<Producer<RT, OUT>>.Apply<A, B>(
        K<Producer<RT, OUT>, Func<A, B>> mf,
        K<Producer<RT, OUT>, A> ma) =>
        ma.As().ApplyBack(mf.As());

    static K<Producer<RT, OUT>, A> MonadT<Producer<RT, OUT>, Eff<RT>>.Lift<A>(K<Eff<RT>, A> ma) => 
        Producer.liftM<RT, OUT, A>(ma);

    static K<Producer<RT, OUT>, A> Maybe.MonadIO<Producer<RT, OUT>>.LiftIO<A>(IO<A> ma) => 
        Producer.liftIO<RT, OUT, A>(ma);

    static K<Producer<RT, OUT>, B> Maybe.MonadUnliftIO<Producer<RT, OUT>>.MapIO<A, B>(K<Producer<RT, OUT>, A> ma, Func<IO<A>, IO<B>> f) => 
        ma.As().MapIO(f);

    static K<Producer<RT, OUT>, IO<A>> Maybe.MonadUnliftIO<Producer<RT, OUT>>.ToIO<A>(K<Producer<RT, OUT>, A> ma) => 
        ma.MapIO(IO.pure);

    static K<Producer<RT, OUT>, B> Applicative<Producer<RT, OUT>>.Action<A, B>(
        K<Producer<RT, OUT>, A> ma, 
        K<Producer<RT, OUT>, B> mb) =>
        Producer.liftM<RT, OUT, B>(ma.As().Run().Action(mb.As().Run()));

    static K<Producer<RT, OUT>, A> Applicative<Producer<RT, OUT>>.Actions<A>(
        IEnumerable<K<Producer<RT, OUT>, A>> fas) =>
        fas.Select(fa => fa.As().Proxy)
           .Actions()
           .ToProducer();

    static K<Producer<RT, OUT>, A> Applicative<Producer<RT, OUT>>.Actions<A>(
        IAsyncEnumerable<K<Producer<RT, OUT>, A>> fas) =>
        fas.Select(fa => fa.As().Proxy)
           .Actions()
           .ToProducer();
}
