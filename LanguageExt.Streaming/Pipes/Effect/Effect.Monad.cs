using System;
using System.Collections.Generic;
using System.Linq;
using LanguageExt.Async.Linq;
using LanguageExt.Traits;

namespace LanguageExt.Pipes;

public class Effect<RT> : 
    MonadT<Effect<RT>, Eff<RT>>,
    MonadUnliftIO<Effect<RT>>
{
    static K<Effect<RT>, B> Monad<Effect<RT>>.Bind<A, B>(K<Effect<RT>, A> ma, Func<A, K<Effect<RT>, B>> f) => 
        ma.As().Bind(x => f(x).As());

    static K<Effect<RT>, B> Functor<Effect<RT>>.Map<A, B>(Func<A, B> f, K<Effect<RT>, A> ma) => 
        ma.As().Map(f);

    static K<Effect<RT>, A> Applicative<Effect<RT>>.Pure<A>(A value) => 
        Effect.pure<RT, A>(value);

    static K<Effect<RT>, B> Applicative<Effect<RT>>.Apply<A, B>(K<Effect<RT>, Func<A, B>> mf, K<Effect<RT>, A> ma) => 
        ma.As().ApplyBack(mf.As());

    static K<Effect<RT>, B> Applicative<Effect<RT>>.Apply<A, B>(K<Effect<RT>, Func<A, B>> mf, Memo<Effect<RT>, A> ma) => 
        new PipeTMemo<Unit, Void, Eff<RT>, A>(ma.Lower().Map(ma => ma.As().Proxy.Kind()).Lift())
            .ApplyBack(mf.As().Proxy)
            .ToEffect();

    static K<Effect<RT>, A> MonadT<Effect<RT>, Eff<RT>>.Lift<A>(K<Eff<RT>, A> ma) =>
        Effect.liftM(ma);

    static K<Effect<RT>, A> MonadIO<Effect<RT>>.LiftIO<A>(IO<A> ma) => 
        Effect.liftIO<RT, A>(ma);

    static K<Effect<RT>, B> MonadUnliftIO<Effect<RT>>.MapIO<A, B>(K<Effect<RT>, A> ma, Func<IO<A>, IO<B>> f) => 
        ma.As().MapIO(f);

    static K<Effect<RT>, IO<A>> MonadUnliftIO<Effect<RT>>.ToIO<A>(K<Effect<RT>, A> ma) =>
        ma.MapIO(IO.pure);

    static K<Effect<RT>, ForkIO<A>> MonadUnliftIO<Effect<RT>>.ForkIO<A>(
        K<Effect<RT>, A> ma,
        Option<TimeSpan> timeout) =>
        Effect.liftM(ma.As().Run().ForkIO(timeout));
    
    static K<Effect<RT>, B> Applicative<Effect<RT>>.Action<A, B>(
        K<Effect<RT>, A> ma, 
        K<Effect<RT>, B> mb) =>
        Effect.liftM(ma.As().Run().Action(mb.As().Run()));

    static K<Effect<RT>, A> Applicative<Effect<RT>>.Actions<A>(
        IEnumerable<K<Effect<RT>, A>> fas) =>
        fas.Select(fa => fa.As().Proxy)
           .Actions()
           .ToEffect();

    static K<Effect<RT>, A> Applicative<Effect<RT>>.Actions<A>(
        IAsyncEnumerable<K<Effect<RT>, A>> fas) =>
        fas.Select(fa => fa.As().Proxy)
           .Actions()
           .ToEffect();
}
