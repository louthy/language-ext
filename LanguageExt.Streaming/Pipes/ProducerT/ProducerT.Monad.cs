using System;
using System.Collections.Generic;
using System.Linq;
using LanguageExt.Async.Linq;
using LanguageExt.Traits;

namespace LanguageExt.Pipes;

public class ProducerT<OUT, M> : 
    MonadT<ProducerT<OUT, M>, M>,
    MonadUnliftIO<ProducerT<OUT, M>>
    where M : MonadIO<M>
{
    static K<ProducerT<OUT, M>, B> Monad<ProducerT<OUT, M>>.Bind<A, B>(
        K<ProducerT<OUT, M>, A> ma, 
        Func<A, K<ProducerT<OUT, M>, B>> f) => 
        ma.As().Bind(x => f(x).As());

    static K<ProducerT<OUT, M>, B> Monad<ProducerT<OUT, M>>.Recur<A, B>(A value, Func<A, K<ProducerT<OUT, M>, Next<A, B>>> f) =>
        Monad.unsafeRecur(value, f);

    static K<ProducerT<OUT, M>, B> Functor<ProducerT<OUT, M>>.Map<A, B>(
        Func<A, B> f, 
        K<ProducerT<OUT, M>, A> ma) => 
        ma.As().Map(f);

    static K<ProducerT<OUT, M>, A> Applicative<ProducerT<OUT, M>>.Pure<A>(A value) => 
        ProducerT.pure<OUT, M, A>(value);

    static K<ProducerT<OUT, M>, B> Applicative<ProducerT<OUT, M>>.Apply<A, B>(
        K<ProducerT<OUT, M>, Func<A, B>> mf,
        K<ProducerT<OUT, M>, A> ma) =>
        ma.As().ApplyBack(mf.As());
    
    static K<ProducerT<OUT, M>, B> Applicative<ProducerT<OUT, M>>.Apply<A, B>(
        K<ProducerT<OUT, M>, Func<A, B>> mf,
        Memo<ProducerT<OUT, M>, A> ma) =>
        new PipeTMemo<Unit, OUT, M, A>(ma.Lower().Map(ma => ma.As().Proxy.Kind()).Lift())
           .ApplyBack(mf.As().Proxy)
           .ToProducer();

    static K<ProducerT<OUT, M>, A> MonadT<ProducerT<OUT, M>, M>.Lift<A>(K<M, A> ma) => 
        ProducerT.liftM<OUT, M, A>(ma);

    static K<ProducerT<OUT, M>, A> MonadIO<ProducerT<OUT, M>>.LiftIO<A>(IO<A> ma) => 
        ProducerT.liftIO<OUT, M, A>(ma);

    static K<ProducerT<OUT, M>, B> MonadUnliftIO<ProducerT<OUT, M>>.MapIO<A, B>(K<ProducerT<OUT, M>, A> ma, Func<IO<A>, IO<B>> f) => 
        ma.As().MapM(m => M.MapIOMaybe(m, f));

    static K<ProducerT<OUT, M>, IO<A>> MonadUnliftIO<ProducerT<OUT, M>>.ToIO<A>(K<ProducerT<OUT, M>, A> ma) => 
        ma.As().MapM(M.ToIOMaybe);

    static K<ProducerT<OUT, M>, B> Applicative<ProducerT<OUT, M>>.Action<A, B>(
        K<ProducerT<OUT, M>, A> ma, 
        K<ProducerT<OUT, M>, B> mb) =>
        ProducerT.liftM<OUT, M, B>(ma.As().Run().Action(mb.As().Run()));

    static K<ProducerT<OUT, M>, A> Applicative<ProducerT<OUT, M>>.Actions<A>(
        IterableNE<K<ProducerT<OUT, M>, A>> fas) =>
        fas.Select(fa => fa.As().Proxy.Kind())
           .Actions()
           .ToProducer();
}
