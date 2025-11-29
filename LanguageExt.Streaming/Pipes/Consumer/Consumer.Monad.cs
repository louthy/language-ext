using System;
using System.Collections.Generic;
using System.Linq;
using LanguageExt.Traits;

namespace LanguageExt.Pipes;

public class Consumer<RT, IN> : 
    MonadUnliftIO<Consumer<RT, IN>>,
    MonadT<Consumer<RT, IN>, Eff<RT>>
{
    static K<Consumer<RT, IN>, B> Monad<Consumer<RT, IN>>.Bind<A, B>(
        K<Consumer<RT, IN>, A> ma, 
        Func<A, K<Consumer<RT, IN>, B>> f) => 
        ma.As().Bind(x => f(x).As());

    static K<Consumer<RT, IN>, B> Functor<Consumer<RT, IN>>.Map<A, B>(
        Func<A, B> f, 
        K<Consumer<RT, IN>, A> ma) => 
        ma.As().Map(f);

    static K<Consumer<RT, IN>, A> Applicative<Consumer<RT, IN>>.Pure<A>(A value) => 
        Consumer.pure<RT, IN, A>(value);

    static K<Consumer<RT, IN>, B> Applicative<Consumer<RT, IN>>.Apply<A, B>(
        K<Consumer<RT, IN>, Func<A, B>> mf,
        K<Consumer<RT, IN>, A> ma) =>
        ma.As().ApplyBack(mf.As());

    static K<Consumer<RT, IN>, B> Applicative<Consumer<RT, IN>>.Apply<A, B>(
        K<Consumer<RT, IN>, Func<A, B>> mf,
        Memo<Consumer<RT, IN>, A> mma) =>
        new PipeTMemo<IN, Void, Eff<RT>, A>(mma.Lower().Map(ma => ma.As().Proxy.Kind()).Lift())
           .ApplyBack(mf.As().Proxy)
           .ToConsumer();

    static K<Consumer<RT, IN>, A> MonadT<Consumer<RT, IN>, Eff<RT>>.Lift<A>(K<Eff<RT>, A> ma) =>
        Consumer.liftM<RT, IN, A>(ma);

    static K<Consumer<RT, IN>, A> MonadIO<Consumer<RT, IN>>.LiftIO<A>(IO<A> ma) => 
        Consumer.liftIO<RT, IN, A>(ma); 

    static K<Consumer<RT, IN>, B> MonadUnliftIO<Consumer<RT, IN>>.MapIO<A, B>(K<Consumer<RT, IN>, A> ma, Func<IO<A>, IO<B>> f) => 
        ma.As().MapIO(f);

    static K<Consumer<RT, IN>, IO<A>> MonadUnliftIO<Consumer<RT, IN>>.ToIO<A>(K<Consumer<RT, IN>, A> ma) => 
        ma.As().MapIO(IO.pure);

    static K<Consumer<RT, IN>, B> Applicative<Consumer<RT, IN>>.Action<A, B>(
        K<Consumer<RT, IN>, A> ma, 
        K<Consumer<RT, IN>, B> mb) =>
        Consumer.liftM<RT, IN, B>(ma.As().Run().Action(mb.As().Run()));

    static K<Consumer<RT, IN>, A> Applicative<Consumer<RT, IN>>.Actions<A>(
        IEnumerable<K<Consumer<RT, IN>, A>> fas) =>
        fas.Select(fa => fa.As().Proxy)
           .Actions()
           .ToConsumer();

    static K<Consumer<RT, IN>, A> Applicative<Consumer<RT, IN>>.Actions<A>(
        IAsyncEnumerable<K<Consumer<RT, IN>, A>> fas) =>
        fas.Select(fa => fa.As().Proxy)
           .Actions()
           .ToConsumer();
}
