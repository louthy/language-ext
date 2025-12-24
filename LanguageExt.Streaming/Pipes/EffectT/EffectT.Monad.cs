using System;
using System.Collections.Generic;
using System.Linq;
using LanguageExt.Async.Linq;
using LanguageExt.Traits;

namespace LanguageExt.Pipes;

public class EffectT<M> :
    MonadT<EffectT<M>, M>,
    MonadUnliftIO<EffectT<M>>
    where M : MonadIO<M>
{
    static K<EffectT<M>, B> Monad<EffectT<M>>.Bind<A, B>(K<EffectT<M>, A> ma, Func<A, K<EffectT<M>, B>> f) =>
        ma.As().Bind(x => f(x).As());

    static K<EffectT<M>, B> Monad<EffectT<M>>.Recur<A, B>(A value, Func<A, K<EffectT<M>, Next<A, B>>> f) => 
        Monad.unsafeRecur(value, f);

    static K<EffectT<M>, B> Functor<EffectT<M>>.Map<A, B>(Func<A, B> f, K<EffectT<M>, A> ma) =>
        ma.As().Map(f);

    static K<EffectT<M>, A> Applicative<EffectT<M>>.Pure<A>(A value) =>
        EffectT.pure<M, A>(value);

    static K<EffectT<M>, B> Applicative<EffectT<M>>.Apply<A, B>(K<EffectT<M>, Func<A, B>> mf, K<EffectT<M>, A> ma) =>
        ma.As().ApplyBack(mf.As());

    static K<EffectT<M>, B> Applicative<EffectT<M>>.Apply<A, B>(K<EffectT<M>, Func<A, B>> mf, Memo<EffectT<M>, A> ma) =>
        new PipeTMemo<Unit, Void, M, A>(ma.Lower().Map(ma => ma.As().Proxy.Kind()).Lift())
           .ApplyBack(mf.As().Proxy)
           .ToEffect();
    
    static K<EffectT<M>, A> MonadT<EffectT<M>, M>.Lift<A>(K<M, A> ma) =>
        EffectT.liftM(ma);

    static K<EffectT<M>, A> MonadIO<EffectT<M>>.LiftIO<A>(IO<A> ma) =>
        EffectT.liftIO<M, A>(ma);

    static K<EffectT<M>, B> MonadUnliftIO<EffectT<M>>.MapIO<A, B>(K<EffectT<M>, A> ma, Func<IO<A>, IO<B>> f) =>
        ma.As().MapM(m => M.MapIOMaybe(m, f));

    static K<EffectT<M>, IO<A>> MonadUnliftIO<EffectT<M>>.ToIO<A>(K<EffectT<M>, A> ma) =>
        ma.As().MapM(M.ToIOMaybe);

    static K<EffectT<M>, ForkIO<A>> MonadUnliftIO<EffectT<M>>.ForkIO<A>(
        K<EffectT<M>, A> ma,
        Option<TimeSpan> timeout) =>
        MonadT.lift<EffectT<M>, M, ForkIO<A>>(ma.As().Run().ForkIOMaybe(timeout));
    
    static K<EffectT<M>, B> Applicative<EffectT<M>>.Action<A, B>(
        K<EffectT<M>, A> ma,
        K<EffectT<M>, B> mb) =>
        EffectT.liftM(ma.As().Run().Action(mb.As().Run()));

    static K<EffectT<M>, A> Applicative<EffectT<M>>.Actions<A>(
        IterableNE<K<EffectT<M>, A>> fas) =>
        fas.Select(fa => fa.As().Proxy.Kind())
           .Actions()
           .ToEffect();
}
