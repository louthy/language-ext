using System;
using System.Collections.Generic;
using System.Linq;
using LanguageExt.Async.Linq;
using LanguageExt.Traits;

namespace LanguageExt.Pipes;

public class ConsumerT<IN, M> : 
    MonadT<ConsumerT<IN, M>, M>,
    MonadUnliftIO<ConsumerT<IN, M>>
    where M : MonadIO<M>
{
    static K<ConsumerT<IN, M>, B> Monad<ConsumerT<IN, M>>.Bind<A, B>(
        K<ConsumerT<IN, M>, A> ma, 
        Func<A, K<ConsumerT<IN, M>, B>> f) => 
        ma.As().Bind(x => f(x).As());

    static K<ConsumerT<IN, M>, B> Functor<ConsumerT<IN, M>>.Map<A, B>(
        Func<A, B> f, 
        K<ConsumerT<IN, M>, A> ma) => 
        ma.As().Map(f);

    static K<ConsumerT<IN, M>, A> Applicative<ConsumerT<IN, M>>.Pure<A>(A value) => 
        ConsumerT.pure<IN, M, A>(value);

    static K<ConsumerT<IN, M>, B> Applicative<ConsumerT<IN, M>>.Apply<A, B>(
        K<ConsumerT<IN, M>, Func<A, B>> mf,
        K<ConsumerT<IN, M>, A> ma) =>
        ma.As().ApplyBack(mf.As());

    static K<ConsumerT<IN, M>, B> Applicative<ConsumerT<IN, M>>.Apply<A, B>(
        K<ConsumerT<IN, M>, Func<A, B>> mf,
        Memo<ConsumerT<IN, M>, A> ma) =>
        new PipeTMemo<IN, Void, M, A>(ma.Lower().Map(ma => ma.As().Proxy.Kind()).Lift())
           .ApplyBack(mf.As().Proxy)
           .ToConsumer();

    static K<ConsumerT<IN, M>, A> MonadT<ConsumerT<IN, M>, M>.Lift<A>(K<M, A> ma) =>
        ConsumerT.liftM<IN, M, A>(ma);

    static K<ConsumerT<IN, M>, A> MonadIO<ConsumerT<IN, M>>.LiftIO<A>(IO<A> ma) => 
        ConsumerT.liftIO<IN, M, A>(ma); 

    static K<ConsumerT<IN, M>, B> MonadUnliftIO<ConsumerT<IN, M>>.MapIO<A, B>(K<ConsumerT<IN, M>, A> ma, Func<IO<A>, IO<B>> f) => 
        ma.As().MapM(m => M.MapIOMaybe(m, f));

    static K<ConsumerT<IN, M>, IO<A>> MonadUnliftIO<ConsumerT<IN, M>>.ToIO<A>(K<ConsumerT<IN, M>, A> ma) => 
        ma.As().MapM(M.ToIOMaybe);

    static K<ConsumerT<IN, M>, B> Applicative<ConsumerT<IN, M>>.Action<A, B>(
        K<ConsumerT<IN, M>, A> ma, 
        K<ConsumerT<IN, M>, B> mb) =>
        ConsumerT.liftM<IN, M, B>(ma.As().Run().Action(mb.As().Run()));

    static K<ConsumerT<IN, M>, A> Applicative<ConsumerT<IN, M>>.Actions<A>(
        IEnumerable<K<ConsumerT<IN, M>, A>> fas) =>
        fas.Select(fa => fa.As().Proxy)
           .Actions()
           .ToConsumer();

    static K<ConsumerT<IN, M>, A> Applicative<ConsumerT<IN, M>>.Actions<A>(
        IAsyncEnumerable<K<ConsumerT<IN, M>, A>> fas) =>
        fas.Select(fa => fa.As().Proxy)
           .Actions()
           .ToConsumer();
}
